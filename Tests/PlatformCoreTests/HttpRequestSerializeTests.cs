using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using AzureSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlatformCoreTests
{
    [TestClass]
    public class HttpRequestSerializeTests
    {
        [TestMethod]
        public void SerializeToStream()
        {
            //PrivateType httpFileCollectionType = new PrivateType(typeof(HttpFileCollection));
            //PrivateObject httpFileCollection = new PrivateObject(typeof(HttpFileCollection));
            var serData = new HttpOperationData
            {
                ExecutorAccountID = "acctid",
                OperationRequestPath = "contentpath",
                //FileCollection = (HttpFileCollection) httpFileCollection.Target,
                FileCollection = new Dictionary<string, Tuple<string, byte[]>>()
                {
                    {"filetest", new Tuple<string, byte[]>("testi1", new byte[] { 3, 4, 5, 7}) }
                },
                FormValues = new Dictionary<string, string>()
                {
                    {"formval1", "val1"},
                    {"formval2", "val2"},
                },
                OwnerRootLocation = "ownerroot",
                RequestContent = new byte[] {1, 2, 3, 4, 7}
            };
            var outputStream = new MemoryStream();
            serData.ToStream(outputStream);
            var outputData = outputStream.ToArray();
            Assert.AreEqual(103, outputData.Length);
        }

        [TestMethod]
        public void SerializeToDeserialize()
        {
            //PrivateType httpFileCollectionType = new PrivateType(typeof(HttpFileCollection));
            //PrivateObject httpFileCollection = new PrivateObject(typeof(HttpFileCollection));
            var serData = new HttpOperationData
            {
                ExecutorAccountID = "acctid",
                OperationRequestPath = "contentpath",
                //FileCollection = (HttpFileCollection) httpFileCollection.Target,
                FileCollection = new Dictionary<string, Tuple<string, byte[]>>()
                {
                    {"filetest", new Tuple<string, byte[]>("testi1", new byte[] { 3, 4, 5, 7}) }
                },
                FormValues = new Dictionary<string, string>()
                {
                    {"formval1", "val1"},
                    {"formval2", "val2"},
                },
                OwnerRootLocation = "ownerroot",
                RequestContent = new byte[] { 1, 2, 3, 4, 7 }
            };
            var outputStream = new MemoryStream();
            serData.ToStream(outputStream);
            var outputData = outputStream.ToArray();
            var inputStream = new MemoryStream(outputData);
            var outputSer = inputStream.DeserializeProtobuf<HttpOperationData>();
            Assert.AreEqual(serData.OwnerRootLocation, outputSer.OwnerRootLocation);
            Assert.AreEqual("testi1", outputSer.FileCollection["filetest"].Item1);
        }

    }
}
