using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebCoreLayer.Controllers
{
    [Produces("application/json")]
    public class LoginController : Controller
    {        
        // GET api/values
        [HttpGet]
        public async Task<string[]> Get()
        {
            var i = 0;
            return new string[] { "value1", "value2" };
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task ExternalLogin(string provider, string returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };

            // Add returnUrl to properties -- if applicable
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                properties.Items.Add("returnUrl", returnUrl);

            // The ASP.NET Core 1.1 version of this line was
            // await HttpContext.Authentication.ChallengeAsync(provider, properties);
            await HttpContext.ChallengeAsync(provider, properties);
        }
    }
}