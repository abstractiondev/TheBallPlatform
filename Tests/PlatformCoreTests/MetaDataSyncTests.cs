using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using SQLite.TheBall.Payments;
using SQLiteSupport;
using ChangeAction = SQLiteSupport.ChangeAction;

namespace PlatformCoreTests
{
    [TestClass]
    public class MetaDataSyncTests
    {
        private readonly string PathRoot = TestSupport.TheBallPath;

        [TestMethod]
        public void VerifyCurrentMetaDataCountTest()
        {
            var fileEntries = FileSystemSync.GetFileInfos(PathRoot);
            var metaDatas = FileSystemSync.GetMetaDatas(PathRoot, fileEntries);
            var datasToAdd = MetaDataSync.UpdatePendingChangeActionsToExistingData(metaDatas, new InformationObjectMetaData[0]);
            Assert.AreEqual(4, datasToAdd.Length);
        }

        [TestMethod]
        public void GetCurrentFileSystemSyncsAsInsertsTest()
        {
            var fileEntries = FileSystemSync.GetFileInfos(PathRoot);
            var metaDatas = FileSystemSync.GetMetaDatas(PathRoot, fileEntries);
            var datasToAdd = MetaDataSync.UpdatePendingChangeActionsToExistingData(metaDatas, new InformationObjectMetaData[0]);
            Assert.IsTrue(metaDatas.SequenceEqual(datasToAdd));
        }

        [TestMethod]
        public void UnchangedMetaDataTest()
        {
            var fileEntries = FileSystemSync.GetFileInfos(PathRoot);
            var metaDatas = FileSystemSync.GetMetaDatas(PathRoot, fileEntries);
            var datasToAdd = MetaDataSync.UpdatePendingChangeActionsToExistingData(metaDatas, new InformationObjectMetaData[0]);
            var newDatasToAdd = MetaDataSync.UpdatePendingChangeActionsToExistingData(metaDatas, metaDatas);
            Assert.AreEqual(0, newDatasToAdd.Length);
            Assert.IsTrue(metaDatas.All(metaData => metaData.CurrentChangeAction == ChangeAction.None));
        }

    }
}