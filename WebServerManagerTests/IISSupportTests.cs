using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheBall.Infra.WebServerManager;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;

namespace TheBall.Infra.WebServerManager.Tests
{
    [TestClass()]
    public class IISSupportTests
    {
        private const string TBUnitTestSite = "TBUnitTestSite";
        private const string TBUnitTestTempSite = "TBUnitTestTempSite";

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            ServerManager iisManager = new ServerManager();
            var unitTestSite = iisManager.Sites[TBUnitTestSite];
            if (unitTestSite != null)
            {
                unitTestSite.Delete();
                iisManager.CommitChanges();
            }
            unitTestSite = iisManager.Sites.Add(TBUnitTestSite, null, 80);
            iisManager.CommitChanges();
        }


        [TestMethod()]
        public void CreateOrRetrieveCCSWebSiteTest()
        {
            //Assert.Fail();
        }

        [TestMethod]
        public void CreateSiteWithInitialBindingsTest()
        {
            IISSupport.CreateIISApplicationSiteIfMissing(TBUnitTestTempSite, @"T:\TBTest");
            ServerManager iisManager = new ServerManager();
            var newSite = iisManager.Sites[TBUnitTestTempSite];
            Assert.IsNotNull(newSite);
        }

        [TestMethod]
        public void UpdateInstanceBindingsTest()
        {
            var bindingData = TBUnitTestSite + ":test1.theball.me,test2.theball.me";
            IISSupport.UpdateInstanceBindings(bindingData, TBUnitTestSite);
        }

        [TestMethod()]
        public void SetHostHeadersTest()
        {
            IISSupport.EnsureHttpHostHeaders(TBUnitTestSite, new string[] { "host1", "host2"});
        }

        [TestMethod()]
        public void UpdateExistingSiteTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void FindDomainCertTest()
        {
            var hash = IISSupport.GetCertHash("*.izenzei.com");
            Assert.IsNotNull(hash);
        }


        [TestMethod()]
        public void UpdateSiteWithDeployTest()
        {
            //Assert.Fail();
        }
    }
}