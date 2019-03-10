using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheBall;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;

namespace WebCoreLayer.Controllers
{
    [Produces("application/json")]
    public class LoginController : Controller
    {
        public const string AnonymousEmail = "no-reply-anon@theball.me";

        [AllowAnonymous]
        [HttpGet]
        public async Task ExternalLogin(string provider, string returnUrl)
        {
            var instanceName = InstanceConfig.Current.InstanceName;
            var instanceProvider = $"{instanceName}_{provider}";
            var properties = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };

            // Add returnUrl to properties -- if applicable
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                properties.Items.Add("returnUrl", returnUrl);

            // The ASP.NET Core 1.1 version of this line was
            // await HttpContext.Authentication.ChallengeAsync(provider, properties);
            await HttpContext.ChallengeAsync(instanceProvider, properties);
        }

        public class RequestParameters
        {
            public string email { get; set; }
        }

        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public async Task RequestPassword([FromBody] RequestParameters parameters)
        {

        }

        public class ResetParameters
        {
            public string password { get; set; }
            public string confirmPassword { get; set; }
        }

        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public async Task ResetPassword([FromBody] ResetParameters parameters)
        {

        }



        public class RegisterParameters
        {
            public string fullName { get; set; }
            public string email { get; set; }
            public string password { get; set; }
            public string confirmPassword { get; set; }
            public bool terms { get; set; }
        }

        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public async Task Register([FromBody] RegisterParameters parameters)
        {

        }


        public class LoginParameters
        {
            public string email { get; set; }
            public string password { get; set; }
        }

        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public async Task Login(string login, string password, string returnUrl)
        //public async Task Login([FromBody] LoginParameters parameters)
        {
            //string returnUrl = "";
            //string login = parameters.email;
            //string password = parameters.password;
            var instanceName = InstanceConfig.Current.InstanceName;
            var properties = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };

            string email = "";
            if (login == null)
            {
                login = "Anonymous";
                email = AnonymousEmail;
            }

            // Add returnUrl to properties -- if applicable
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                properties.Items.Add("returnUrl", returnUrl);

            // The ASP.NET Core 1.1 version of this line was
            // await HttpContext.Authentication.ChallengeAsync(provider, properties);
            //await HttpContext.ChallengeAsync(instanceProvider, properties);
            var claimsIdentity = new ClaimsIdentity(new Claim[] {new Claim(ClaimTypes.Name, login), new Claim(ClaimTypes.Email, email), new Claim(ClaimTypes.Sid, Guid.Empty.ToString())});
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task ValidateToken(string provider, string token)
        {
            var result = await VerifyFacebookAccessToken(token);
        }

        [HttpGet]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public class FacebookUserViewModel
        {
            public string id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string username { get; set; }
            public string email { get; set; }
        }

        public static async Task<FacebookUserViewModel> VerifyFacebookAccessToken(string accessToken)
        {
            FacebookUserViewModel fbUser = null;
            var path = "https://graph.facebook.com/me?fields=email,access_token=" + accessToken;
            var client = new HttpClient();
            var uri = new Uri(path);
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                fbUser = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserViewModel>(content);
            }
            return fbUser;
        }
    }
}