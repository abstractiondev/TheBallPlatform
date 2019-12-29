using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AzureSupport;
using TheBall.Core.StorageCore;

namespace TheBall.Core
{
    public class ExecuteProcessImplementation
    {
        static IContainerOwner Owner { get { return InformationContext.CurrentOwner; }}

        public static async Task<Process> GetTarget_ProcessAsync(string processId)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<Process>(Owner, processId);
        }

        public static string GetTarget_ProcessLockLocation(Process process)
        {
            return process.RelativeLocation + ".lock";
        }

        public static async Task ExecuteMethod_ExecuteAndStoreProcessWithLockAsync(string processLockLocation, Process process)
        {
            var storageService = CoreServices.GetCurrent<IStorageService>();
            string lockEtag = await storageService.AcquireLogicalLockByCreatingBlobAsync(processLockLocation);
            if (lockEtag == null)
                return;
            try
            {
                string operationTypeName = process.ExecutingOperation.ItemFullType;
                OperationSupport.ExecuteOperation(operationTypeName, new Tuple<string, object>("Process", process));
                await process.StoreInformationAsync();
            }
            finally
            {
                await storageService.ReleaseLogicalLockByDeletingBlobAsync(processLockLocation, lockEtag);
            }
        }
    }
}