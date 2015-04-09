using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.AaltoGlobalImpact.OIP;
using SQLiteSupport;

namespace PlatformCoreTests
{
    [TestClass]
    public class SQLiteStorageSyncTests_OIP
    {
        private readonly string PathRoot = TestSupport.OIPPath;

        private TheBallDataContext CurrentContext;
        private string CurrentDBFileName;

        [TestInitialize]
        public void SetupForTest()
        {
            //CurrentDBFileName = Path.GetTempFileName();
            //CurrentDBFileName = @"d:\tmp\testdata.sqlite";
            CurrentDBFileName = ":memory:";
            CurrentContext = TheBallDataContext.CreateOrAttachToExistingDB(CurrentDBFileName);
            CurrentContext.CreateDomainDatabaseTablesIfNotExists();
        }

        [TestCleanup]
        public void TearDownForTest()
        {
            CurrentContext.Dispose();
            CurrentContext = null;
            if(CurrentDBFileName != ":memory:")
                File.Delete(CurrentDBFileName);
        }

        [TestMethod]
        public void VerifyCurrentMetaDataCountTest()
        {
            Assert.AreEqual(0, CurrentContext.TextContentTable.Count());
        }

        [TestMethod]
        public void GetCurrentFileSystemSyncsAsInsertsTest()
        {
            bool changesWereApplied = SQLiteSync.ApplyStorageChangesToSQLiteDB(PathRoot, CurrentContext);
            Assert.IsTrue(changesWereApplied);
            Assert.AreEqual(6, CurrentContext.TextContentTable.Count());
            Assert.AreEqual(1, CurrentContext.EmbeddedContentTable.Count());
        }

        [TestMethod]
        public void UnchangedMetaDataTest()
        {
            bool changesWereApplied = SQLiteSync.ApplyStorageChangesToSQLiteDB(PathRoot, CurrentContext);
            changesWereApplied = SQLiteSync.ApplyStorageChangesToSQLiteDB(PathRoot, CurrentContext);
            Assert.IsFalse(changesWereApplied);
            Assert.AreEqual(6, CurrentContext.TextContentTable.Count());
            Assert.AreEqual(1, CurrentContext.EmbeddedContentTable.Count());
            Assert.AreEqual(33, CurrentContext.InformationObjectMetaDataTable.Count());
        }

        [TestMethod]
        public void RemovingMetaDataTest()
        {
            bool changesWereApplied = SQLiteSync.ApplyStorageChangesToSQLiteDB(PathRoot, CurrentContext);
            var currentMetadata = CurrentContext.InformationObjectMetaDataTable.ToArray();
            changesWereApplied = SQLiteSync.ApplyStorageChangesToSQLiteDB(PathRoot, CurrentContext, s => new InformationObjectMetaData[0]);
            Assert.IsTrue(changesWereApplied);
            Assert.AreEqual(0, CurrentContext.InformationObjectMetaDataTable.Count());
        }

        [TestMethod]
        public void AlteringMetaDataTest()
        {
            bool changesWereApplied = SQLiteSync.ApplyStorageChangesToSQLiteDB(PathRoot, CurrentContext);
            var currentMetadata = CurrentContext.InformationObjectMetaDataTable.ToArray();
            changesWereApplied = SQLiteSync.ApplyStorageChangesToSQLiteDB(PathRoot, CurrentContext, s =>
            {
                List<InformationObjectMetaData> newMetaDatas = new List<InformationObjectMetaData>();
                foreach (var data in currentMetadata)
                {
                    var newMetaData = new InformationObjectMetaData
                    {
                        CurrentStoragePath = data.CurrentStoragePath,
                        FileLength = data.FileLength + 1,
                        LastWriteTime = data.LastWriteTime,
                        MD5 = "",
                        ObjectID = data.ObjectID,
                        ObjectType = data.ObjectType,
                        SemanticDomain = data.SemanticDomain,
                        SerializationType = data.SerializationType
                    };
                    newMetaDatas.Add(newMetaData);
                }
                return newMetaDatas.ToArray();
            });
            Assert.IsTrue(changesWereApplied);
            Assert.AreEqual(33, CurrentContext.InformationObjectMetaDataTable.Count());
        }

    }
}