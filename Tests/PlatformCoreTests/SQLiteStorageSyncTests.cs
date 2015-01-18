using System.Data.Linq;
using System.Data.SQLite;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.TheBall.Payments;
using SQLiteSupport;

namespace PlatformCoreTests
{
    [TestClass]
    public class SQLiteStorageSyncTests
    {
        private readonly string PathRoot = TestSupport.TheBallPath;

        private SQLite.TheBall.Payments.TheBallDataContext CurrentContext;

        [TestInitialize]
        public void SetupForTest()
        {
            CurrentContext = new TheBallDataContext(new SQLiteConnection("Data Source=:memory:"));
            CurrentContext.CreateDomainDatabaseTablesIfNotExists();
        }

        [TestCleanup]
        public void TearDownForTest()
        {
            CurrentContext = null;
        }

        [TestMethod]
        public void VerifyCurrentMetaDataCountTest()
        {
            Assert.AreEqual(0, CurrentContext.CustomerAccountTable.Count());
        }

        [TestMethod]
        public void GetCurrentFileSystemSyncsAsInsertsTest()
        {
            bool changesWereApplied = SQLiteSync.ApplyStorageChangesToSQLiteDB(PathRoot, CurrentContext);
            Assert.IsTrue(changesWereApplied);
            Assert.AreEqual(1, CurrentContext.CustomerAccountTable.Count());
            Assert.AreEqual(0, CurrentContext.GroupSubscriptionPlanTable.Count());
        }

        [TestMethod]
        public void UnchangedMetaDataTest()
        {
            bool changesWereApplied = SQLiteSync.ApplyStorageChangesToSQLiteDB(PathRoot, CurrentContext);
            changesWereApplied = SQLiteSync.ApplyStorageChangesToSQLiteDB(PathRoot, CurrentContext);
            Assert.IsFalse(changesWereApplied);
            Assert.AreEqual(1, CurrentContext.CustomerAccountTable.Count());
            Assert.AreEqual(0, CurrentContext.GroupSubscriptionPlanTable.Count());
            Assert.AreEqual(4, CurrentContext.InformationObjectMetaDataTable.Count());
        }

    }
}