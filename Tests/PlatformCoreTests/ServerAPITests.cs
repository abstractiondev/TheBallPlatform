using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCoreLayer.Controllers;

namespace PlatformCoreTests
{
    [TestClass]
    public class ServerAPITests
    {
        [TestMethod]
        public async Task GetAPIXmlFromControllerTest()
        {
            var resourceStream = APIController.GetAPIResourceStream("Footvoter.Services", "FootvoterServices.xml");
            string apiXMLContent;
            using (var reader = new StreamReader(resourceStream))
            {
                apiXMLContent = reader.ReadToEnd();
            }
            Assert.IsTrue(apiXMLContent?.Length > 10);
        }

        [TestMethod]
        public async Task GetAPIXmlFromControllerDefaultFilenameTest()
        {
            var resourceStream = APIController.GetAPIResourceStream("Footvoter.Services");
            string apiXMLContent;
            using (var reader = new StreamReader(resourceStream))
            {
                apiXMLContent = reader.ReadToEnd();
            }
            Assert.IsTrue(apiXMLContent?.Length > 10);
        }

    }
}