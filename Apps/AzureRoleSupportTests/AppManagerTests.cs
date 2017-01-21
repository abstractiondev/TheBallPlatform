using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheBall.Infra.AzureRoleSupport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBall.Infra.AzureRoleSupport.Tests
{
    [TestClass()]
    public class AppManagerTests
    {


        [TestMethod()]
        public async Task AppManagerStartStopTest()
        {
            int testCounter = 10;
            while (testCounter-- > 0)
            {
                string directory = AppDomain.CurrentDomain.BaseDirectory;
                var workerConsoleFullDir = directory.Replace("AzureRoleSupportTests", "TheBallWorkerConsole");
                var workerConsoleFullPath = Path.Combine(workerConsoleFullDir, "TheBallWorkerConsole.exe");
                AppManager workerManager = new AppManager(workerConsoleFullPath, null);
                await workerManager.StartAppConsole(true);
                var result = await workerManager.ShutdownAppConsole(true);
                Assert.AreEqual(0, result);
            }
        }
    }
}