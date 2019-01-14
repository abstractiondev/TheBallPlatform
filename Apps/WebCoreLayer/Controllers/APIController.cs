using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebCoreLayer.Controllers
{
    public class APIController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAPIXmlDefinition(string namespaceName)
        {
            var contentFileName = namespaceName.Replace(".", "") + ".xml";
            var resourceStream = GetAPIResourceStream(namespaceName, contentFileName);
            var result = File(resourceStream, "text/xml", contentFileName);
            return result;
        }

        public static Stream GetAPIResourceStream(string namespaceName, string contentFileName = null)
        {
            if(contentFileName == null)
                contentFileName = namespaceName.Replace(".", "") + ".xml";
            var asm = typeof(AzureSupport.WebSupport).Assembly;
            var resourceStream = asm.GetManifestResourceStream($"AzureSupport.{namespaceName}.{contentFileName}");
            if (resourceStream == null)
                throw new InvalidDataException($"Requested API Content not available: {namespaceName}");
            return resourceStream;
        }
    }
}