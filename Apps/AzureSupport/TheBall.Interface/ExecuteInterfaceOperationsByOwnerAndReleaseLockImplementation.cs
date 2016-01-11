using System;
using System.Threading.Tasks;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;

namespace TheBall.Interface
{
    public class ExecuteInterfaceOperationsByOwnerAndReleaseLockImplementation
    {
        public static void ExecuteMethod_ExecuteOperationsAndReleaseLock(string instanceName, string lockedOwnerPrefix, string lockedOwnerID, string[] operationIDs, string lockBlobFullPath)
        {
            var executionOwner = new VirtualOwner(lockedOwnerPrefix,
                lockedOwnerID);
            try
            {
                InformationContext.InitializeToLogicalContext(executionOwner, instanceName);
                foreach (var operationID in operationIDs)
                {
                    try
                    {

                    }
                    catch (Exception exception)
                    {
                        // mark operation as error and continue
                    }

                }
                // TODO: execute operation as operation owner
            }
            finally
            {
                InformationContext.RemoveFromLogicalContext();
            }
            var lockFullName = lockBlobFullPath;
            StorageSupport.ReleaseLogicalLockByDeletingBlob(lockFullName, null);
        }

        public static async Task ExecuteMethod_ExecuteOperationsAndReleaseLockAsync(string instanceName, string lockedOwnerPrefix, string lockedOwnerID, string[] operationIDs, string lockBlobFullPath)
        {
            var executionOwner = new VirtualOwner(lockedOwnerPrefix,
                lockedOwnerID);
            try
            {
                InformationContext.InitializeToLogicalContext(executionOwner, instanceName);
                foreach (var operationID in operationIDs)
                {
                    try
                    {
                        await
                            ExecuteInterfaceOperation.ExecuteAsync(new ExecuteInterfaceOperationParameters
                            {
                                OperationID = operationID
                            });
                    }
                    catch (Exception exception)
                    {
                        // mark operation as error and continue
                    }

                }
                await InformationContext.ExecuteAsOwnerAsync(SystemOwner.CurrentSystem, async () =>
                {
                    var lockFullName = lockBlobFullPath;
                    await StorageSupport.ReleaseLogicalLockByDeletingBlobAsync(lockFullName);
                });
            }
            finally
            {
                InformationContext.ProcessAndClearCurrentIfAvailable();
            }
        }
    }
}