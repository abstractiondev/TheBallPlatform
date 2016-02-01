using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using AzureSupport;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.FacebookOAuth2;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using Microsoft.WindowsAzure;
using TheBall;
using TheBall.CORE.InstanceSupport;

namespace WebInterface
{
    public partial class TheBallLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["ssokey"] != null)
            {
                AuthenticationSupport.ClearAuthenticationCookie(Response);
                handleWilmaLogin();
                return;
            }
            if(Request.Params["SignOut"] != null)
            {
                AuthenticationSupport.ClearAuthenticationCookie(Response);
                Response.Redirect("/", true);
                return;
            }
            openIdBox.Focus();         
            OpenIdRelyingParty openid = new OpenIdRelyingParty();        
            var openIDResponse = openid.GetResponse();
            string idprovider = Request.Params["idprovider"];
            string idProviderUrl = Request.Params["idProviderUrl"];
            bool requestEmail = Request.Params["requestEmail"] != null;
            string redirectUrl = Request.Params["ReturnUrl"];
            if (openIDResponse != null)
            {
                handleOpenIDResponse(openIDResponse);
            } else
            {
                string oauthCode = Request.Params["code"];
                if (String.IsNullOrEmpty(oauthCode) == false)
                {
                    string state = Request.Params["state"];
                    var stateParts = state.Split(new[] {"&"}, StringSplitOptions.None);
                    var providerPart = stateParts[0].Replace("TBAProvider=", "");
                    var redirectPart = stateParts[1];
                    string authRedirectUrl = redirectPart.Contains("RedirectUrl=") ? redirectPart.Replace("RedirectUrl=", "") : FormsAuthentication.DefaultUrl;
                    FinalizeOAuthLogin(oauthCode, providerPart, authRedirectUrl);
                    return;
                }
                if(String.IsNullOrEmpty(idProviderUrl) == false)
                {
                    if (idProviderUrl == "https://www.google.com/accounts/o8/id")
                    {
                        var req = Request;
                        string currentReturnUrl = req.Url.GetLeftPart(UriPartial.Path) + (String.IsNullOrEmpty(redirectUrl) ? "?TBAProvider=Google" : "?TBAProvider=Google&RedirectUrl=" + redirectUrl);
                        var loginUrl = GetGoogleServiceLoginUrl(new Uri(currentReturnUrl));
                        Response.Redirect(loginUrl.AbsoluteUri);
                    }
                    else if (idProviderUrl == "https://www.facebook.com/dialog/oauth")
                    {
                        var req = Request;
                        string currentReturnUrl = req.Url.GetLeftPart(UriPartial.Path) + (String.IsNullOrEmpty(redirectUrl) ? "?TBAProvider=Facebook" : "?TBAProvider=Facebook&RedirectUrl=" + redirectUrl);
                        FacebookOAuth2Client client = new FacebookOAuth2Client(appId:SecureConfig.Current.FacebookOAuthClientID, appSecret:SecureConfig.Current.FacebookOAuthClientSecret,
                            requestedScopes:"email");
                        client.RequestAuthentication(new HttpContextWrapper(HttpContext.Current), new Uri(currentReturnUrl));
                        //var loginUrl = GetFacebookServiceLoginUrl(new Uri(currentReturnUrl));
                        //Response.Redirect(loginUrl.AbsoluteUri);
                    }
                    else
                    {
                        CreateOpenIDRequestAndRedirect(idProviderUrl, requestEmail);
                    }
                    return;
                }
                if (idprovider != null)
                {
                    performLoginForProvider(idprovider, requestEmail);
                }
            }
        }

        private string FinalizeOAuthLogin(string oauthCode, string provider, string redirectUrl)
        {
            Tuple<string, string> authTokens;
            string userName;
            string emailAddress;
            DateTime limitTimestamp = DateTime.UtcNow;
            if (provider == "Google")
            {
                authTokens = GetGoogleAuthTokens(new Uri(Request.Url.GetLeftPart(UriPartial.Path)), oauthCode);
                var jwtToken = new JwtSecurityToken(authTokens.Item2);
                if (jwtToken.ValidTo < limitTimestamp)
                    throw new SecurityException("Token expired");
                string myUserID = jwtToken.Claims.First(claim => claim.Type == "openid_id").Value;
                string myEmail = jwtToken.Claims.First(claim => claim.Type == "email").Value;
                bool emailVerified =
                    Boolean.Parse(jwtToken.Claims.First(claim => claim.Type == "email_verified").Value);
                userName = myUserID;
                emailAddress = emailVerified ? myEmail : null;
            }
            else if (provider == "Facebook")
            {
                authTokens = GetFacebookAuthTokens(new Uri(Request.Url.GetLeftPart(UriPartial.Path)), oauthCode);
                userName = $"https://www.facebook.com/{authTokens.Item1}";
                emailAddress = authTokens.Item2;
            }
            else
                throw new NotSupportedException("Provider not supported: " + provider);
            validateEmailAndExitForRestricted(emailAddress);
            AuthenticationSupport.SetAuthenticationCookie(Response, userName, emailAddress);
            Response.Redirect(redirectUrl, true);
            return redirectUrl;
        }

        private void validateEmailAndExitForRestricted(string emailAddress)
        {
            if (InstanceConfig.Current.HasEmailAddressRestriction)
            {
                bool isValid = String.IsNullOrEmpty(emailAddress) == false && 
                    InstanceConfig.Current.RestrictedEmailAddresses.Any(
                        okEmail => okEmail.ToLower().Trim() == emailAddress.ToLower().Trim());
                if (!isValid)
                    Response.Redirect("/", true);
            }
        }

        private void performLoginForProvider(string idprovider, bool requestEmail)
        {
            switch (idprovider)
            {
                case "google":
                    PerformGoogleLogin(requestEmail);
                    return;
                case "yahoo":
                    PerformYahooLogin(requestEmail);
                    return;
                case "aol":
                    PerformAOLLogin();
                    return;
                case "wordpress":
                    openIdBox.Text = "http://ENTER-YOUR-BLOG-NAME-HERE.wordpress.com";
                    break;
                default:
                    break;
            }
        }

        private void handleOpenIDResponse(IAuthenticationResponse openIDResponse)
        {
            switch (openIDResponse.Status)
            {
                case AuthenticationStatus.Authenticated:
                    // This is where you would look for any OpenID extension responses included
                    // in the authentication assertion.                                
                    var claimsResponse = openIDResponse.GetExtension<ClaimsResponse>();
                    var fetchResponse = openIDResponse.GetExtension<FetchResponse>();
                    var friendlyName = openIDResponse.FriendlyIdentifierForDisplay;
                    bool isTrustableProvider = isTrustableFriendlyName(friendlyName);
                    string emailAddress = null;
                    if (fetchResponse != null && isTrustableProvider)
                    {
                        emailAddress = fetchResponse.GetAttributeValue(WellKnownAttributes.Contact.Email);
                        validateEmailAndExitForRestricted(emailAddress);
                    }
                    var profileFields = claimsResponse;
                    // Store off the "friendly" username to display -- NOT for username lookup                                
                    // Use FormsAuthentication to tell ASP.NET that the user is now logged in,                                
                    // with the OpenID Claimed Identifier as their username. 
                    string userName = openIDResponse.ClaimedIdentifier.ToString();
                    //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userName,
                    //                                                                 DateTime.Now,
                    //                                                                 DateTime.Now.AddDays(10),
                    //                                                                 true, "user custom data");

                    AuthenticationSupport.SetAuthenticationCookie(Response, userName, emailAddress);
                    //FormsAuthentication.RedirectFromLoginPage(response.ClaimedIdentifier, false);
                    //string redirectUrl = FormsAuthentication.GetRedirectUrl(userName, true);
                    string redirectUrl = Request.Params["ReturnUrl"];
                    if (redirectUrl == null)
                        redirectUrl = FormsAuthentication.DefaultUrl;
                    Response.Redirect(redirectUrl, true);
                    break;
                case AuthenticationStatus.Canceled:
                    this.loginCanceledLabel.Visible = true;
                    break;
                case AuthenticationStatus.Failed:
                    this.loginFailedLabel.Visible = true;
                    break;
            }
        }

        private bool isTrustableFriendlyName(string friendlyName)
        {
            if (friendlyName == "www.google.com/accounts/o8/id")
                return true;
            if (friendlyName.StartsWith("me.yahoo.com/a/"))
                return true;
            if (friendlyName.StartsWith("steamcommunity.com/openid/id/"))
                return true;
            return false;
        }

        private void handleOAuth2()
        {
        }

        private void handleWilmaLogin()
        {
            string ssokey = Request.Params["ssokey"];
            string query = Request.Params["query"];
            string logout = Request.Params["logout"];
            string nonce = Request.Params["nonce"];
            string h = Request.Params["h"];
            if(nonce.Length < 16 || nonce.Length > 40)
                throw new SecurityException("Invalid login parameters");
            string hashSourceStr = string.Format("ssokey={0}&query={1}&logout={2}&nonce={3}",
                ssokey, query, logout, nonce
                /*
                HttpUtility.UrlEncode(ssokey),
                HttpUtility.UrlEncode(query),
                HttpUtility.UrlEncode(logout),
                HttpUtility.UrlEncode(nonce)*/
                );
            var hashSourceBin = Encoding.UTF8.GetBytes(hashSourceStr);
            //throw new NotImplementedException("Wilma login functional, pre-shared secret needs to be config/non-source code implemented");
            string wilmaSharedSecret = SecureConfig.Current.WilmaSharedSecret;
            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(wilmaSharedSecret)); // TODO: Dynamic config load from Ball-instance specific container
            var hashValue = hmacsha1.ComputeHash(hashSourceBin);
            string hashValueStr = Convert.ToBase64String(hashValue);
            // For now Wilma doesn't escape + characters in base64 on query string - test against with and without + chars
            string tempFixedHashValue = hashValueStr.Replace("+", " ");
            if (hashValueStr != h && tempFixedHashValue != h)
                throw new SecurityException("Invalid hash value - phase 1");

            // Login verification done, then call to Wilma for user results
            RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
            var cryptoData = new byte[16];
            rngCrypto.GetBytes(cryptoData);
            string myLogout = "";
            string myNonce = Convert.ToBase64String(cryptoData);
            string myHSource = string.Format("ssokey={0}&logout={1}&nonce={2}",
                ssokey, myLogout, myNonce);
            var myHHash = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(myHSource));
            string myH = Convert.ToBase64String(myHHash);
            string wilmaRequestUrl = String.Format("{0}?ssokey={1}&logout={2}&nonce={3}&h={4}",
                query,
                ssokey,
                HttpUtility.UrlEncode(myLogout),
                HttpUtility.UrlEncode(myNonce), 
                HttpUtility.UrlEncode(myH));
            HttpWebRequest wilmaRequest = WebRequest.CreateHttp(wilmaRequestUrl);
            Debug.WriteLine(myNonce);
            Debug.WriteLine(myNonce.Length);
            var response = wilmaRequest.GetResponse();
            var stream = response.GetResponseStream();
            string content = null;
            var isoEnc = Encoding.GetEncoding("ISO-8859-1");
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
            /*
            using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1")))
            {
            var dataBuffer = new byte[100*1024];
            using(BinaryReader bReader = new BinaryReader(stream)) {
                var byteCount = bReader.Read(dataBuffer, 0, dataBuffer.Length);
                var convertedContent = Encoding.Convert(Encoding.ASCII, isoEnc,
                    dataBuffer, 0, byteCount);
                content = isoEnc.GetString(convertedContent);
             */
                content = reader.ReadToEnd();
                // Wilma got fixed for scandinavian character usage, no need to replace anymore
                //content = content.Replace("ä", "??").Replace("ö", "??");
                int prehPosition = content.LastIndexOf("\r\nh=");
                if(prehPosition < 0)
                    throw new SecurityException("Invalid h position");
                int hPosition = prehPosition + 2;
                var contentWithoutH = content.Substring(0, hPosition);
                //var verifyH = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(contentWithoutH));
                var verifyH = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(contentWithoutH));
                var verifyH2 = hmacsha1.ComputeHash(isoEnc.GetBytes(contentWithoutH));
                string verifyHTxt = Convert.ToBase64String(verifyH);
                string verifyH2Txt = Convert.ToBase64String(verifyH2);
                string hValueFromContent = content.Substring(hPosition + 2);
                if(hValueFromContent != verifyHTxt)
                    throw new SecurityException("Invalid hash value - phase 2");
                string wilmaLoginName = getWilmaContentValue(content, "login");
                string secureProtocolPrefix = "https://";
                if(query.StartsWith(secureProtocolPrefix) == false)
                    throw new SecurityException("Invalid URL for UserID");
                //string queryWithoutPrefix = query.Substring(secureProtocolPrefix.Length);
                string wilmaUserID = query + "/" + wilmaLoginName;
                AuthenticationSupport.SetAuthenticationCookie(Response, wilmaUserID, null);
                string redirectUrl = Request.Params["ReturnUrl"];
                if (redirectUrl == null)
                    redirectUrl = FormsAuthentication.DefaultUrl;
                Response.Redirect(redirectUrl, true);
            }
        }

        private string getWilmaContentValue(string content, string propertyName)
        {
            string linefeedStr = "\r\n";
            string searchString = linefeedStr + propertyName + "=";
            int searchMatchIX = content.LastIndexOf(searchString);
            if(searchMatchIX < 0)
                throw new ArgumentException("Propertyname not found in content: " + propertyName);
            int propValueStartIX = searchMatchIX + searchString.Length;
            int nextLineFeedIX = content.IndexOf(linefeedStr, propValueStartIX);
            if(nextLineFeedIX < 0)
                throw new InvalidDataException("Content not ending to (h +) linefeed: " + content);
            int valueLength = nextLineFeedIX - propValueStartIX;
            return content.Substring(propValueStartIX, valueLength);

        }

        protected void openidValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // This catches common typos that result in an invalid OpenID Identifier.        
            args.IsValid = Identifier.IsValid(args.Value);
        }

        protected void loginButton_Click(object sender, EventArgs e)
        {
            if (!this.Page.IsValid)
            {
                return;
                // don't login if custom validation failed.        
            }
            //string openIdUrl = this.openIdBox.Text;
            //CreateOpenIDRequestAndRedirect(openIdUrl);
        }

        private void CreateOpenIDRequestAndRedirect(string openIdUrl, bool requestEmail = false)
        {
            try
            {
                using (OpenIdRelyingParty openid = new OpenIdRelyingParty())
                {
                    IAuthenticationRequest request = openid.CreateRequest(openIdUrl);
                    // This is where you would add any OpenID extensions you wanted                        
                    // to include in the authentication request.                        
                    request.AddExtension(new ClaimsRequest
                                             {
                                                 //Country = DemandLevel.Request,
                                                 Email = requestEmail ? DemandLevel.Require : DemandLevel.NoRequest,
                                                 //Gender = DemandLevel.Require,
                                                 //PostalCode = DemandLevel.Require,
                                                 //TimeZone = DemandLevel.Require,
                                             }); // Send your visitor to their Provider for authentication.
                    request.RedirectToProvider();
                }
            }
            catch (ProtocolException ex)
            {
                // The user probably entered an Identifier that                
                // was not a valid OpenID endpoint.                
                this.openidValidator.Text = ex.Message;
                this.openidValidator.IsValid = false;
            }
        }

        protected void bGoogleLogin_Click(object sender, EventArgs e)
        {
            PerformGoogleLogin(false);
        }



        private void PerformAOLLogin()
        {
            CreateOpenIDRequestAndRedirect("https://www.aol.com");
        }


        private void PerformGoogleLogin(bool requestEmail)
        {
            CreateOpenIDRequestAndRedirect("https://www.google.com/accounts/o8/id", requestEmail);
        }

        private void PerformYahooLogin(bool requestEmail)
        {
            CreateOpenIDRequestAndRedirect("https://me.yahoo.com", requestEmail);
        }

        protected static Uri GetGoogleServiceLoginUrl(Uri returnUrl)
        {
            string authorizationEndpoint = "https://accounts.google.com/o/oauth2/auth";
            var state = string.IsNullOrEmpty(returnUrl.Query) ? string.Empty : returnUrl.Query.Substring(1);
            var req = HttpContext.Current.Request;
            string client_id = SecureConfig.Current.GoogleOAuthClientID;

            return BuildUri(authorizationEndpoint, new NameValueCollection
            {
                { "response_type", "code" },
                { "client_id", client_id },
                { "scope", "openid email" },
                // { "prompt", "select_account"},
                { "openid.realm", "https://" + req.Url.DnsSafeHost + (req.Url.IsDefaultPort ? "" : ":" + req.Url.Port) + "/" },
                { "redirect_uri", returnUrl.GetLeftPart(UriPartial.Path) },
                { "state", state },
            });
        }

        private static Uri BuildUri(string baseUri, NameValueCollection queryParameters)
        {
            var q = HttpUtility.ParseQueryString(string.Empty);
            q.Add(queryParameters);
            var builder = new UriBuilder(baseUri) { Query = q.ToString() };
            return builder.Uri;
        }

        protected static Tuple<string, string> GetFacebookAuthTokens(Uri returnUrl, string authorizationCode)
        {
            var client = new FacebookOAuth2Client(appId: SecureConfig.Current.FacebookOAuthClientID,
                appSecret: SecureConfig.Current.FacebookOAuthClientSecret, requestedScopes:"email");
            var authResult = client.VerifyAuthentication(new HttpContextWrapper(HttpContext.Current), returnUrl);
            var data = authResult.ExtraData;
            //bool isVerified = data.ContainsKey("verified") ? authResult.ExtraData["verified"] == "True" : false;
            //if(!isVerified)
            //    throw new SecurityException("Authentication data not verified");
            var emailAddress = data.ContainsKey("email") ? data["email"] : null;
            if(emailAddress == null)
                throw new SecurityException("Email is required for login");
            if(!data.ContainsKey("id"))
                throw new SecurityException("ID is required for login");
            var result = new Tuple<string, string>(data["id"], emailAddress);
            return result;
        }


        protected static Tuple<string, string> GetGoogleAuthTokens(Uri returnUrl, string authorizationCode)
        {
            string TokenEndpoint = "https://accounts.google.com/o/oauth2/token";
            var postData = HttpUtility.ParseQueryString(string.Empty);
            string client_id = SecureConfig.Current.GoogleOAuthClientID;
            string client_secret = SecureConfig.Current.GoogleOAuthClientSecret;
            var nvc = new NameValueCollection
            {
                {"grant_type", "authorization_code"},
                {"code", authorizationCode},
                {"client_id", client_id},
                {"client_secret", client_secret},
                {"redirect_uri", returnUrl.GetLeftPart(UriPartial.Path)},
            };
            postData.Add(nvc);
            var webRequest = (HttpWebRequest)WebRequest.Create(TokenEndpoint);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            using (var s = webRequest.GetRequestStream())
            using (var sw = new StreamWriter(s))
                sw.Write(postData.ToString());
            using (var webResponse = webRequest.GetResponse())
            {
                var responseStream = webResponse.GetResponseStream();
                if (responseStream == null)
                    return null;

                var json = JSONSupport.GetObjectFromStream<OAuthData>(responseStream);
                var accessToken = json.access_token;
                var idToken = json.id_token;
                return new Tuple<string, string>(accessToken, idToken);
            }
        }

        [DataContract]
        private class OAuthData
        {
            [DataMember]
            public string id_token;
            [DataMember]
            public string access_token;
        }
    }
}