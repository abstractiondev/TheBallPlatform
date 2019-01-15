using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCoreLayer.Controllers;

namespace PlatformCoreTests
{
    [TestClass]
    public class ServerAPITests
    {
        [TestMethod]
        public void VerifyAllResourceNames()
        {
            /*
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => !asm.IsDynamic)
                .Where(asm => asm.FullName.Contains("AzureSupport"))
                //.Concat(new Assembly[] { typeof(AzureSupport.WebSupport).Assembly})
                .ToArray();*/
            var assemblies = new Assembly[] { typeof(AzureSupport.WebSupport).Assembly};
            Parallel.ForEach(assemblies, asm =>
            {
                var names = asm.GetManifestResourceNames().Where(name => name.ToLower().EndsWith(".xml"))
                    .ToArray();
                Parallel.ForEach(names, name =>
                {
                    var resourceStream = asm.GetManifestResourceStream(name);
                    XDocument xDoc = null;
                    using (var reader = XmlReader.Create(resourceStream))
                    {
                        try
                        {
                            xDoc = XDocument.Load(reader);
                        }
                        catch
                        {

                        }
                    }
                    if (xDoc == null)
                        return;
                    var theBallElement = xDoc.Descendants()
                        .FirstOrDefault(elem => elem.Name.LocalName == "InstanceOfTheBall");
                    var semanticName = theBallElement?.Attribute("semanticDomainName")?.Value;
                    if (semanticName == null)
                        return;
                    var nameSplit = name.Split('.');
                    var namespaceLength = nameSplit.Length - 3;
                    var assemblyLength = 1;
                    var namespacePart = String.Join('.', nameSplit.Skip(assemblyLength).Take(namespaceLength).ToArray());
                    var contentNamePart = String.Join('.', nameSplit.Skip(assemblyLength).Skip(namespaceLength).ToArray());
                    var expectedName = namespacePart.Replace(".", "") + ".xml";
                    Assert.AreEqual(expectedName, contentNamePart);
                    Console.WriteLine($"Validated resource: {namespacePart}.{contentNamePart}");
                });
            });
        }

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