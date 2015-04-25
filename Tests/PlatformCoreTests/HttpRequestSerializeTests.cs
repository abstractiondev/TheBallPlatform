using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity.Infrastructure;
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
            PrivateObject httpFileCollection = new PrivateObject(typeof(HttpFileCollection));
            var serData = new HttpRequestSerializer.HttpRequestPackage
            {
                AccountId = "acctid",
                ContentPath = "contentpath",
                //FileCollection = (HttpFileCollection) httpFileCollection.Target,
                FileCollection = new Dictionary<string, byte[]>()
                {
                    {"filetest", new byte[] { 3, 4, 5, 7} }
                },
                FormValues = new Dictionary<string, string>()
                {
                    {"formval1", "val1"},
                    {"formval2", "val2"},
                },
                OwnerRoot = "ownerroot",
                RequestContent = new byte[] {1, 2, 3, 4, 7}
            };
            var outputStream = new MemoryStream();
            serData.ToStream(outputStream);
            var outputData = outputStream.ToArray();
            Assert.AreEqual(93, outputData.Length);
        }

        [TestMethod]
        public void SerializeToDeserialize()
        {
            //PrivateType httpFileCollectionType = new PrivateType(typeof(HttpFileCollection));
            PrivateObject httpFileCollection = new PrivateObject(typeof(HttpFileCollection));
            var serData = new HttpRequestSerializer.HttpRequestPackage
            {
                AccountId = "acctid",
                ContentPath = "contentpath",
                //FileCollection = (HttpFileCollection) httpFileCollection.Target,
                FileCollection = new Dictionary<string, byte[]>()
                {
                    {"filetest", new byte[] { 3, 4, 5, 7} }
                },
                FormValues = new Dictionary<string, string>()
                {
                    {"formval1", "val1"},
                    {"formval2", "val2"},
                },
                OwnerRoot = "ownerroot",
                RequestContent = new byte[] { 1, 2, 3, 4, 7 }
            };
            var outputStream = new MemoryStream();
            serData.ToStream(outputStream);
            var outputData = outputStream.ToArray();
            var inputStream = new MemoryStream(outputData);
            var outputSer = inputStream.DeserializeProtobuf<HttpRequestSerializer.HttpRequestPackage>();
            Assert.AreEqual(serData.OwnerRoot, outputSer.OwnerRoot);
        }

    }
}
