using System;
using System.Threading.Tasks;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class ExecuteInterfaceOperationsByOwnerAndReleaseLockImplementation
    {
        public static void ExecuteMethod_ExecuteOperationsAndReleaseLock(string lockedOwnerPrefix, string lockedOwnerID, string[] operationIDs, string lockBlobFullPath)
        {
            var executionOwner = new VirtualOwner(lockedOwnerPrefix,
                lockedOwnerID);
            InformationContext.Current.Owner = executionOwner;
            foreach (var operationID in operationIDs)
            {
                try
                {
                    InformationContext.InitializeToLogicalContext();
                    InformationContext.Current.InitializeCloudStorageAccess(StorageSupport.CurrActiveContainer.Name);
                    // TODO: execute operation as operation owner
                }
                catch (Exception exception)
                {
                    // mark operation as error and continue
                }
                finally
                {
                    InformationContext.RemoveFromLogicalContext();
                }
            }

            var lockFullName = lockBlobFullPath;
            StorageSupport.ReleaseLogicalLockByDeletingBlob(lockFullName, null);
        }

        public static async Task ExecuteMethod_ExecuteOperationsAndReleaseLockAsync(string lockedOwnerPrefix, string lockedOwnerID, string[] operationIDs, string lockBlobFullPath)
        {
            var executionOwner = new VirtualOwner(lockedOwnerPrefix,
                lockedOwnerID);
            InformationContext.Current.Owner = executionOwner;
            foreach (var operationID in operationIDs)
            {
                try
                {
                    InformationContext.InitializeToLogicalContext();
                    InformationContext.Current.InitializeCloudStorageAccess(StorageSupport.CurrActiveContainer.Name);
                    // TODO: execute operation as operation owner
                }
                catch (Exception exception)
                {
                    // mark operation as error and continue
                }
                finally
                {
                    InformationContext.RemoveFromLogicalContext();
                }
            }

            var lockFullName = lockBlobFullPath;
            StorageSupport.ReleaseLogicalLockByDeletingBlob(lockFullName, null);
        }
    }
}