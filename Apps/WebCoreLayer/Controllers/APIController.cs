using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KubeTool;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebCoreLayer.Controllers
{
    [AllowAnonymous]
    public class APIController : Controller
    {
        [HttpGet]
        public async Task<JsonResult> UpdateAPI()
        {
            var results = await KubeSupport.UpdatePlatformToLatest();
            JsonResult result = new JsonResult(new { Results = results});
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> GetAPIXmlDefinition(string namespaceName)
        {
            var contentFileName = namespaceName.Replace(".", "") + ".xml";
            var resourceStream = GetAPIResourceStream(namespaceName, contentFileName);
            var result = File(resourceStream, "text/xml", contentFileName);
            return result;
        }

        [HttpGet]
        public async Task<JsonResult> GetAPIList()
        {
            var apiNames = GetAPINames();
            var apiNamesContainer = new { apiNames = apiNames };
            JsonResult result = new JsonResult(apiNamesContainer);
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

        public static string[] GetAPINames()
        {
            // TODO: Filter candidates with same ruling <AssemblyName.namespace.name.namespacename.xml> to be included only.
            var asm = typeof(AzureSupport.WebSupport).Assembly;
            var names = asm.GetManifestResourceNames();
            var apiNames = names
                .Where(name => name.EndsWith(".xml"))
                .Select(name => String.Join('.', name.Split('.').Skip(1).SkipLast(2)))
                .ToArray();
            return apiNames;
        }

    }
}