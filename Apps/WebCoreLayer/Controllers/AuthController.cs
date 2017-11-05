using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebCoreLayer.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class AuthController : Controller
    {
        [HttpGet]
        public async Task Account(string path)
        {
            Response.StatusCode = 200;
            await Response.WriteAsync($"Account (path: {path}): {Request.Path}");
        }

        [HttpGet]
        public async Task Group(string groupId, string path)
        {
            Response.StatusCode = 200;
            await Response.WriteAsync($"Group {groupId} (path: {path}): {Request.Path}");
        }
    }
}