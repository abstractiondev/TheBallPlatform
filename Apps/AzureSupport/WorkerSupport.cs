using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using AzureSupport.TheBall.Core;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.Core;
using TheBall.Core.Storage;
using TheBall.Index;

namespace TheBall
{
    public static class WorkerSupport
    {
        class BlobCopyItem
        {
            public CloudBlockBlob SourceBlob;
            public CloudBlockBlob TargetBlob;
        }

        public enum SyncOperationType
        {
            Copy,
            Delete,
        }

        public delegate Task<bool> PerformCustomOperation(CloudBlob source, CloudBlob target, SyncOperationType operationType);

        public static async Task UpdateContainerFromMaster(string containerLocation, Type containerType, string masterLocation, Type masterType)
        {
            bool masterEtagUpdated = false;
            //do
            //{
                masterEtagUpdated = false;
                IInformationObject container = await StorageSupport.RetrieveInformationA(containerLocation, containerType);
                IInformationObject referenceToMaster = await StorageSupport.RetrieveInformationA(masterLocation, masterType);
                string masterEtag = referenceToMaster.ETag;
                string masterID = referenceToMaster.ID;
                Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
                container.CollectMasterObjectsFromTree(result, candidate => candidate.ID == masterID);
                bool foundOutdatedMaster =
                    result.Values.SelectMany(item => item).Count(candidate => candidate.MasterETag != masterEtag) > 0;
                if(foundOutdatedMaster)
                {
                    referenceToMaster.MasterETag = referenceToMaster.ETag;
                    container.ReplaceObjectInTree(referenceToMaster);
                    await container.StoreInformationAsync();
                    referenceToMaster = await StorageSupport.RetrieveInformationA(masterLocation, masterType);
                    masterEtagUpdated = referenceToMaster.ETag != masterEtag;
                }
            //} while (masterEtagUpdated);
        }


        public static async Task<int> WebContentSyncA(string sourcePathRoot, string targetPathRoot, PerformCustomOperation customHandler = null)
        {
            //requestOptions.BlobListingDetails = BlobListingDetails.Metadata;
            if (sourcePathRoot.EndsWith("/") == false)
                sourcePathRoot += "/";
            if (targetPathRoot.EndsWith("/") == false)
                targetPathRoot += "/";
            var sourceBlobList = (await StorageSupport.GetBlobsWithMetadataA(null, sourcePathRoot, true)).OrderBy(blob => blob.Name).ToArray();
            var targetBlobList =
                (await StorageSupport.GetBlobsWithMetadataA(null, targetPathRoot, true)).OrderBy(blob => blob.Name)
                    .ToArray();
            List<CloudBlockBlob> targetBlobsToDelete;
            List<BlobCopyItem> blobCopyList;
            int sourcePathLen = sourcePathRoot.Length;
            int targetPathLen = targetPathRoot.Length;
            CompareSourceToTarget(sourceBlobList, targetBlobList, sourcePathLen, targetPathLen,
                out blobCopyList, out targetBlobsToDelete);
            var processTasks = new List<Task>();
            foreach (var blobToDelete in targetBlobsToDelete)
            {
                Task handleTask = null;
                if (customHandler != null)
                    handleTask = customHandler(null, blobToDelete, SyncOperationType.Delete);
                if (handleTask == null)
                {
                    var deleteTask = blobToDelete.DeleteIfExistsAsync();
                    handleTask = deleteTask;
                }
                processTasks.Add(handleTask);
            }
            foreach (var blobCopyItem in blobCopyList)
            {
                CloudBlockBlob targetBlob;
                if (blobCopyItem.TargetBlob == null)
                {
                    string sourceBlobNameWithoutSourcePrefix =
                        blobCopyItem.SourceBlob.Name.Substring(sourcePathRoot.Length);
                    string targetBlobName;
                    if (sourceBlobNameWithoutSourcePrefix.StartsWith("/") && String.IsNullOrEmpty(targetPathRoot))
                        targetBlobName = sourceBlobNameWithoutSourcePrefix.Substring(1);
                    else
                        targetBlobName = targetPathRoot + sourceBlobNameWithoutSourcePrefix;
                    //string targetBlobName = String.IsNullOrEmpty(targetPathRoot) ? sourceBlobName.
                    //string targetBlobName = 
                    //    blobCopyItem.SourceBlob.Name.Replace(sourcePathRoot, targetPathRoot);
                    targetBlob = StorageSupport.CurrActiveContainer.GetBlockBlobReference(targetBlobName);
                }
                else
                    targetBlob = blobCopyItem.TargetBlob;
                Console.WriteLine("Processing sync: " + blobCopyItem.SourceBlob.Name + " => " + targetBlob.Name);
                Task handleTask = null;
                if (customHandler != null)
                    handleTask = customHandler(blobCopyItem.SourceBlob, targetBlob, SyncOperationType.Copy);
                if (handleTask == null)
                {
                    var copyTask = targetBlob.StartCopyAsync(blobCopyItem.SourceBlob);
                    handleTask = copyTask;
                }
                processTasks.Add(handleTask);
            }
            await Task.WhenAll(processTasks);
            return targetBlobsToDelete.Count + blobCopyList.Count;
        }


        public static async Task<int> WebContentSyncBetweenContainersA(string sourceContainerName, string sourcePathRoot, string targetContainerName, string targetPathRoot, PerformCustomOperation customHandler = null)
        {
            //requestOptions.BlobListingDetails = BlobListingDetails.Metadata;
            if (sourcePathRoot.EndsWith("/") == false)
                sourcePathRoot += "/";
            if (targetPathRoot.EndsWith("/") == false)
                targetPathRoot += "/";
            string sourceSearchRoot = sourceContainerName + "/" + sourcePathRoot;
            string targetSearchRoot = targetContainerName + "/" + targetPathRoot;
            CloudBlobContainer targetContainer = StorageSupport.CurrBlobClient.GetContainerReference(targetContainerName);
            CloudBlockBlob[] sourceBlobList = null;
            //StorageSupport.CurrBlobClient.ListBlobs(sourceSearchRoot, true, BlobListingDetails.Metadata).
            //OfType<CloudBlockBlob>().OrderBy(blob => blob.Name).ToArray();
            CloudBlockBlob[] targetBlobList = null;
                //StorageSupport.CurrBlobClient.ListBlobs(targetSearchRoot, true, BlobListingDetails.Metadata).
                //OfType<CloudBlockBlob>().OrderBy(blob => blob.Name).ToArray();
            List<CloudBlockBlob> targetBlobsToDelete = null;
            List<BlobCopyItem> blobCopyList;
            int sourcePathLen = sourcePathRoot.Length;
            int targetPathLen = targetPathRoot.Length;
            CompareSourceToTarget(sourceBlobList, targetBlobList, sourcePathLen, targetPathLen,
                out blobCopyList, out targetBlobsToDelete);
            var processTasks = new List<Task>();
            foreach (var blobToDelete in targetBlobsToDelete)
            {
                Task handleTask = null;
                if (customHandler != null)
                    handleTask = customHandler(null, blobToDelete, SyncOperationType.Delete);
                if (handleTask == null)
                {
                    var deleteTask = blobToDelete.DeleteIfExistsAsync();
                    handleTask = deleteTask;
                }
                processTasks.Add(handleTask);
            }
            foreach (var blobCopyItem in blobCopyList)
            {
                CloudBlockBlob targetBlob;
                if (blobCopyItem.TargetBlob == null)
                {
                    string sourceBlobNameWithoutSourcePrefix =
                        blobCopyItem.SourceBlob.Name.Substring(sourcePathRoot.Length);
                    string targetBlobName;
                    if (sourceBlobNameWithoutSourcePrefix.StartsWith("/") && String.IsNullOrEmpty(targetPathRoot))
                        targetBlobName = sourceBlobNameWithoutSourcePrefix.Substring(1);
                    else
                        targetBlobName = targetPathRoot + sourceBlobNameWithoutSourcePrefix;
                    //string targetBlobName = String.IsNullOrEmpty(targetPathRoot) ? sourceBlobName.
                    //string targetBlobName = 
                    //    blobCopyItem.SourceBlob.Name.Replace(sourcePathRoot, targetPathRoot);
                    targetBlob = targetContainer.GetBlockBlobReference(targetBlobName);
                }
                else
                    targetBlob = blobCopyItem.TargetBlob;
                Console.WriteLine("Processing sync: " + blobCopyItem.SourceBlob.Name + " => " + targetBlob.Name);
                Task handleTask = null;
                if (customHandler != null)
                    handleTask = customHandler(blobCopyItem.SourceBlob, targetBlob, SyncOperationType.Copy);
                if (handleTask == null)
                {
                    var copyTask = targetBlob.StartCopyAsync(blobCopyItem.SourceBlob);
                    handleTask = copyTask;
                }
                processTasks.Add(handleTask);
            }
            await Task.WhenAll(processTasks);
            return targetBlobsToDelete.Count + blobCopyList.Count;
        }

        private static void CompareSourceToTarget(CloudBlockBlob[] sourceBlobs, CloudBlockBlob[] targetBlobs, int sourcePathLen, int targetPathLen, out List<BlobCopyItem> blobCopyList, out List<CloudBlockBlob> targetBlobsToDelete)
        {
            blobCopyList = new List<BlobCopyItem>();
            targetBlobsToDelete = new List<CloudBlockBlob>();
            int currTargetIX = 0;
            int currSourceIX = 0;
            while(currSourceIX < sourceBlobs.Length && currTargetIX < targetBlobs.Length)
            {
                var currSourceItem = sourceBlobs[currSourceIX];
                var currTargetItem = targetBlobs[currTargetIX];
                string sourceCompareName = currSourceItem.Name.Substring(sourcePathLen).ToLower();
                string targetCompareName = currTargetItem.Name.Substring(targetPathLen).ToLower();
                int compareResult = String.Compare(sourceCompareName, targetCompareName);
                bool namesMatch = compareResult == 0;
                bool sourceButNoTarget = compareResult < 0;
                if (namesMatch)
                {
                    // Compare blob contents
                    bool sourceIsTemplate = currSourceItem.GetBlobInformationType() ==
                                            StorageSupport.InformationType_WebTemplateValue;
                    bool targetIsTemplate = currTargetItem.GetBlobInformationType() ==
                                            StorageSupport.InformationType_WebTemplateValue;
                    if ((sourceIsTemplate && !targetIsTemplate) || currSourceItem.Properties.ContentMD5 != currTargetItem.Properties.ContentMD5)
                        blobCopyList.Add(new BlobCopyItem
                                             {
                                                 SourceBlob = currSourceItem,
                                                 TargetBlob = currTargetItem
                                             });
                    currSourceIX++;
                    currTargetIX++;
                }
                else if (sourceButNoTarget)
                {
                    blobCopyList.Add(new BlobCopyItem
                                         {
                                             SourceBlob = currSourceItem,
                                             TargetBlob = null
                                         });
                    currSourceIX++;
                }
                else // target but no source
                {
                    targetBlobsToDelete.Add(currTargetItem);
                    currTargetIX++;
                }
            }
            // if the target and source ixs are matched exit here
            if (currSourceIX == sourceBlobs.Length && currTargetIX == targetBlobs.Length)
                return;

            // Delete targets blobs that weren't enumerated before the sources did end
            while(currTargetIX < targetBlobs.Length)
                targetBlobsToDelete.Add(targetBlobs[currTargetIX++]);

            // Copy the source blobs that weren't enumerated before the targets did end
            while(currSourceIX < sourceBlobs.Length)
                blobCopyList.Add(new BlobCopyItem
                                     {
                                         SourceBlob = sourceBlobs[currSourceIX++],
                                         TargetBlob = null
                                     });
        }

        private static int counter = 0;

        
        public static async Task DeleteEntireOwner(IContainerOwner containerOwner)
        {
            await StorageSupport.DeleteEntireOwnerAsync(containerOwner);
        }

        public static Task GetFirstCompleted(Task[] tasks, out int availableIx)
        {
            Task currArrayTask = null;
            for (availableIx = 0; availableIx < tasks.Length; availableIx++)
            {
                currArrayTask = tasks[availableIx];
                if (currArrayTask.IsCompleted)
                    break;
            }
            if (currArrayTask == null)
                throw new NotSupportedException("Cannot find completed task in array when there is supposed to be one");
            return currArrayTask;
        }


        public static async Task ProcessIndexingAsync(QueueSupport.MessageObject<string>[] indexingMessages, string indexStorageRootFolder)
        {
            foreach (var indexingMessage in indexingMessages)
            {
                var splitValues = indexingMessage.RetrievedObject.Split(':');
                var containerName = splitValues[0];
                var ownerString = splitValues[1];
                var owner = VirtualOwner.FigureOwner(ownerString);
                var indexRequestID = splitValues[2];
                string containerIndexRoot = Path.Combine(indexStorageRootFolder, containerName);
                if (Directory.Exists(containerIndexRoot) == false)
                    Directory.CreateDirectory(containerIndexRoot);
                try
                {
                    InformationContext.StartResourceMeasuringOnCurrent(InformationContext.ResourceUsageType.WorkerIndexing);
                    await IndexInformation.ExecuteAsync(new IndexInformationParameters
                        {
                            IndexingRequestID = indexRequestID,
                            IndexName = IndexSupport.DefaultIndexName,
                            IndexStorageRootPath = containerIndexRoot,
                            Owner = owner
                        });
                }
                finally
                {
                    if (containerName != null)
                    {
                        await InformationContext.ProcessAndClearCurrentIfAvailableAsync();
                    }
                }
            }
        }

        public static async Task ProcessQueriesAsync(QueueSupport.MessageObject<string>[] queryMessages, string indexStorageRootFolder)
        {
            foreach (var queryMessage in queryMessages)
            {
                var splitValues = queryMessage.RetrievedObject.Split(':');
                var containerName = splitValues[0];
                var ownerString = splitValues[1];
                var owner = VirtualOwner.FigureOwner(ownerString);
                var queryRequestID = splitValues[2];
                string containerIndexRoot = Path.Combine(indexStorageRootFolder, containerName);
                if (Directory.Exists(containerIndexRoot) == false)
                    Directory.CreateDirectory(containerIndexRoot);
                try
                {
                    InformationContext.StartResourceMeasuringOnCurrent(InformationContext.ResourceUsageType.WorkerQuery);
                    await QueryIndexedInformation.ExecuteAsync(new QueryIndexedInformationParameters
                        {
                            QueryRequestID = queryRequestID,
                            IndexName = IndexSupport.DefaultIndexName,
                            IndexStorageRootPath = containerIndexRoot,
                            Owner = owner
                        });
                }
                finally
                {
                    if (containerName != null)
                        await InformationContext.ProcessAndClearCurrentIfAvailableAsync();

                }
            }
        }

        public static async Task ProcessPublishWebContent(string sourceContainerName, string sourceOwner, string sourceRoot, string targetContainerName)
        {
            // Hardcoded double-verify for valid container
            var blob = StorageSupport.GetBlob(targetContainerName, RenderWebSupport.CurrentToServeFileName);
            var blobData = await blob.DownloadTextAsync();
            string[] contentArr = blobData.Split(':');
            if (contentArr.Length < 2 || contentArr[1] != sourceOwner)
                return;
            DateTime currPublishTimeUtc = DateTime.UtcNow;
            string targetRootFolderName = currPublishTimeUtc.ToString("yyyy-MM-dd_HH-mm-ss");
            // Sync website
            string targetWebsiteRoot = targetRootFolderName;
            var owner = VirtualOwner.FigureOwner(sourceOwner);
            string sourceWebsiteRoot = sourceOwner + "/" + sourceRoot;
            await WebContentSyncBetweenContainersA(sourceContainerName, sourceWebsiteRoot, targetContainerName, targetWebsiteRoot);
            // Copy Media
            /*
            string mediaFolderName = "AaltoGlobalImpact.OIP/MediaContent";
            string targetMediaRoot = targetRootFolderName + "/" + mediaFolderName;
            string sourceMediaRoot = sourceOwner + "/" + mediaFolderName;
            WebContentSync(sourceContainerName, sourceMediaRoot, targetContainerName, targetMediaRoot,
                           RenderWebSupport.CopyAsIsSyncHandler);
             */
            // Copy required data to go with website stuff
            string[] foldersToCopy = new string[] {
                "AaltoGlobalImpact.OIP/NodeSummaryContainer",
                "AaltoGlobalImpact.OIP/TextContent",
                "AaltoGlobalImpact.OIP/EmbeddedContent",
                "AaltoGlobalImpact.OIP/AddressAndLocationCollection",
                "AaltoGlobalImpact.OIP/MediaContent",
                "AaltoGlobalImpact.OIP/GroupContainer",
                "AaltoGlobalImpact.OIP/AttachedToObjectCollection",
                "AaltoGlobalImpact.OIP/BinaryFileCollection",
                "AaltoGlobalImpact.OIP/LinkToContentCollection",
                "AaltoGlobalImpact.OIP/EmbeddedContentCollection",
                "AaltoGlobalImpact.OIP/CategoryCollection",
                "AaltoGlobalImpact.OIP/ContentCategoryRankCollection",
                "AaltoGlobalImpact.OIP/DynamicContentCollection"
            };
            foreach (string renderRequiredFolder in foldersToCopy)
            {
                string targetFolder = targetRootFolderName + "/" + renderRequiredFolder;
                string sourceFolder = sourceOwner + "/" + renderRequiredFolder;
                await WebContentSyncBetweenContainersA(sourceContainerName, sourceFolder, targetContainerName, targetFolder);
            }
            var lastUpdateFileBlob = StorageSupport.GetBlob(targetContainerName, RenderWebSupport.LastUpdateFileName);
            await lastUpdateFileBlob.UploadBlobTextAsync(targetRootFolderName);
            var currentToServeBlob = StorageSupport.GetBlob(targetContainerName, RenderWebSupport.CurrentToServeFileName);
            await currentToServeBlob.UploadBlobTextAsync(targetRootFolderName + ":" + sourceOwner);
        }
    }
}