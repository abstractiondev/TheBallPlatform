using System.IO;
using System.Text;
using AzureSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlatformCoreTests
{
    [TestClass]
    public class JSONSerializeTests
    {
        const string CustomJson = @"
{
    'propno1':1,
    'propno2':2,
    'propstr1':'hello',
    'propstr2':'world'
}";
        [TestMethod]
        public void DeserializeUnknownObjectFromString()
        {
            var customObject = JSONSupport.GetExpandoObject(CustomJson);
        }


        [TestMethod]
        public void DeserializeUnknownObjectFromStream()
        {
            var writeStream = new MemoryStream();
            byte[] data;
            using (var writer = new StreamWriter(writeStream, Encoding.UTF8))
            {
                writer.Write(CustomJson);
                writer.Flush();
                data = writeStream.ToArray();
            }
            var readStream = new MemoryStream(data);
            var customObject = JSONSupport.GetExpandoObject(readStream);
        }
    }
}