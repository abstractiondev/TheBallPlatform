using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebCoreLayer.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        [HttpGet]
        public async Task Account()
        {
            var i = 0;
            Response.StatusCode = 200;
            await HttpContext.Response.WriteAsync("Tööt");
        }
    }
}