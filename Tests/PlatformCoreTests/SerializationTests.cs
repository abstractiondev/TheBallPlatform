using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheBall.Payments;

namespace PlatformCoreTests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void CustomerAccountCollectionDeserializationTest()
        {
            var customerAccountCollection = CustomerAccountCollection.DeserializeFromXml(
                File.ReadAllText(
                    TestSupport.GetTheBallFileFullPath("TheBall.Payments/CustomerAccountCollection/MasterCollection")));
            Assert.IsNotNull(customerAccountCollection);
        }
    }
}
