using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBall;
using TheBall.CORE;

namespace WebCoreLayer.Controllers
{
    public class UIController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task UI(string path)
        {
            await HandleUIRequest(path);
        }

        private async Task HandleUIRequest(string requestPath)
        {
            var context = HttpContext;
            var contentPath = "ui/" + requestPath;
            //var containerOwner = InformationContext.CurrentOwner != SystemSupport.SystemOwner ? InformationContext.CurrentOwner : SystemSupport.AnonymousOwner;
            var containerOwner = SystemSupport.AnonymousOwner;
            await AuthController.HandleOwnerGetRequest(containerOwner, context, contentPath);
        }

    }
}