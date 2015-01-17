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
        private readonly string PathRoot = TestSupport.TheBallPath;

        [TestMethod]
        public void GetFileSystemInfoResultsTest()
        {
            var fileEntries = FileSystemSync.GetFileInfos(PathRoot);
            var allContainsRootPath = fileEntries.All(fi => fi.DirectoryName.Contains(TestSupport.TheBallPath));
            Assert.IsTrue(allContainsRootPath);
        }

        [TestMethod]
        public void GetMetaDataTest()
        {
            var fileEntries = FileSystemSync.GetFileInfos(PathRoot);
            var metaDatas = FileSystemSync.GetMetaDatas(PathRoot, fileEntries);
            Assert.AreEqual(4, metaDatas.Count(md => md.SemanticDomain == "TheBall.Payments"));
            Assert.AreEqual(1, metaDatas.Count(md => md.ObjectType == "CustomerAccount"));
        }

    }
}
