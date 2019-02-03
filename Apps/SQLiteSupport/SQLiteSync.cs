using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SQLiteSupport
{
    public interface IStorageSyncableDataContext : IDisposable
    {
        DbSet<InformationObjectMetaData> InformationObjectMetaDataTable { get; }
        void PerformUpdate(string storageRootPath, InformationObjectMetaData updateData);
        void PerformInsert(string storageRootPath, InformationObjectMetaData insertData);
        void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData);
        Task PerformUpdateAsync(string storageRootPath, InformationObjectMetaData updateData);
        Task PerformInsertAsync(string storageRootPath, InformationObjectMetaData insertData);
        Task PerformDeleteAsync(string storageRootPath, InformationObjectMetaData deleteData);
        DatabaseFacade Database { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);
    }

    public static class SQLiteSync
    {

        public static async Task<bool> ApplyStorageChangesToSQLiteDBAsync(string storageRootPath, IStorageSyncableDataContext dataContext,
    Func<string, InformationObjectMetaData[]> metadataRetrieverFunc = null)
        {
            var existingMetadatas = dataContext.InformationObjectMetaDataTable.ToArray();
            InformationObjectMetaData[] currentMetadatas;
            currentMetadatas = metadataRetrieverFunc != null ? metadataRetrieverFunc(storageRootPath) : FileSystemSync.GetMetaDatas(storageRootPath);
            bool anyChanges = await MetaDataSync.ApplyChangeActionsToExistingDataAsync(currentMetadatas, existingMetadatas,
                async insertItem => await dataContext.PerformInsertAsync(storageRootPath, insertItem),
                async updateItem => await dataContext.PerformUpdateAsync(storageRootPath, updateItem),
                async deleteItem => await dataContext.PerformDeleteAsync(storageRootPath, deleteItem));
            CancellationToken cancellationToken = new CancellationToken();
            await dataContext.SaveChangesAsync(true, cancellationToken);
            return anyChanges;
        }


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
            dataContext.SaveChanges();
            return anyChanges;
        }
    }
}
