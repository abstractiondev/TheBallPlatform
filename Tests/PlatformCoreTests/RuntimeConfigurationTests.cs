using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using SER.TheBall.Payments;
using TheBall.Core.InstanceSupport;

namespace PlatformCoreTests
{
    [TestClass]
    public class RuntimeConfigurationTests
    {
        [TestMethod]
        public async Task RuntimeConfigurationTest()
        {
            var infraConfigFullPath = Path.Combine(TestSupport.TestDataPath, @"RuntimeConfigs\InfraShared\InfraConfig.json" );
            await RuntimeConfiguration.InitializeRuntimeConfigs(infraConfigFullPath);
        }

    }
}
