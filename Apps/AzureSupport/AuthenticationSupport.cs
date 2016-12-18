using System;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Web;
using AzureSupport;
using TheBall.CORE;

namespace TheBall
{
    public class TheBallIdentity : GenericIdentity
    {
        public readonly string EmailAddress;
        public readonly string AccountID;

        public TheBallIdentity(string userName, string emailAddress, string accountID) : base(userName, "theball")
        {
            EmailAddress = emailAddress;
            AccountID = accountID;
        }
    }

    public static class AuthenticationSupport
    {
        private const string AuthCookieName = "TheBall_AUTH";
        private const string EmailCookieName = "TheBall_EMAIL";
        private const string AuthStrSeparator = ">TheBall<";
        private const int TimeoutSeconds = 1800;
        //private const int TimeoutSeconds = 10;
        public static void SetAuthenticationCookie(HttpResponse response, string validUserName, string emailAddress, string accountID)
        {
            if (emailAddress == null)
                emailAddress = "";
            string cookieValue = DateTime.UtcNow.Ticks + AuthStrSeparator + validUserName + AuthStrSeparator + accountID + AuthStrSeparator + emailAddress;
            string authString = EncryptionSupport.EncryptStringToBase64(cookieValue);
            if(response.Cookies[AuthCookieName] != null)
                response.Cookies.Remove(AuthCookieName);
            if(response.Cookies[EmailCookieName] != null)
                response.Cookies.Remove(EmailCookieName);
            HttpCookie cookie = new HttpCookie(AuthCookieName, authString);
            // Session limit from browser
            //cookie.Expires = DateTime.UtcNow.AddSeconds(TimeoutSeconds); 
            cookie.HttpOnly = false;
            cookie.Secure = true;
            HttpContext.Current.Response.Cookies.Add(cookie);
            HttpCookie emailCookie = new HttpCookie(EmailCookieName, emailAddress);
            emailCookie.HttpOnly = false;
            emailCookie.Secure = true;
            HttpContext.Current.Response.Cookies.Add(emailCookie);
        }

        public static void SetUserAuthentication(HttpContext context, string userName, string emailAddress,
            string accountID)
        {
            if (String.IsNullOrEmpty(accountID))
            {
                var loginID = Login.GetLoginIDFromLoginURL(userName);
                var login = ObjectStorage.RetrieveFromOwnerContent<Login>(SystemSupport.SystemOwner, loginID);
                accountID = login?.Account;
            }
            context.User = new GenericPrincipal(new TheBallIdentity(userName, emailAddress, accountID), null);
            // Reset cookie time to be again timeout from this request
            //encCookie.Expires = DateTime.Now.AddSeconds(TimeoutSeconds);
            //context.Response.Cookies.Set(encCookie);
            SetAuthenticationCookie(context.Response, userName, emailAddress, accountID);
        }

        public static void SetUserFromCookieIfExists(HttpContext context)
        {
            var request = HttpContext.Current.Request;
            var encCookie = request.Cookies[AuthCookieName];
            if (encCookie != null)
            {
                try
                {
                    string cookieValue = EncryptionSupport.DecryptStringFromBase64(encCookie.Value);
                    var valueSplit = cookieValue.Split(new[] {AuthStrSeparator}, StringSplitOptions.None);
                    long ticks = long.Parse(valueSplit[0]);
                    DateTime cookieTime = new DateTime(ticks);
                    DateTime utcNow = DateTime.UtcNow;
                    if(cookieTime.AddSeconds(TimeoutSeconds) < utcNow)
                        throw new SecurityException("Cookie expired");
                    string userName = valueSplit[1];
                    string accountID = valueSplit[2];
                    string emailAddress = valueSplit[3];
                    SetUserAuthentication(context, userName, emailAddress, accountID);
                } catch
                {
                    ClearAuthenticationCookie(context.Response);
                }
            }
            
        }

        public static void ClearAuthenticationCookie(HttpResponse response)
        {
            HttpCookie cookie = new HttpCookie(AuthCookieName);
            cookie.Expires = DateTime.Today.AddDays(-1);
            if(response.Cookies[AuthCookieName] != null)
                response.Cookies.Set(cookie);
            else
                response.Cookies.Add(cookie);
        }
    }
}