using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using AzureSupport;
using TheBall.Core;
using TheBall.Core.Storage;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class PullSyncDataImplementation
    {
        public static IContainerOwner GetTarget_CollaborationSource(CollaborationPartner partner)
        {
            return new VirtualOwner(partner.PartnerType, partner.PartnerID, true);
        }

        public static string GetTarget_SyncSourceRoot(IContainerOwner collaborationTarget)
        {
            var sourceRoot = BlobStorage.CombinePath("TheBall.Interface", "ShareInfo", collaborationTarget.GetOwnerPrefix());
            return sourceRoot;
        }

        public static string GetTarget_SyncTargetRoot(IContainerOwner collaborationSource)
        {
            var sourceRoot = BlobStorage.CombinePath("TheBall.Interface", "ShareInfo", collaborationSource.GetOwnerPrefix());
            return sourceRoot;
        }

        public static IContainerOwner GetTarget_CollaborationTarget()
        {
            return InformationContext.CurrentOwner;
        }

        public static async Task<BlobStorageItem[]> GetTarget_ExistingSourceItemsAsync(IContainerOwner collaborationSource, string syncSourceRoot)
        {
            var allBlobs = await BlobStorage.GetBlobItemsA(collaborationSource, syncSourceRoot);
            var metadataBlobs = allBlobs.Where(blob => isMetadata(blob.FileName)).ToArray();
            var realBlobTasks =
                metadataBlobs.Select(metadataItem => getRealItemFromShareMetadata(collaborationSource, metadataItem))
                    .ToArray();
            await Task.WhenAll(realBlobTasks);
            var contentBlobs = realBlobTasks.Select(task => task.Result).Where(blob => blob != null).ToArray();
            return contentBlobs;
        }

        private static async Task<BlobStorageItem> getRealItemFromShareMetadata(IContainerOwner owner, BlobStorageItem metadataItem)
        {
            var metadataContent = await BlobStorage.GetBlobContentFromOtherOwnerA(owner, metadataItem.Name);
            var shareInfo = JSONSupport.GetObjectFromData<ShareInfo>(metadataContent);
            var interfaceDataItemPath = BlobStorage.CombinePath("TheBall.Interface", "InterfaceData", shareInfo.ItemName);
            var blobItem = await BlobStorage.GetBlobStorageItemA(interfaceDataItemPath, owner);
            if (blobItem?.ContentMD5 != shareInfo.ContentMD5)
                return null;
            return blobItem;
        }

        private static bool isMetadata(string fileName)
        {
            return fileName.StartsWith("_") && fileName.EndsWith(".json");
        }

        public static async Task<BlobStorageItem[]> GetTarget_ExistingTargetItemsAsync(IContainerOwner collaborationTarget, string syncTargetRoot)
        {
            var blobs = await BlobStorage.GetBlobItemsA(collaborationTarget, syncTargetRoot);
            var nonMetadataBlobs = blobs.Where(blob => !isMetadata(blob.Name)).ToArray();
            return nonMetadataBlobs;
        }

        public static async Task ExecuteMethod_SyncItemsAsync(IContainerOwner collaborationSource, string syncSourceRoot, BlobStorageItem[] existingSourceItems, IContainerOwner collaborationTarget, string syncTargetRoot, BlobStorageItem[] existingTargetItems)
        {
            var sourcePrefixToReplace = BlobStorage.CombinePath(collaborationSource.GetOwnerPrefix(), "TheBall.Interface", "InterfaceData");
            var targetPrefixToReplaceTo = BlobStorage.CombinePath(collaborationTarget.GetOwnerPrefix(),
                "TheBall.Interface", "ShareInfo", collaborationSource.GetOwnerPrefix());

            var sourceToTargetItems = existingSourceItems.Select(sourceItem =>
            {
                var sourceItemName = sourceItem.Name;
                if (!sourceItemName.StartsWith(sourcePrefixToReplace))
                    throw new SecurityException("Source item not starting with proper prefix: " + sourceItemName);
                var targetItemName = sourceItem.Name.Replace(sourcePrefixToReplace, targetPrefixToReplaceTo);
                if(!targetItemName.StartsWith(targetPrefixToReplaceTo))
                    throw new SecurityException("Target item not starting with proper prefix: " + targetItemName);
                var targetItem = new BlobStorageItem(targetItemName, sourceItem.ContentMD5, sourceItem.ETag, sourceItem.Length, sourceItem.LastModified);
                return new { SourceItem = sourceItem, TargetItem = targetItem};
            }).OrderBy(item => item.TargetItem.Name).ToArray();

            var targetItems = existingTargetItems.OrderBy(item => item.Name).ToArray();

            int sourceIX = 0;
            var sourceItemCount = sourceToTargetItems.Length;
            int targetIX = 0;
            var targetItemCount = targetItems.Length;
            List<Task> processTasks = new List<Task>();
            while (sourceIX < sourceItemCount || targetIX < targetItemCount)
            {
                var sourceItem = sourceIX < sourceItemCount ? sourceToTargetItems[sourceIX] : null;
                var targetItem = targetIX < targetItemCount ? targetItems[targetIX] : null;
                var isNewItem = sourceItem != null &&
                                (targetItem == null || String.CompareOrdinal(sourceItem.TargetItem.Name, targetItem.Name) < 0);
                bool isDeletedItem = targetItem != null &&
                                     (sourceItem == null ||
                                      String.CompareOrdinal(sourceItem.TargetItem.Name, targetItem.Name) > 0);
                bool isSameItem = sourceItem?.TargetItem?.Name == targetItem?.Name;
                bool isSameWithDifferentContent = isSameItem &&
                                                  (sourceItem.SourceItem.ContentMD5 != targetItem.ContentMD5);
                bool doCopy = isNewItem || isSameWithDifferentContent;
                bool doDelete = isDeletedItem;
                if (doCopy)
                {
                    var copyTask = BlobStorage.CopyBlobBetweenOwnersA(collaborationSource, sourceItem.SourceItem.Name, collaborationTarget,
                            sourceItem.TargetItem.Name);
                    processTasks.Add(copyTask);
                }
                else if (doDelete)
                {
                    var deleteTask = BlobStorage.DeleteBlobA(targetItem.Name);
                    processTasks.Add(deleteTask);
                }
                if (isNewItem)
                    sourceIX++;
                else if (isDeletedItem)
                    targetIX++;
                else // same item
                {
                    sourceIX++;
                    targetIX++;
                }
            }
            await Task.WhenAll(processTasks);
        }

        public static UpdateSharedDataSummaryDataParameters UpdateSummaryData_GetParameters(CollaborationPartner partner)
        {
            return new UpdateSharedDataSummaryDataParameters {Partner = partner};
        }
    }
}