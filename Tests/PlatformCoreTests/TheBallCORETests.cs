using System;
using AzureSupport.TheBall.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheBall.Core;

namespace PlatformCoreTests
{
    [TestClass]
    public class TheBallCORETests
    {
        [TestMethod]
        public void ExecuteForObjectTest()
        {
            string objectFullAddress =
                "grp/8bc520cf-6c16-41d8-86b0-0726e8782683/AaltoGlobalImpact.OIP/TextContent/a5366f8e-cafa-416f-bf0b-326011e5b1d3";
            objectFullAddress.ExecuteForObject((owner, domain, type, id, path) =>
            {
                var refOwner = VirtualOwner.FigureOwner(objectFullAddress);
                var refDomain = "AaltoGlobalImpact.OIP";
                var refType = "TextContent";
                var refID = "a5366f8e-cafa-416f-bf0b-326011e5b1d3";

                Assert.IsTrue(refOwner.IsSameOwner(owner));
                Assert.AreEqual(domain, refDomain);
                Assert.AreEqual(refType, type);
                Assert.AreEqual(refID, id);
            });
        }
    }
}
