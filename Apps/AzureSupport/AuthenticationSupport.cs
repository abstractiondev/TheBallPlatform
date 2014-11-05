using System;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Web;
using AzureSupport;

namespace TheBall
{
    public static class AuthenticationSupport
    {
        private const string AuthCookieName = "TheBall_AUTH";
        private const string EmailCookieName = "TheBall_EMAIL";
        private const string AuthStrSeparator = ">TheBall<";
        private const int TimeoutSeconds = 1800;
        //private const int TimeoutSeconds = 10;
        public static void SetAuthenticationCookie(HttpResponse response, string validUserName, string emailAddress)
        {
            string cookieValue = DateTime.UtcNow.Ticks + AuthStrSeparator + validUserName;
            if (emailAddress != null)
                cookieValue += AuthStrSeparator + emailAddress;
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
            if (emailAddress != null)
            {
                HttpCookie emailCookie = new HttpCookie(EmailCookieName, emailAddress);
                cookie.HttpOnly = false;
                cookie.Secure = true;
                HttpContext.Current.Response.Cookies.Add(emailCookie);
            }
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
                    string emailAddress = null;
                    if (valueSplit.Length > 2)
                        emailAddress = valueSplit[2];
                    context.User = new GenericPrincipal(new GenericIdentity(userName, "theball"), new string[0]);
                    // Reset cookie time to be again timeout from this request
                    //encCookie.Expires = DateTime.Now.AddSeconds(TimeoutSeconds);
                    //context.Response.Cookies.Set(encCookie);
                    SetAuthenticationCookie(context.Response, userName, emailAddress);
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