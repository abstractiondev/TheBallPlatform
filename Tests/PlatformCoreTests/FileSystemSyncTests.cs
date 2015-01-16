using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLiteSupport;

namespace PlatformCoreTests
{
    [TestClass]
    public class FileSystemSyncTests
    {
        [TestMethod]
        public void GetFileSystemInfoResultsTest()
        {
            var fileEntries = FileSystemSync.GetFileInfos(TestSupport.TheBallPath);
            var allContainsRootPath = fileEntries.All(fi => fi.DirectoryName.Contains(TestSupport.TheBallPath));
            Assert.IsTrue(allContainsRootPath);
        }

        [TestMethod]
        public void GetMetaDataTest()
        {
            string pathRoot = TestSupport.TheBallPath;
            var fileEntries = FileSystemSync.GetFileInfos(pathRoot);
            var metaDatas = FileSystemSync.GetMetaDatas(pathRoot, fileEntries);
            Assert.AreEqual(4, metaDatas.Count(md => md.SemanticDomain == "TheBall.Payments"));
            Assert.AreEqual(1, metaDatas.Count(md => md.ObjectType == "CustomerAccount"));
        }

    }
}
