using System;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
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
        private const string AccountIDCookieName = "TheBall_ACCOUNT";
        private const string ClientMetadataCookieName = "TheBall_META";
        private const string AuthStrSeparator = ">TB<";
        private const int TimeoutSeconds = 1800;
        //private const int TimeoutSeconds = 10;

        public static void SetAuthenticationCookieFromUserName(HttpResponse response, string validUserName,
            string emailAddress)
        {
            var loginID = Login.GetLoginIDFromLoginURL(validUserName);
            var login = ObjectStorage.RetrieveFromOwnerContent<Login>(SystemOwner.CurrentSystem, loginID);
            var accountID = login?.Account;
            string base64ClientMetadata = null;
            if (accountID != null)
            {
                var account = ObjectStorage.RetrieveFromOwnerContent<Account>(SystemOwner.CurrentSystem, accountID);
                base64ClientMetadata = account?.GetClientMetadataAsBase64();
            }
            SetAuthenticationCookie(response, validUserName, emailAddress, accountID, base64ClientMetadata);
        }

        public static void SetAuthenticationCookie(HttpResponse response, string validUserName, string emailAddress, string accountID,
            string base64ClientMetadata)
        {
            if (emailAddress == null)
                emailAddress = "";
            string cookieValue = String.Join(AuthStrSeparator, 
                DateTime.UtcNow.Ticks.ToString(), 
                validUserName, 
                accountID, 
                emailAddress, 
                base64ClientMetadata);
            string authString = EncryptionSupport.EncryptStringToBase64(cookieValue);

            setResponseCookie(response, new HttpCookie(AuthCookieName, authString)
            {
                HttpOnly = true,
                Secure = true
            });

            setResponseCookie(response, new HttpCookie(EmailCookieName, emailAddress)
            {
                HttpOnly = false,
                Secure = true
            });

            setResponseCookie(response, new HttpCookie(AccountIDCookieName, accountID)
            {
                HttpOnly = false,
                Secure = true
            });

            if (!String.IsNullOrEmpty(base64ClientMetadata))
            {
                var clientMetadataJSONData = Convert.FromBase64String(base64ClientMetadata);
                var clientMetadataJSONString = Encoding.UTF8.GetString(clientMetadataJSONData);
                setResponseCookie(response, new HttpCookie(ClientMetadataCookieName, clientMetadataJSONString)
                {
                  HttpOnly = false,
                  Secure = true
                });

            }
        }

        private static void setResponseCookie(HttpResponse response, HttpCookie cookie)
        {
            if(response.Cookies[cookie.Name] != null)
                response.Cookies.Remove(cookie.Name);
            response.Cookies.Add(cookie);
        }

        public static void SetUserAuthentication(HttpContext context, string userName, string emailAddress,
            string accountID, string base64ClientMetadata)
        {
            context.User = new GenericPrincipal(new TheBallIdentity(userName, emailAddress, accountID), null);
            SetAuthenticationCookie(context.Response, userName, emailAddress, accountID, base64ClientMetadata);
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
                    string base64ClientMeta = valueSplit[4];
                    SetUserAuthentication(context, userName, emailAddress, accountID, base64ClientMeta);
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
            setResponseCookie(response, cookie);
        }
    }
}