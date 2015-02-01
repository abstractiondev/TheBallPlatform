using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using System.Text;
using SQLite.TheBall.Payments;

namespace SQLiteSupport
{
    public interface IStorageSyncableDataContext : IDisposable
    {
        Table<InformationObjectMetaData> InformationObjectMetaDataTable { get; }
        void PerformUpdate(string storageRootPath, InformationObjectMetaData updateData);
        void PerformInsert(string storageRootPath, InformationObjectMetaData insertData);
        void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData);
        DbConnection Connection { get; }
        void SubmitChanges();
    }

    public static class SQLiteSync
    {
        public static bool ApplyStorageChangesToSQLiteDB(string storageRootPath, IStorageSyncableDataContext dataContext,
            Func<string, InformationObjectMetaData[]> metadataRetrieverFunc = null)
        {
            var existingMetadatas = dataContext.InformationObjectMetaDataTable.ToArray();
            InformationObjectMetaData[] currentMetadatas;
            currentMetadatas = metadataRetrieverFunc != null ? metadataRetrieverFunc(storageRootPath) : FileSystemSync.GetMetaDatas(storageRootPath);
            bool anyChanges = MetaDataSync.ApplyChangeActionsToExistingData(currentMetadatas, existingMetadatas,
                insertItem => dataContext.PerformInsert(storageRootPath, insertItem),
                updateItem => dataContext.PerformUpdate(storageRootPath, updateItem),
                deleteItem => dataContext.PerformDelete(storageRootPath, deleteItem));
            dataContext.SubmitChanges();
            return anyChanges;
        }
    }
}
