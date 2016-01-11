using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class LockInterfaceOperationsByOwnerImplementation
    {
        public static IContainerOwner GetTarget_QueueOwner()
        {
            return SystemSupport.SystemOwner;
        }

        public static string GetTarget_QueueLocation()
        {
            return OperationSupport.OperationQueueLocationName;
        }

        public static async Task<IEnumerable<IGrouping<string, string>>> GetTarget_OwnerGroupedItemsAsync(IContainerOwner queueOwner, string queueLocation)
        {
            var blobSegment = await queueOwner.ListBlobsWithPrefixAsync(queueLocation);
            var blobList = blobSegment.Results.Cast<CloudBlockBlob>();
            var grouped = blobList.GroupBy(blob =>
            {
                var fileName = Path.GetFileName(blob.Name);
                string ownerPrefix;
                string ownerID;
                bool isLockEntry = fileName.EndsWith(".lock");
                if (isLockEntry)
                    OperationSupport.GetLockItemComponents(fileName, out ownerPrefix, out ownerID);
                else
                {
                    string timeStampPart;
                    string operationID;
                    OperationSupport.GetQueueItemComponents(fileName, out timeStampPart, out ownerPrefix, out ownerID,
                        out operationID);
                }
                return ownerPrefix + "_" + ownerID;
            }, blob => blob.Name);
            return grouped;
        }

        public static string GetTarget_LockFileNameFormat()
        {
            return OperationSupport.LockFileNameFormat;
        }

        public static async Task<LockInterfaceOperationsByOwner.AcquireFirstObtainableLockReturnValue> ExecuteMethod_AcquireFirstObtainableLockAsync(IEnumerable<IGrouping<string, string>> ownerGroupedItems, IContainerOwner queueOwner, string queueLocation, string lockFileNameFormat)
        {
            var fullLockPathFormat =
                Path.Combine(queueOwner.GetOwnerPrefix(), queueLocation, lockFileNameFormat).Replace(@"\", "/");
            string currLockFile = null;
            foreach (var grp in ownerGroupedItems)
            {
                var allGroupItems = grp.ToArray();
                if (allGroupItems.Any(item => item.EndsWith(".lock")))
                    continue;
                var ownerprefix_id = grp.Key;
                currLockFile = String.Format(fullLockPathFormat, ownerprefix_id);
                bool acquireLock = await StorageSupport.AcquireLogicalLockByCreatingBlobAsync(currLockFile);
                if (!acquireLock)
                    continue;
                var ownerOperationBlobNames = allGroupItems;
                var ownerOperationIDs = ownerOperationBlobNames.Select(blobName =>
                {
                    var fileName = Path.GetFileName(ownerOperationBlobNames.First());
                    string timestampPart;
                    string ownerPrefix;
                    string ownerID;
                    string operationID;
                    OperationSupport.GetQueueItemComponents(fileName, out timestampPart, out ownerPrefix,
                        out ownerID, out operationID);
                    return new Tuple<string, string, string>(ownerPrefix, ownerID, operationID);
                }).ToArray();
                var result = new LockInterfaceOperationsByOwner.AcquireFirstObtainableLockReturnValue
                {
                    LockedOwnerPrefix = ownerOperationIDs.First().Item1,
                    LockedOwnerID = ownerOperationIDs.First().Item2,
                    OperationIDs = ownerOperationIDs.Select(item => item.Item3).ToArray(),
                    LockBlobFullPath = currLockFile
                };
                var blobContents = String.Join(Environment.NewLine, result.OperationIDs);
                var lockBlob = StorageSupport.CurrActiveContainer.GetBlockBlobReference(currLockFile);
                await lockBlob.UploadBlobTextAsync(blobContents, false);
                return result;
            }
            return null;
        }

        public static LockInterfaceOperationsByOwnerReturnValue Get_ReturnValue(LockInterfaceOperationsByOwner.AcquireFirstObtainableLockReturnValue acquireFirstObtainableLockOutput)
        {
            if (acquireFirstObtainableLockOutput == null)
                return null;
            return new LockInterfaceOperationsByOwnerReturnValue
            {
                LockedOwnerPrefix = acquireFirstObtainableLockOutput.LockedOwnerPrefix,
                LockedOwnerID = acquireFirstObtainableLockOutput.LockedOwnerID,
                OperationIDs = acquireFirstObtainableLockOutput.OperationIDs,
                LockBlobFullPath = acquireFirstObtainableLockOutput.LockBlobFullPath
            };
        }
    }
}