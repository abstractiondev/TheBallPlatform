using System.Dynamic;
using System.IO;
using System.Text;
using AzureSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheBall.Interface.INT;

namespace PlatformCoreTests
{
    [TestClass]
    public class JSONSerializeTests
    {
        const string CustomJson = @"{
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

        [TestMethod]
        public void SerializeUnknownObjectToString()
        {
            var customObject = JSONSupport.GetExpandoObject(CustomJson);
            var serialized = JSONSupport.SerializeToJSONString(customObject);
            Assert.IsTrue(serialized.Contains("propno1"));
            Assert.IsTrue(serialized.Contains("propno2"));
            Assert.IsTrue(serialized.Contains("hello"));
            Assert.IsTrue(serialized.Contains("world"));
        }

        [TestMethod]
        public void SerializeDeserializeCustomJSONData1()
        {
            var expObj = new ExpandoObject();
            dynamic dynObj = expObj;
            dynObj.Prop1 = "prop1";
            dynObj.Prop2 = "prop2";
            dynObj.Prop3 = 3;
            dynObj.Prop4 = 4.0;
            CustomJSONData testData = new CustomJSONData()
            {
                Name = "DataName",
                Data = dynObj
            };
            var serialized = JSONSupport.SerializeToJSONString(testData);
            dynamic deser = JSONSupport.GetExpandoObject(serialized);
            var origObj = deser.Data;
            Assert.IsTrue(origObj.Prop1 == "prop1");
            Assert.IsTrue(origObj.Prop2 == "prop2");
            Assert.IsTrue(origObj.Prop3 == 3);
            Assert.IsTrue(origObj.Prop4 == 4.0);
        }

        [TestMethod]
        public void SerializeDeserializeCustomJSONData2()
        {
            var expObj = new ExpandoObject();
            dynamic dynObj = expObj;
            dynObj.Prop1 = "prop1";
            dynObj.Prop2 = "prop2";
            dynObj.Prop3 = 3;
            dynObj.Prop4 = 4.0;
            CustomJSONData testData = new CustomJSONData()
            {
                Name = "DataName",
                Data = dynObj
            };
            var serialized = JSONSupport.SerializeToJSONString(testData);
            dynamic deser = JSONSupport.GetObjectFromString<CustomJSONData>(serialized);
            var origObj = deser.Data;
            Assert.IsTrue(origObj.Prop1 == "prop1");
            Assert.IsTrue(origObj.Prop2 == "prop2");
            Assert.IsTrue(origObj.Prop3 == 3);
            Assert.IsTrue(origObj.Prop4 == 4.0);
        }

    }
}