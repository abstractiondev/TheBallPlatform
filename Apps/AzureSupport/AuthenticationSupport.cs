using System;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AzureSupport;
using Microsoft.AspNetCore.Http;
using TheBall.Core;

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

        [Obsolete("error", true)]
        public static async Task SetAuthenticationCookieFromUserName(HttpResponse response, string validUserName,
            string emailAddress)
        {
            var loginID = Login.GetLoginIDFromLoginURL(validUserName);
            var login = await ObjectStorage.RetrieveFromOwnerContentA<Login>(SystemOwner.CurrentSystem, loginID);
            var accountID = login?.Account;
            string base64ClientMetadata = null;
            if (accountID != null)
            {
                var account = await ObjectStorage.RetrieveFromOwnerContentA<Account>(SystemOwner.CurrentSystem, accountID);
                base64ClientMetadata = account?.GetClientMetadataAsBase64();
            }
            SetAuthenticationCookie(response, validUserName, emailAddress, accountID, base64ClientMetadata);
        }

        [Obsolete("error", true)]
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

            setResponseCookie(response, AuthCookieName, authString, true);
            setResponseCookie(response, EmailCookieName, emailAddress, false);
            setResponseCookie(response, AccountIDCookieName, accountID, false);


            string clientMetadataJSONString = null;
            if (!String.IsNullOrEmpty(base64ClientMetadata))
            {
                var clientMetadataJSONData = Convert.FromBase64String(base64ClientMetadata);
                clientMetadataJSONString = Encoding.UTF8.GetString(clientMetadataJSONData);

            }
            setResponseCookie(response, ClientMetadataCookieName, clientMetadataJSONString, true);
        }

        private static void setResponseCookie(HttpResponse response, string cookieName, string cookieValue, bool httpOnly, DateTimeOffset? expires = null)
        {
            response.Cookies.Delete(cookieName);
            response.Cookies.Append(cookieName, cookieValue, new CookieOptions { HttpOnly = httpOnly, Secure = true, Expires = expires });
        }

        [Obsolete("error", true)]
        public static void SetUserAuthentication(HttpContext context, string userName, string emailAddress,
            string accountID, string base64ClientMetadata)
        {
            context.User = new GenericPrincipal(new TheBallIdentity(userName, emailAddress, accountID), null);
            SetAuthenticationCookie(context.Response, userName, emailAddress, accountID, base64ClientMetadata);
        }

        [Obsolete("error", true)]
        public static void SetUserFromCookieIfExists(HttpContext context)
        {
            var request = context.Request;
            var encCookie = request.Cookies[AuthCookieName];
            if (encCookie != null)
            {
                try
                {
                    string cookieValue = EncryptionSupport.DecryptStringFromBase64(encCookie);
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
            setResponseCookie(response, AuthCookieName, "", true, expires:DateTime.Today.AddDays(-1));
        }
    }
}