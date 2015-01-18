using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using SQLite.TheBall.Payments;

namespace SQLiteSupport
{
    public interface IStorageSyncableDataContext
    {
        Table<InformationObjectMetaData> InformationObjectMetaDataTable { get; }
        void PerformUpdate(string storageRootPath, InformationObjectMetaData updateData);
        void PerformInsert(string storageRootPath, InformationObjectMetaData insertData);
        void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData);
        void SubmitChanges();
    }

    public static class SQLiteSync
    {
        public static bool ApplyStorageChangesToSQLiteDB(string storageRootPath, IStorageSyncableDataContext dataContext)
        {
            var existingMetadatas = dataContext.InformationObjectMetaDataTable.ToArray();
            var currentMetadatas = FileSystemSync.GetMetaDatas(storageRootPath);
            bool anyChanges = MetaDataSync.ApplyChangeActionsToExistingData(currentMetadatas, existingMetadatas,
                insertItem => dataContext.PerformInsert(storageRootPath, insertItem),
                updateItem => dataContext.PerformUpdate(storageRootPath, updateItem),
                deleteItem => dataContext.PerformDelete(storageRootPath, deleteItem));
            dataContext.SubmitChanges();
            return anyChanges;
        }
    }
}
