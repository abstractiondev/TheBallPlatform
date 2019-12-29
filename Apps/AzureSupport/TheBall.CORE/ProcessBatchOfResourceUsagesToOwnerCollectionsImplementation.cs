using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.Core.Storage;
using TheBall.Core.StorageCore;

namespace TheBall.Core
{
    public class ProcessBatchOfResourceUsagesToOwnerCollectionsImplementation
    {
        private const string LockLocation = SystemSupport.SystemOwnerRoot + "/TheBall.Core/RequestResourceUsage/0.lock";
        public static async Task<CloudBlockBlob[]> GetTarget_BatchToProcessAsync(int processBatchSize, bool processIfLess)
        {
            //options.
            //options.AccessCondition 
            //StorageSupport.CurrActiveContainer.ListBlobsSegmented()
            //    ListBlobsWithPrefix("sys/AAA/TheBall.Core/RequestResourceUsage")

            throw new NotSupportedException();
            /*
            string prefix = "sys/AAA/TheBall.Core/RequestResourceUsage/";

            var blobResultSegment = await StorageSupport.ListBlobsWithPrefixAsync(null, prefix);
            var blobList = blobResultSegment.Results.Take(processBatchSize).Cast<CloudBlockBlob>().ToArray();
            List<CloudBlockBlob> result = new List<CloudBlockBlob>();
            foreach (var blob in blobList)
            {
                if (blob.Name == LockLocation)
                    return null;
                result.Add(blob);
            }
            if (result.Count < processBatchSize && processIfLess == false)
                return null;
            // Acquire Lock
            string lockETag = await StorageSupport.AcquireLogicalLockByCreatingBlobAsync(LockLocation);
            if (lockETag == null)
                return null;
            return result.ToArray();
            */
        }

        public static async Task ExecuteMethod_ProcessBatchAsync(CloudBlockBlob[] batchToProcess)
        {
            if (batchToProcess == null)
                return;
            Dictionary<string, List<RequestResourceUsage>> ownerGroupedUsages = new Dictionary<string, List<RequestResourceUsage>>();
            Type type = typeof (RequestResourceUsage);
            int i = 0;
            foreach (var blob in batchToProcess)
            {
                i++;
                Debug.WriteLine("Reading resource " + i + ": " + blob.Name);
                RequestResourceUsage resourceUsage = (RequestResourceUsage) await StorageSupport.RetrieveInformationA(blob.Name, type);
                addResourceUsageToOwner(resourceUsage, ownerGroupedUsages);
            }
            await storeOwnerContentsAsCollections(ownerGroupedUsages);
        }

        private static async Task storeOwnerContentsAsCollections(Dictionary<string, List<RequestResourceUsage>> ownerGroupedUsages)
        {
            var allKeys = ownerGroupedUsages.Keys;
            foreach (var ownerKey in allKeys)
            {
                IContainerOwner owner;
                if (ownerKey.StartsWith(SystemSupport.SystemOwnerRoot))
                    owner = SystemOwner.CurrentSystem;
                else
                    owner = VirtualOwner.FigureOwner(ownerKey);
                var ownerContent = ownerGroupedUsages[ownerKey];
                var firstRangeItem = ownerContent[0];
                var lastRangeItem = ownerContent[ownerContent.Count - 1];
                string collName = String.Format("{0}_{1}",
                                                firstRangeItem.ProcessorUsage.TimeRange.EndTime.ToString(
                                                    "yyyyMMddHHmmssfff"),
                                                lastRangeItem.ProcessorUsage.TimeRange.EndTime.ToString(
                                                    "yyyyMMddHHmmssfff"));
                //var existing = RequestResourceUsageCollection.RetrieveFromOwnerContent(owner, collName);
                //if (existing != null)
                //    continue;
                RequestResourceUsageCollection ownerCollection = new RequestResourceUsageCollection();
                ownerCollection.SetLocationAsOwnerContent(owner, collName);
                ownerCollection.CollectionContent = ownerContent;
                await ownerCollection.StoreInformationAsync(null, true);
            }
        }

        private static void addResourceUsageToOwner(RequestResourceUsage resourceUsage, Dictionary<string, List<RequestResourceUsage>> ownerGroupedUsages)
        {
            string ownerPrefixKey = resourceUsage.OwnerInfo.OwnerType + "/" + resourceUsage.OwnerInfo.OwnerIdentifier;
            List<RequestResourceUsage> ownerContent = null;
            if (ownerGroupedUsages.ContainsKey(ownerPrefixKey))
            {
                ownerContent = ownerGroupedUsages[ownerPrefixKey];
            }
            else
            {
                ownerContent = new List<RequestResourceUsage>();
                ownerGroupedUsages.Add(ownerPrefixKey, ownerContent);
            }
            ownerContent.Add(resourceUsage);
        }

        public static async Task ExecuteMethod_DeleteProcessedItemsAsync(CloudBlockBlob[] batchToProcess)
        {
            if (batchToProcess == null)
                return;
            int i = 0;
            var deleteTasks = batchToProcess.Select(blob =>
                {
                    i++;
                    Debug.WriteLine("Deleting blob " + i + ": " + blob.Name);
                    return blob.DeleteIfExistsAsync(); 
                });
            await Task.WhenAll(deleteTasks);
        }

        public static async Task ExecuteMethod_ReleaseLockAsync(CloudBlockBlob[] batchToProcess)
        {
            if (batchToProcess == null)
                return;
            // Release lock
            var storageService = CoreServices.GetCurrent<IStorageService>();
            await storageService.ReleaseLogicalLockByDeletingBlobAsync(LockLocation, null);
        }

        public static ProcessBatchOfResourceUsagesToOwnerCollectionsReturnValue Get_ReturnValue(int processBatchSize, CloudBlockBlob[] batchToProcess)
        {
            return new ProcessBatchOfResourceUsagesToOwnerCollectionsReturnValue
                {
                    ProcessedAnything = batchToProcess != null,
                    ProcessedFullCount = batchToProcess != null && processBatchSize == batchToProcess.Length
                };
        }
    }

}