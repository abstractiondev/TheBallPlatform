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
                bool isLockEntry = fileName.EndsWith(OperationSupport.LockExtension) || fileName.EndsWith(OperationSupport.DedicatedLockExtension);
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

        public static async Task<LockInterfaceOperationsByOwner.AcquireFirstObtainableLockReturnValue> ExecuteMethod_AcquireFirstObtainableLockAsync(IContainerOwner dedicatedToOwner, IEnumerable<IGrouping<string, string>> ownerGroupedItems, IContainerOwner queueOwner, string queueLocation, string lockFileNameFormat)
        {
            var fullLockPathFormat =
                Path.Combine(queueOwner.GetOwnerPrefix(), queueLocation, lockFileNameFormat).Replace(@"\", "/");
            string currLockFile = null;
            bool isDedicated = dedicatedToOwner != null;
            string dedicatedToOwnerPrefix = isDedicated 
                ? dedicatedToOwner.ContainerName + "_" + dedicatedToOwner.LocationPrefix : null;

            var groupsOfInterest = getGroupsOfInterest(isDedicated, ownerGroupedItems, dedicatedToOwnerPrefix);

            Func<string, bool> lockPredicate = item => item.EndsWith(OperationSupport.LockExtension);
            Func<string, bool> dedicatedLockPredicate = item => item.EndsWith(OperationSupport.DedicatedLockExtension);

            foreach (var grp in groupsOfInterest)
            {
                var ownerprefix_id = grp.Key;
                var allGroupItems = grp.ToArray();
                bool hasOperationLock = allGroupItems.Any(lockPredicate);
                if (hasOperationLock)
                    continue;
                bool hasDedicatedLock = allGroupItems.Any(dedicatedLockPredicate);
                var operationItems =
                    allGroupItems.Where(item => lockPredicate(item) == false && dedicatedLockPredicate(item) == false)
                        .ToArray();
                // If has dedicated lock and its not our dedicated lock
                if (hasDedicatedLock && ownerprefix_id != dedicatedToOwnerPrefix)
                {
                    continue;
                }
                if (isDedicated && !hasDedicatedLock) // Fabricated key only, no dedicated lock yet
                {
                    // Attempt to acquire dedicated lock and return afterwards (regardless)
                    var dedicatedLockFile = String.Format(fullLockPathFormat, ownerprefix_id,
                        OperationSupport.DedicatedLockExtension);
                    var dedicatedAcquired =
                        await StorageSupport.AcquireLogicalLockByCreatingBlobAsync(dedicatedLockFile);
                    return null;
                }
                // If there are no other than dedicated lock file, do nothing
                if (operationItems.Length == 0)
                    continue;
                currLockFile = String.Format(fullLockPathFormat, ownerprefix_id, OperationSupport.LockExtension);
                var lockEtag = await StorageSupport.AcquireLogicalLockByCreatingBlobAsync(currLockFile);
                if (lockEtag == null)
                    continue;
                var ownerOperationBlobNames = operationItems;
                var ownerOperationIDs = ownerOperationBlobNames.Select(blobName =>
                {
                    var fileName = Path.GetFileName(blobName);
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
                    OperationQueueItems = allGroupItems,
                    LockBlobFullPath = currLockFile
                };
                var blobContents = String.Join(Environment.NewLine, result.OperationIDs);
                var lockBlob = StorageSupport.CurrActiveContainer.GetBlockBlobReference(currLockFile);
                await lockBlob.UploadBlobTextAsync(blobContents, false);
                return result;
            }
            // If there was no locks
            return null;
        }

        private static IEnumerable<IGrouping<string, string>> getGroupsOfInterest(bool isDedicated, IEnumerable<IGrouping<string, string>> ownerGroupedItems,
            string dedicatedToOwnerPrefix)
        {
            var groupsOfInterest = isDedicated
                ? ownerGroupedItems.Where(grp => grp.Key == dedicatedToOwnerPrefix)
                : ownerGroupedItems;
            if (isDedicated)
            {
                var workArray = groupsOfInterest.ToArray();
                if (workArray.Length == 0)
                {
                    var fabricatedGroup = new[] {dedicatedToOwnerPrefix}.GroupBy(item => item);
                    workArray = fabricatedGroup.ToArray();
                }
                groupsOfInterest = workArray;
            }
            return groupsOfInterest;
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
                OperationQueueItems = acquireFirstObtainableLockOutput.OperationQueueItems,
                LockBlobFullPath = acquireFirstObtainableLockOutput.LockBlobFullPath
            };
        }
    }
}