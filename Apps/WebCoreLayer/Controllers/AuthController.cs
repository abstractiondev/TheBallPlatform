using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebCoreLayer.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        [HttpGet]
        public async Task<string[]> Account()
        {
            var i = 0;
            return new string[] { "value1", "value2" };
        }
    }
}