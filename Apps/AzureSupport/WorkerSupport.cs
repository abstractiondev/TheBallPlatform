using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using AzureSupport.TheBall.CORE;
using Microsoft.WindowsAzure.StorageClient;
using TheBall.CORE;
using TheBall.Index;

namespace TheBall
{
    public static class WorkerSupport
    {
        class BlobCopyItem
        {
            public CloudBlob SourceBlob;
            public CloudBlob TargetBlob;
        }

        public enum SyncOperationType
        {
            Copy,
            Delete,
        }

        public delegate bool PerformCustomOperation(CloudBlob source, CloudBlob target, SyncOperationType operationType);

        private static void UpdateContainerFromMasterCollection(string containerType, string containerLocation, string masterCollectionType, string masterCollectionLocation)
        {
            IInformationObject containerObject = StorageSupport.RetrieveInformation(containerLocation, containerType);
            IInformationObject masterCollectionObject = StorageSupport.RetrieveInformation(masterCollectionLocation,
                                                                                           masterCollectionType);
            IInformationCollection masterCollection = (IInformationCollection) masterCollectionObject;
            // TODO: Revisit why this can be null
            if (containerObject == null || masterCollection == null)
                return;
            containerObject.UpdateCollections(masterCollection);
            StorageSupport.StoreInformation(containerObject);
        }

        private static void UpdateCollectionFromMasterCollection(string referenceCollectionType, string referenceCollectionLocation, string masterCollectionType, string masterCollectionLocation)
        {
            IInformationObject referenceCollectionObject = StorageSupport.RetrieveInformation(referenceCollectionLocation,
                                                                                        referenceCollectionType);
            IInformationCollection referenceCollection = (IInformationCollection) referenceCollectionObject;
            // TODO: Revisit why this can be null
            if (referenceCollection == null)
                return;
            referenceCollection.RefreshContent();
            StorageSupport.StoreInformation(referenceCollectionObject);
        }

        private static void UpdateCollectionFromDirectory(string collectionType, string collectionLocation, string directoryLocation)
        {
            IInformationObject collectionObject = StorageSupport.RetrieveInformation(collectionLocation, collectionType);
            IInformationCollection collection = (IInformationCollection) collectionObject;
            collection.RefreshContent();
            StorageSupport.StoreInformation(collectionObject);
        }

        public static void UpdateContainerFromMaster(string containerLocation, string containerType, string masterLocation, string masterType)
        {
            bool masterEtagUpdated = false;
            //do
            //{
                masterEtagUpdated = false;
                IInformationObject container = StorageSupport.RetrieveInformation(containerLocation, containerType);
                IInformationObject referenceToMaster = StorageSupport.RetrieveInformation(masterLocation, masterType);
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
                    container.StoreInformation();
                    referenceToMaster = StorageSupport.RetrieveInformation(masterLocation, masterType);
                    masterEtagUpdated = referenceToMaster.ETag != masterEtag;
                }
            //} while (masterEtagUpdated);
        }

        public static int WebContentSync(string sourceContainerName, string sourcePathRoot, string targetContainerName, string targetPathRoot, PerformCustomOperation customHandler = null)
        {
            //requestOptions.BlobListingDetails = BlobListingDetails.Metadata;
            string sourceSearchRoot = sourceContainerName + "/" + sourcePathRoot;
            string targetSearchRoot = targetContainerName + "/" + targetPathRoot;
            CloudBlobContainer targetContainer = StorageSupport.CurrBlobClient.GetContainerReference(targetContainerName);
            BlobRequestOptions requestOptions = new BlobRequestOptions
                                                    {
                                                        UseFlatBlobListing = true,
                                                        BlobListingDetails = BlobListingDetails.Metadata,
                                                    };
            var sourceBlobList = StorageSupport.CurrBlobClient.ListBlobsWithPrefix(sourceSearchRoot, requestOptions).
                OfType<CloudBlob>().OrderBy(blob => blob.Name).ToArray();
            var targetBlobList = StorageSupport.CurrBlobClient.ListBlobsWithPrefix(targetSearchRoot, requestOptions).
                OfType<CloudBlob>().OrderBy(blob => blob.Name).ToArray();
            List<CloudBlob> targetBlobsToDelete;
            List<BlobCopyItem> blobCopyList;
            int sourcePathLen = sourcePathRoot.Length;
            int targetPathLen = targetPathRoot.Length;
            CompareSourceToTarget(sourceBlobList, targetBlobList, sourcePathLen, targetPathLen, 
                out blobCopyList, out targetBlobsToDelete);
            foreach(var blobToDelete in targetBlobsToDelete)
            {
                try
                {
                    bool handled = false;
                    if (customHandler != null)
                    {
                        handled = customHandler(null, blobToDelete, SyncOperationType.Delete);
                    }
                    if (handled == false)
                        blobToDelete.DeleteWithoutFiringSubscriptions();
                }
                catch (WebException wex)
                {
                    throw new InvalidDataException("Error with blob deletion: " + blobToDelete.Name, wex);
                }
            }
            foreach(var blobCopyItem in blobCopyList)
            {
                try
                {
                    CloudBlob targetBlob;
                    if (blobCopyItem.TargetBlob == null)
                    {
                        string sourceBlobNameWithoutSourcePrefix = blobCopyItem.SourceBlob.Name.Substring(sourcePathRoot.Length);
                        string targetBlobName;
                        if (sourceBlobNameWithoutSourcePrefix.StartsWith("/") && String.IsNullOrEmpty(targetPathRoot))
                            targetBlobName = sourceBlobNameWithoutSourcePrefix.Substring(1);
                        else
                            targetBlobName = targetPathRoot + sourceBlobNameWithoutSourcePrefix;
                        //string targetBlobName = String.IsNullOrEmpty(targetPathRoot) ? sourceBlobName.
                        //string targetBlobName = 
                        //    blobCopyItem.SourceBlob.Name.Replace(sourcePathRoot, targetPathRoot);
                        targetBlob = targetContainer.GetBlobReference(targetBlobName);
                    }
                    else
                        targetBlob = blobCopyItem.TargetBlob;
                    bool handled = false;
                    Console.WriteLine("Processing sync: " + blobCopyItem.SourceBlob.Name + " => " + targetBlob.Name);
                    if (customHandler != null)
                    {
                        handled = customHandler(blobCopyItem.SourceBlob, targetBlob, SyncOperationType.Copy);
                    }
                    if (handled == false)
                        targetBlob.CopyFromBlob(blobCopyItem.SourceBlob);
                }
                catch (WebException wex)
                {
                    throw new InvalidDataException("Error with blob copy: " + blobCopyItem.SourceBlob.Name, wex);
                }

            }
            return targetBlobsToDelete.Count + blobCopyList.Count;
        }

        private static void CompareSourceToTarget(CloudBlob[] sourceBlobs, CloudBlob[] targetBlobs, int sourcePathLen, int targetPathLen, out List<BlobCopyItem> blobCopyList, out List<CloudBlob> targetBlobsToDelete)
        {
            blobCopyList = new List<BlobCopyItem>();
            targetBlobsToDelete = new List<CloudBlob>();
            int currTargetIX = 0;
            int currSourceIX = 0;
            while(currSourceIX < sourceBlobs.Length && currTargetIX < targetBlobs.Length)
            {
                CloudBlob currSourceItem = sourceBlobs[currSourceIX];
                CloudBlob currTargetItem = targetBlobs[currTargetIX];
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

        
        public static void ProcessUpdateWebContent(UpdateWebContentOperation operation)
        {
            string sourceContainerName = operation.SourceContainerName;
            string sourcePathRoot = operation.SourcePathRoot;
            string targetContainerName = operation.TargetContainerName;
            string targetPathRoot = operation.TargetPathRoot;
            bool renderWhileSync = operation.RenderWhileSync;
            WorkerSupport.WebContentSync(sourceContainerName, sourcePathRoot, targetContainerName, targetPathRoot,
                                         renderWhileSync
                                             ? (WorkerSupport.PerformCustomOperation)RenderWebSupport.RenderingSyncHandler
                                             : (WorkerSupport.PerformCustomOperation)RenderWebSupport.CopyAsIsSyncHandler);
        }


        public static void DeleteEntireOwner(IContainerOwner containerOwner)
        {
            StorageSupport.DeleteEntireOwner(containerOwner);
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


        public static void ProcessIndexing(QueueSupport.MessageObject<string>[] indexingMessages, string indexStorageRootFolder)
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
                    InformationContext.Current.InitializeCloudStorageAccess(containerName: containerName);
                    InformationContext.Current.Owner = owner;
                    InformationContext.StartResourceMeasuringOnCurrent(InformationContext.ResourceUsageType.WorkerIndexing);
                    IndexInformation.Execute(new IndexInformationParameters
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
                        InformationContext.ProcessAndClearCurrent();

                }
            }
        }

        public static void ProcessQueries(QueueSupport.MessageObject<string>[] queryMessages, string indexStorageRootFolder)
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
                    InformationContext.Current.InitializeCloudStorageAccess(containerName: containerName);
                    InformationContext.Current.Owner = owner;
                    InformationContext.StartResourceMeasuringOnCurrent(InformationContext.ResourceUsageType.WorkerQuery);
                    QueryIndexedInformation.Execute(new QueryIndexedInformationParameters
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
                        InformationContext.ProcessAndClearCurrent();

                }
            }
        }

        public static void ProcessPublishWebContent(string sourceContainerName, string sourceOwner, string sourceRoot, string targetContainerName)
        {
            // Hardcoded double-verify for valid container
            var blob = StorageSupport.GetBlob(targetContainerName, RenderWebSupport.CurrentToServeFileName);
            var blobData = blob.DownloadText();
            string[] contentArr = blobData.Split(':');
            if (contentArr.Length < 2 || contentArr[1] != sourceOwner)
                return;
            DateTime currPublishTimeUtc = DateTime.UtcNow;
            string targetRootFolderName = currPublishTimeUtc.ToString("yyyy-MM-dd_HH-mm-ss");
            // Sync website
            string targetWebsiteRoot = targetRootFolderName;
            VirtualOwner owner = VirtualOwner.FigureOwner(sourceOwner);
            string sourceWebsiteRoot = sourceOwner + "/" + sourceRoot;
            WebContentSync(sourceContainerName, sourceWebsiteRoot, targetContainerName, targetWebsiteRoot,
                           RenderWebSupport.CopyAsIsSyncHandler);
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
                WebContentSync(sourceContainerName, sourceFolder, targetContainerName, targetFolder,
                               RenderWebSupport.CopyAsIsSyncHandler);
            }
            var lastUpdateFileBlob = StorageSupport.GetBlob(targetContainerName, RenderWebSupport.LastUpdateFileName);
            lastUpdateFileBlob.UploadBlobText(targetRootFolderName);
            var currentToServeBlob = StorageSupport.GetBlob(targetContainerName, RenderWebSupport.CurrentToServeFileName);
            currentToServeBlob.UploadBlobText(targetRootFolderName + ":" + sourceOwner);
        }
    }
}