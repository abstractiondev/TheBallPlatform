using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheBall.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBall.Infrastructure.Tests
{
    [TestClass()]
    public class UpdateInfraDataInterfaceObjectsImplementationTests
    {
        [TestMethod()]
        public void ParseUpdateConfigItemPartsTest()
        {
            var uriPath = "any/paths/deploy/20170119.1931_dev_1512d76423f017a42638b13034b02b4c9b9898f7";
            var updateConfigItem = UpdateInfraDataInterfaceObjectsImplementation.ParseUpdateConfigItemParts(uriPath);
            Assert.AreEqual("20170119.1931", updateConfigItem.BuildNumber);
            Assert.AreEqual("dev", updateConfigItem.MaturityLevel);
            Assert.AreEqual("1512d76423f017a42638b13034b02b4c9b9898f7", updateConfigItem.Commit);
        }
    }
}