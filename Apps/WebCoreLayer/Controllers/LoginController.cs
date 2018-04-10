using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheBall;
using TheBall.CORE.InstanceSupport;

namespace WebCoreLayer.Controllers
{
    [Produces("application/json")]
    public class LoginController : Controller
    {        
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

        [HttpGet]
        public async Task ValidateToken(string provider, string token)
        {
        }

        [HttpGet]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}