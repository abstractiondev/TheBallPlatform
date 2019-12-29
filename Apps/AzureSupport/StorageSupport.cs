using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AaltoGlobalImpact.OIP;
using AzureSupport;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using TheBall.Core;
using TheBall.Core.InstanceSupport;
using TheBall.Core.Storage;
using TheBall.Core.StorageCore;
using TheBall.Index;

namespace TheBall
{
    public static class StorageSupport
    {
        public const string SubscriptionContainer = "subscription";
        public static string InformationTypeKey = "InformationType";
        public static string InformationObjectTypeKey = "InformationObjectType";
        public static string InformationSourcesKey = "InformationSources";
        public static string InformationType_WebTemplateValue = "WebTemplate";
        public static string InformationType_RuntimeWebTemplateValue = "RuntimeWebTemplate";
        public static string InformationType_InformationObjectValue = "InformationObject";
        public static string InformationType_RenderedWebPage = "RenderedWebPage";
        public static string InformationType_GenericContentValue = "GenericContent";

        static StorageSupport()
        {
        }

        private static string getContainerName(string instanceName)
        {
            string containerName = instanceName.Replace('.', '-').ToLower();
            if (InstanceConfig.Current.ContainerRedirectsDict.ContainsKey(containerName))
                return InstanceConfig.Current.ContainerRedirectsDict[containerName];
            return containerName;
        }

        //public static CloudTableClient CurrTableClient { get; private set; }
        [Obsolete("error", true)]
        public static CloudStorageAccount CurrStorageAccount
        {
            get
            {
                var storageAccount = SecureConfig.Current.AzureAccountName;
                var storageKey = SecureConfig.Current.AzureStorageKey;
                CloudStorageAccount account = new CloudStorageAccount(new StorageCredentials(storageAccount, storageKey), true);
                return account;
            }
        }

        public static async Task<byte[]> DownloadByteArrayAsync(this CloudFile file)
        {
            using (var memStream = new MemoryStream())
            {
                await file.DownloadToStreamAsync(memStream);
                return memStream.ToArray();
            }
        }

        public static async Task<byte[]> DownloadByteArrayAsync(this CloudBlockBlob blob)
        {
            using (var memStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(memStream);
                return memStream.ToArray();
            }
        }

        [Obsolete("error", true)]
        public static CloudBlobContainer CurrActiveContainer
        {
            get
            {
                var blobClient = CurrBlobClient;
                var containerName = getContainerName(InformationContext.Current.InstanceName);
                return blobClient.GetContainerReference(containerName);
            }
        }

        [Obsolete("error", true)]
        public static CloudBlobClient CurrBlobClient
        {
            get
            {
                var blobClient = CurrStorageAccount.CreateCloudBlobClient();
                return blobClient;
            }
        }
        public const int AccOrGrpPlusIDPathLength = 41;
        private const string ContentFolderName = "Content";
        public const int GuidLength = 36;

        public static Guid ActiveOwnerID
        {
            get { return Guid.Empty; }
        }

        public static string GetParentDirectoryTarget(string targetLocation)
        {
            int lastDirectorySlashIndex = targetLocation.LastIndexOf("/");
            string directoryLocation = targetLocation.Substring(0, lastDirectorySlashIndex + 1);
            return directoryLocation;
        }

        public static string GetBlobInformationType(this CloudBlockBlob blob)
        {
            //FetchMetadataIfMissing(blob);
            if (Path.HasExtension(blob.Name) && !blob.Name.EndsWith(".pending"))
                return InformationType_GenericContentValue;
            if (blob.Name.Contains("/AaltoGlobalImpact.OIP/MediaContent/"))
                return InformationType_GenericContentValue;
            return InformationType_InformationObjectValue;
            //return blob.Attributes.Metadata[InformationTypeKey];
        }

        public static string GetBlobInformationType(this BlobStorageItem blob)
        {
            //FetchMetadataIfMissing(blob);
            if (Path.HasExtension(blob.Name) && !blob.Name.EndsWith(".pending"))
                return InformationType_GenericContentValue;
            if (blob.Name.Contains("/AaltoGlobalImpact.OIP/MediaContent/"))
                return InformationType_GenericContentValue;
            return InformationType_InformationObjectValue;
            //return blob.Attributes.Metadata[InformationTypeKey];
        }

        public static string GetBlobInformationObjectType(this CloudBlockBlob blob)
        {
            return InformationObjectSupport.GetInformationObjectType(blob.Name);
        }


        public static async Task<byte[]> DownloadBlobByteArrayAsync(string blobPath, bool returnNullIfMissing = false, IContainerOwner dedicatedOwner = null)
        {
            try
            {
                if (dedicatedOwner == null)
                    dedicatedOwner = InformationContext.CurrentOwner;
                var blob = GetOwnerBlobReference(dedicatedOwner, blobPath);
                using (var memStream = new MemoryStream())
                {
                    await blob.DownloadToStreamAsync(memStream);
                    return memStream.ToArray();
                }
            }
            catch (StorageException stEx)
            {
                if (returnNullIfMissing && stEx.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        [Obsolete("error", true)]
        public static async Task<CloudBlockBlob> UploadBlobTextAsync(this CloudBlobContainer container,
            string blobPath, string textContent)
        {
            var blob = container.GetBlockBlobReference(blobPath);
            await UploadBlobTextAsync(blob, textContent);
            return blob;
        }

        [Obsolete("error", true)]
        public static async Task UploadBlobTextAsync(this CloudBlockBlob blob, string textContent, bool requireNonExistentBlob = false)
        {
            //blob.Properties.ContentType = GetMimeType(Path.GetExtension(blob.Name));
            BlobRequestOptions options = new BlobRequestOptions();
            options.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(3), 10);
            var accessCondition = requireNonExistentBlob ? AccessCondition.GenerateIfNoneMatchCondition("*") : null;
            await blob.UploadTextAsync(textContent, Encoding.UTF8, accessCondition, options, null);
        }


        [Obsolete("error", true)]
        public static async Task<CloudBlockBlob> UploadBlobBinaryAsync(this CloudBlobContainer container, string blobPath, byte[] binaryContent)
        {
            var blob = container.GetBlockBlobReference(blobPath);
            //blob.Properties.ContentType = GetMimeType(Path.GetExtension(blobPath));
            await blob.UploadFromByteArrayAsync(binaryContent, 0, binaryContent.Length);
            InformationContext.AddStorageTransactionToCurrent();
            return blob;
        }

        [Obsolete("error", true)]
        public static async Task UploadBlobStreamAsync(this CloudBlobContainer container,
    string blobPath, Stream streamContent)
        {
            var blob = container.GetBlockBlobReference(blobPath);
            //blob.Properties.ContentType = GetMimeType(Path.GetExtension(blobPath));
            await blob.UploadFromStreamAsync(streamContent);
            InformationContext.AddStorageTransactionToCurrent();
        }

        public static string GetContainerNameFromHostName(string hostName)
        {
            return hostName.Replace(".", "-");
        }

        public static async Task ReconnectMastersAndCollectionsAsync(this IInformationObject informationObject, bool updateContents)
        {
            var owner = VirtualOwner.FigureOwner(informationObject.RelativeLocation);
            if (updateContents)
            {
                IBeforeStoreHandler beforeStoreHandler = informationObject as IBeforeStoreHandler;
                if(beforeStoreHandler != null)
                    beforeStoreHandler.PerformBeforeStoreUpdate();
            }
            List<IInformationObject> masterObjects = new List<IInformationObject>();
            List<IInformationObject> masterReferringCollections = new List<IInformationObject>();
            informationObject.FindObjectsFromTree(masterObjects, candidate => candidate.IsIndependentMaster, true);
            informationObject.FindObjectsFromTree(
                masterReferringCollections, candidate =>
                                                {
                                                    IInformationCollection informationCollection =
                                                        candidate as IInformationCollection;
                                                    return informationCollection != null &&
                                                           informationCollection.IsMasterCollection;
                                                }, true);
            foreach(var referenceMaster in masterObjects)
            {
                if (referenceMaster == informationObject)
                    continue;
                FixOwnerLocation(referenceMaster, owner);
                var realMaster = await referenceMaster.RetrieveMasterAsync(true);
                if (updateContents)
                {
                    if (referenceMaster.MasterETag != realMaster.ETag)
                    {
                        referenceMaster.UpdateMasterValueTreeFromOtherInstance(realMaster);
                        referenceMaster.MasterETag = realMaster.ETag;
                    }
                }
            }
            foreach(IInformationCollection referringCollection in masterReferringCollections)
            {
                if (referringCollection == informationObject)
                    continue;
                IInformationObject referringObject = (IInformationObject) referringCollection;
                FixOwnerLocation(referringObject, owner);
                string masterLocation = referringCollection.GetMasterLocation();
                // Don't self master
                if (masterLocation == referringObject.RelativeLocation)
                    continue;
                if(updateContents)
                {
                    var masterInstance = await referringCollection.GetMasterInstanceAsync();
                    informationObject.UpdateCollections(masterInstance: masterInstance);
                }
            }
            if (updateContents)
                await StoreInformationAsync(informationObject);
        }

        public static void FixOwnerLocation(IInformationObject informationObject, IContainerOwner owner)
        {
            string ownerLocation = GetOwnerContentLocation(owner, informationObject.RelativeLocation);
            informationObject.RelativeLocation = ownerLocation;
        }

        public static async Task<BlobStorageItem> StoreInformationAsync(this IInformationObject informationObject, IContainerOwner owner = null, bool overwriteIfExists = false)
        {
            var storageService = CoreServices.GetCurrent<IStorageService>();
            string location = owner != null
                                  ? GetOwnerContentLocation(owner, informationObject.RelativeLocation)
                                  : informationObject.RelativeLocation;
            // Updating the relative location just in case - as there shouldn't be a mismatch - critical for master objects
            informationObject.RelativeLocation = location;
            var beforeStoreHandler = informationObject as IBeforeStoreHandler;
            beforeStoreHandler?.PerformBeforeStoreUpdate();
            var dataContent = SerializeInformationObjectToBuffer(informationObject);
            //memoryStream.Seek(0, SeekOrigin.Begin);
            //CloudBlockBlob blob = CurrActiveContainer.GetBlockBlobReference(location);
            string etag = informationObject.ETag;
            bool isNewBlob = etag == null;
            AccessCondition accessCondition = null;
            if (!overwriteIfExists)
            {
                accessCondition = etag != null ? AccessCondition.GenerateIfMatchCondition(etag) : AccessCondition.GenerateIfNoneMatchCondition("*");
            }
            //blob.SetBlobInformationObjectType(informationObjectType.FullName);
            var blobLocation = informationObject.RelativeLocation;
            var blobItem = await storageService.UploadBlobDataA(owner, blobLocation, dataContent, etag);
            InformationContext.AddStorageTransactionToCurrent();
            informationObject.ETag = blobItem.ETag;
            IAdditionalFormatProvider additionalFormatProvider = informationObject as IAdditionalFormatProvider;
            if (additionalFormatProvider != null)
            {
                var additionalContentToStore = additionalFormatProvider.GetAdditionalContentToStore(informationObject.ETag);
                foreach (var additionalContent in additionalContentToStore)
                {
                    string contentLocation = blobItem.Name + "." + additionalContent.Extension;
                    //await CurrActiveContainer.UploadBlobBinaryAsync(contentLocation, additionalContent.Content);
                    await storageService.UploadBlobDataA(null, contentLocation, additionalContent.Content);
                    InformationContext.AddStorageTransactionToCurrent();
                }
            }
            informationObject.PostStoringExecute(owner);
            Debug.WriteLine(String.Format("Wrote: {0} ID {1}", informationObject.GetType().Name,
                informationObject.ID));
            InformationContext.Current.ObjectStoredNotification(informationObject,
                isNewBlob ? InformationContext.ObjectChangeType.N_New : InformationContext.ObjectChangeType.M_Modified);
            return blobItem;
        }


        private static string resolveTypeNameFromRelativeLocation(string relativeLocation)
        {
            string[] partsOfLocation = relativeLocation.Split('/');
            var owner = VirtualOwner.FigureOwner(relativeLocation);
            return partsOfLocation[2] + "." + partsOfLocation[3];
        }

        public static async Task<IInformationObject> RetrieveInformationA(string relativeLocation, Type typeToRetrieve, string eTag = null, IContainerOwner owner = null)
        {
            var result = await RetrieveInformationWithBlobA(relativeLocation, typeToRetrieve, eTag, owner);
            return result?.Item1;
        }

        public static async Task<Tuple<IInformationObject, BlobStorageItem>> RetrieveInformationWithBlobA(string relativeLocation, Type typeToRetrieve, string eTag = null, IContainerOwner owner = null)
        {
            //if (owner != null)
            //    relativeLocation = GetOwnerContentLocation(owner, relativeLocation);
            //var blob = CurrActiveContainer.GetBlockBlobReference(relativeLocation);
            var storageService = CoreServices.GetCurrent<IStorageService>();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                string blobEtag = null;
                BlobStorageItem blobItem;
                try
                {
                    blobItem = await storageService.GetBlobItemA(owner, relativeLocation);
                    await storageService.DownloadBlobStreamA(owner, relativeLocation, memoryStream, true, eTag);
                    InformationContext.AddStorageTransactionToCurrent();
                    blobEtag = blobItem.ETag;
                }
                catch (StorageException stEx)
                {
                    if (stEx.RequestInformation.HttpStatusCode == (int) HttpStatusCode.NotFound)
                        return null;
                    throw;
                }

                //if (memoryStream.Length == 0)
                //    return null;
                memoryStream.Seek(0, SeekOrigin.Begin);
                var informationObject = DeserializeInformationObjectFromStream(typeToRetrieve, memoryStream);
                informationObject.ETag = blobEtag;
                //informationObject.RelativeLocation = blob.Attributes.Metadata["RelativeLocation"];
                informationObject.RelativeLocation = relativeLocation;
                informationObject.SetInstanceTreeValuesAsUnmodified();
                Debug.WriteLine(String.Format("Read: {0} ID {1}", informationObject.GetType().Name,
                    informationObject.ID));
                return new Tuple<IInformationObject, BlobStorageItem>(informationObject, blobItem);
            }
        }


        private static IInformationObject DeserializeInformationObjectFromStream(Type typeToRetrieve, MemoryStream memoryStream)
        {
            StorageSerializationType storageSerializationType = getStorageSerializationType(typeToRetrieve);
            IInformationObject informationObject = null;
            if (storageSerializationType == StorageSerializationType.XML)
            {
                DataContractSerializer serializer = new DataContractSerializer(typeToRetrieve);
                informationObject = (IInformationObject) serializer.ReadObject(memoryStream);
            }
            else if (storageSerializationType == StorageSerializationType.JSON)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeToRetrieve);
                informationObject = (IInformationObject) serializer.ReadObject(memoryStream);
            }
            else if (storageSerializationType == StorageSerializationType.Binary)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                informationObject = (IInformationObject) binaryFormatter.Deserialize(memoryStream);
            } else if (storageSerializationType == StorageSerializationType.ProtoBuf)
            {
                informationObject = (IInformationObject) memoryStream.DeserializeProtobuf(typeToRetrieve);
            }
            else // if(storageSerializationType == StorageSerializationType.Custom)
            {
                throw new NotSupportedException("Custom or unspecified formatting not supported");
            }
            return informationObject;
        }

        private static byte[] SerializeInformationObjectToBuffer(IInformationObject informationObject)
        {
            Type informationObjectType = informationObject.GetType();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] dataContent = null;
                StorageSerializationType storageSerializationType = getStorageSerializationType(informationObjectType);
                if (storageSerializationType == StorageSerializationType.XML)
                {
                    DataContractSerializer ser = new DataContractSerializer(informationObjectType);
                    using (
                        XmlTextWriter writer = new XmlTextWriter(memoryStream, Encoding.UTF8)
                            {
                                Formatting = Formatting.Indented
                            })
                    {
                        ser.WriteObject(writer, informationObject);
                        writer.Flush();
                        dataContent = memoryStream.ToArray();
                    }
                }
                else if (storageSerializationType == StorageSerializationType.JSON)
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(informationObjectType);
                    serializer.WriteObject(memoryStream, informationObject);
                    dataContent = memoryStream.ToArray();
                } else if (storageSerializationType == StorageSerializationType.Binary)
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, informationObject);
                    dataContent = memoryStream.ToArray();
                } else if (storageSerializationType == StorageSerializationType.ProtoBuf)
                {
                    memoryStream.SerializeProtobuf(informationObject);
                    dataContent = memoryStream.ToArray();
                }
                else // if (storageSerializationType == StorageSerializationType.Custom)
                {
                    throw new NotSupportedException("Custom or unspecified formatting not supported");
                }
                return dataContent;
            }
        }



        private static StorageSerializationType getStorageSerializationType(Type type)
        {
            PropertyInfo propInfo = type.GetProperty("ClassStorageSerializationType",
                                                     BindingFlags.Public | BindingFlags.Static);
            if(propInfo == null)
                return StorageSerializationType.XML;
            var propValue = propInfo.GetValue(null);
            if(propValue == null)
                return StorageSerializationType.XML;
            return (StorageSerializationType) propValue;
        }

        public static string GetOwnerContentLocation(IContainerOwner owner, string blobAddress)
        {
            return BlobStorage.GetOwnerContentLocation(owner, blobAddress);
        }

        public static async Task<BlobStorageItem> UploadOwnerBlobBinaryA(IContainerOwner owner, string blobAddress, byte[] binaryContent, string contentInformationType = null)
        {
            return await CoreServices.GetCurrent<IStorageService>().UploadBlobDataA(owner, blobAddress, binaryContent);
        }


        [Obsolete("Use current owner (without parameter)", false)]
        public static CloudBlockBlob GetOwnerBlobReference(IContainerOwner containerOwner, string contentPath)
        {
            string blobAddress = GetOwnerContentLocation(containerOwner, contentPath);
            CloudBlockBlob blob = CurrActiveContainer.GetBlockBlobReference(blobAddress);
            return blob;
        }

        [Obsolete("obsolete", true)]
        public static CloudBlockBlob GetOwnerBlobReference(string blobPath)
        {
            string blobAddress = GetOwnerContentLocation(InformationContext.CurrentOwner, blobPath);
            CloudBlockBlob blob = CurrActiveContainer.GetBlockBlobReference(blobAddress);
            return blob;
        }

        public static async Task DeleteInformationObjectAsync(this IInformationObject informationObject, IContainerOwner owner = null)
        {
            var storageService = CoreServices.GetCurrent<IStorageService>();
            string relativeLocation = informationObject.RelativeLocation;
            if (owner != null)
                relativeLocation = storageService.GetOwnerContentLocation(owner, relativeLocation);
            await storageService.DeleteBlobA(relativeLocation);
            IAdditionalFormatProvider additionalFormatProvider = informationObject as IAdditionalFormatProvider;
            if (additionalFormatProvider != null)
            {
                foreach (var contentExtension in additionalFormatProvider.GetAdditionalFormatExtensions())
                {
                    string contentLocation = relativeLocation + "." + contentExtension;
                    await storageService.DeleteBlobA(contentLocation);
                }

            }
            IIndexedDocument iDoc = informationObject as IIndexedDocument;
            //TODO: Generic default view deletion
            //DefaultViewSupport.DeleteDefaultView(informationObject);
            informationObject.PostDeleteExecute(owner);
            InformationContext.Current.ObjectStoredNotification(informationObject, InformationContext.ObjectChangeType.D_Deleted);

        }


        [Obsolete("obsolete", true)]
        public static CloudBlockBlob GetBlob(string containerName, string blobAddress)
        {
            var container = CurrBlobClient.GetContainerReference(containerName);
            return (CloudBlockBlob) container.GetBlob(blobAddress);
        }

        [Obsolete("obsolete", true)]
        public static CloudBlockBlob GetBlob(this CloudBlobContainer container, string blobAddress, IContainerOwner owner = null)
        {
            string relativeLocation = blobAddress;
            if (owner != null)
                relativeLocation = GetOwnerContentLocation(owner, relativeLocation);
            CloudBlockBlob blob = container.GetBlockBlobReference(relativeLocation);
            return blob;
        }

        public static string GetAccountIDFromLocation(string referenceLocation)
        {
            if (referenceLocation.StartsWith("acc/") == false)
                throw new InvalidDataException("Referencelocation is not account owned: " + referenceLocation);
            return referenceLocation.Substring(4, GuidLength);
        }

        public static string GetGroupIDFromLocation(string referenceLocation)
        {
            if (referenceLocation.StartsWith("grp/") == false)
                throw new InvalidDataException("Referencelocation is not group owned: " + referenceLocation);
            return referenceLocation.Substring(4, GuidLength);
        }


        public static string GetContentRootLocation(string referenceLocation)
        {
            if(referenceLocation.StartsWith("acc/") == false && referenceLocation.StartsWith("grp/") == false && referenceLocation.StartsWith("dev/") == false)
                throw new InvalidDataException("Unable to determine root for reference: " + referenceLocation);
            //var contentRoot = referenceLocation.Substring(0, AccOrGrpPlusIDPathLength) + ContentFolderName + "/";
            var contentRoot = referenceLocation.Substring(0, AccOrGrpPlusIDPathLength);
            return contentRoot;
        }

        public static async Task<int> DeleteContentsFromOwnerAsync(IContainerOwner owner)
        {
            string referenceLocation = GetOwnerContentLocation(owner, ".");
            return await DeleteContentsFromOwnerAsync(referenceLocation);
        }

        public static async Task<int> DeleteContentsFromOwnerAsync(string referenceLocation)
        {
            var storageService = CoreServices.GetCurrent<IStorageService>();
            var owner = InformationContext.CurrentOwner;
            var blobInfos = await storageService.GetBlobItemsA(owner, referenceLocation);
            var deleteTasks = blobInfos
                .Select(blob => storageService.CombinePathForOwner(owner, blob.DirectoryName, blob.FileName))
                .Select(fullName => storageService.DeleteBlobA(fullName)).ToArray();
            await Task.WhenAll(deleteTasks);
            return deleteTasks.Length;
        }

        [Obsolete("error", true)]
        public static async Task<BlobResultSegment> ListBlobsWithPrefixAsync(this IContainerOwner owner, string prefix, 
            bool useFlatBlobListing = true, BlobContinuationToken continuationToken = null, bool withMetadata = false, bool allowNoOwner = false)
        {
            if(owner == null && !allowNoOwner)
                throw new ArgumentNullException("owner", "Owner can be null only if specified with flag");
            var usedPrefix = owner != null ? GetOwnerContentLocation(owner, prefix) : prefix;
            string searchRoot = CurrActiveContainer.Name + "/" + usedPrefix;
            return await CurrBlobClient.ListBlobsSegmentedAsync(searchRoot, useFlatBlobListing, withMetadata ? BlobListingDetails.Metadata : BlobListingDetails.None, null, continuationToken, null, null);
        }



        public static string GetOwnerRootAddress(IContainerOwner owner)
        {
            string ownerRootAddress = GetOwnerContentLocation(owner, "");
            return ownerRootAddress;
        }

        [Obsolete("obsolete", true)]
        public static async Task<int> DeleteEntireOwnerAsync(IContainerOwner owner)
        {
            if(owner == null)
                throw new ArgumentNullException("owner");
            string ownerRootAddress = GetOwnerRootAddress(owner);
            string storageLevelOwnerRootAddress = CurrActiveContainer.Name + "/" + ownerRootAddress;
            var blobs = await GetBlobItemsA(null, storageLevelOwnerRootAddress, true);
            var deleteTasks = blobs.Select(blob => BlobStorage.DeleteBlobA(blob.Name)).ToArray();
            await Task.WhenAll(deleteTasks);
            return deleteTasks.Length;
        }

        public static async Task<int> DeleteBlobsFromOwnerTargetA(IContainerOwner owner, string targetLocation)
        {
            var blobs = await GetBlobItemsA(owner, targetLocation);
            var deleteTasks = new List<Task>();
            int deletedCount = 0;
            foreach (var blob in blobs)
            {
                var deleteTask = BlobStorage.DeleteBlobA(blob.Name);
                deleteTasks.Add(deleteTask);
                deletedCount++;
            }
            await Task.WhenAll(deleteTasks);
            return deletedCount;
        }


        public static bool CanContainExternalMetadata(this CloudBlockBlob blob)
        {
            string blobInformationType = blob.GetBlobInformationType();
            if (blobInformationType == null)
                return false;
            if (blobInformationType == InformationType_GenericContentValue)
                return false;
            return true;
        }

        [Obsolete("obsolete", true)]
        public static bool IsStoredInActiveContainer(this CloudBlockBlob blob)
        {
            return blob.Container.Name == CurrActiveContainer.Name;
        }

        public static async Task DeleteBlobAsync(string blobPath)
        {
            var storageService = CoreServices.GetCurrent<IStorageService>();
            await storageService.DeleteBlobA(blobPath);
            InformationContext.AddStorageTransactionToCurrent();
        }

        public static async Task DeleteBlobsAsync(string[] blobPaths)
        {
            var owner = InformationContext.CurrentOwner;
            var storageService = CoreServices.GetCurrent<IStorageService>();
            var deletionTasks = blobPaths
                .Select(blobPath =>  storageService.GetOwnerContentLocation(owner, blobPath))
                .Select(blobLocation => storageService.DeleteBlobA(blobLocation)).ToArray();
            await Task.WhenAll(deletionTasks);
        }



        [Obsolete("obsolete", true)]
        public static string GetLocationParentDirectory(string location)
        {
            int lastIndexOfSeparator = location.LastIndexOf('/');
            int lastPositionToInclude = lastIndexOfSeparator + 1; // keep the separator
            return location.Substring(0, lastPositionToInclude);
        }

        [Obsolete("obsolete", true)]
        public static async Task<CloudBlockBlob> GetInformationObjectBlobWithProperties(IInformationObject informationObject)
        {
            CloudBlockBlob blob = CurrActiveContainer.GetBlob(informationObject.RelativeLocation);
            await blob.FetchAttributesAsync();
            InformationContext.AddStorageTransactionToCurrent();
            return blob;
        }

        private static BlobStorageItem getBlobStorageItem(CloudBlockBlob blob)
        {
            return new BlobStorageItem(blob.Name, blob.Properties.ContentMD5, blob.Properties.ETag, blob.Properties.Length,
                blob.Properties.LastModified.GetValueOrDefault().UtcDateTime);
        }

        public static async Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner owner, string directoryLocation, bool allowNoOwner = false)
        {
            var storageService = CoreServices.GetCurrent<IStorageService>();
            var result = await storageService.GetBlobItemsA(owner, directoryLocation);
            return result;
        }

        [Obsolete("error", true)]
        public static async Task<CloudBlockBlob[]> GetBlobsWithMetadataA(IContainerOwner owner, string prefix, bool allowNoOwner = false)
        {
            BlobContinuationToken continuationToken = null;
            List<CloudBlockBlob> cloudBlockBlobs = new List<CloudBlockBlob>();
            do
            {
                var blobListItems = await ListBlobsWithPrefixAsync(owner, prefix, true, continuationToken, true, allowNoOwner);
                var cloudBlobsToAdd = blobListItems.Results.Cast<CloudBlockBlob>();
                cloudBlockBlobs.AddRange(cloudBlobsToAdd);
                continuationToken = blobListItems.ContinuationToken;
            } while (continuationToken != null);
            return cloudBlockBlobs.ToArray();
        }

        public static void FixCurrentOwnerLocation(this IInformationObject informationObject)
        {
            string relativeLocation = informationObject.RelativeLocation;
            string strippedLocation = RemoveOwnerPrefixIfExists(relativeLocation);
            string fixedLocation = GetOwnerContentLocation(InformationContext.CurrentOwner, strippedLocation);
            informationObject.RelativeLocation = fixedLocation;
        }

        [Obsolete("error", true)]
        public static async Task<string> AcquireLogicalLockByCreatingBlobAsync(string lockLocation)
        {
            CloudBlockBlob blob = CurrActiveContainer.GetBlockBlobReference(lockLocation);
            DateTime created = DateTime.UtcNow;
            blob.Metadata.Add("LockCreated", created.ToString("s"));
            string blobContent = "LockCreated: " + created.ToString("s");
            var accessCondition = AccessCondition.GenerateIfNoneMatchCondition("*");
            string lockETag = null;
            try
            {
                Debug.WriteLine("Trying to aqruire lock: " + lockLocation);
                await blob.UploadTextAsync(blobContent, Encoding.UTF8, accessCondition,null, null);
                InformationContext.AddStorageTransactionToCurrent();
                Debug.WriteLine("Success!");
                lockETag = blob.Properties.ETag;
            }
            catch
            {
                Debug.WriteLine("FAILED!");
            }
            return lockETag;
        }

        [Obsolete("error", true)]
        public static async Task<bool> ReleaseLogicalLockByDeletingBlobAsync(string lockBlobFullPath, string eTag)
        {
            CloudBlockBlob blob = CurrActiveContainer.GetBlockBlobReference(lockBlobFullPath);
            try
            {
                Debug.WriteLine("Trying to release lock: " + lockBlobFullPath);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.None, eTag != null ? AccessCondition.GenerateIfMatchCondition(eTag) : null, null, null);
                InformationContext.AddStorageTransactionToCurrent();
                Debug.WriteLine("Success!");
            }
            catch
            {
                Debug.WriteLine("FAILED!");
                return false;
            }
            return true;

        }

        [Obsolete("obsolete", true)]
        public static async Task<bool> ReleaseLogicalLockByDeletingBlob(string lockLocation, string lockEtag)
        {
            CloudBlockBlob blob = CurrActiveContainer.GetBlockBlobReference(lockLocation);
            BlobRequestOptions options = new BlobRequestOptions
            {
                RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(3), 10)
            };
            var accessCondition = lockEtag != null ? AccessCondition.GenerateIfMatchCondition(lockEtag) : null;
            try
            {
                Debug.WriteLine("Trying to release lock: " + lockLocation);
                await blob.DeleteAsync(DeleteSnapshotsOption.None, accessCondition, options, null);
                InformationContext.AddStorageTransactionToCurrent();
                Debug.WriteLine("Success!");
            }
            catch
            {
                Debug.WriteLine("FAILED!");
                return false;
            }
            return true;
        }

        public static string RemoveOwnerPrefixIfExists(string contentLocation)
        {
            if (contentLocation.StartsWith("acc/") || contentLocation.StartsWith("grp/"))
                return contentLocation.Substring(AccOrGrpPlusIDPathLength);
            return contentLocation;
        }

        private const string LockPath = "TheBall.Core/Locks/";

        [Obsolete("error", true)]
        public static async Task<string> TryClaimLockForOwnerAsync(IContainerOwner owner, string ownerLockFileName, string lockFileContent)
        {
            string lockFileName = LockPath + ownerLockFileName;
            var lockETag = await createLockFileWithContentAsync(owner, lockFileName, lockFileContent, true);
            return lockETag;
        }

        private static async Task deleteLockFileAsync(IContainerOwner owner, string lockFileName, string lockID)
        {
            var blob = GetOwnerBlobReference(owner, lockFileName);
            await blob.DeleteAsync(DeleteSnapshotsOption.None, AccessCondition.GenerateIfMatchCondition(lockID), null, null);
            InformationContext.AddStorageTransactionToCurrent();
        }

        [Obsolete("error", true)]
        private static async Task<string> createLockFileWithContentAsync(IContainerOwner owner, string lockFileName, string lockFileContent, bool requireUnclaimedLock)
        {
            var blob = GetOwnerBlobReference(owner, lockFileName);
            try
            {
                await blob.UploadBlobTextAsync(lockFileContent, requireUnclaimedLock);
            }
            catch (StorageException exception)
            {
                if (exception.RequestInformation.ExtendedErrorInformation.ErrorCode == StorageErrorCodeStrings.ResourceAlreadyExists)
                    return null;
            }
            InformationContext.AddStorageTransactionToCurrent();
            var lockETag = blob.Properties.ETag;
            return lockETag;
        }

        [Obsolete("error", true)]
        public static async Task ReplicateClaimedLockAsync(IContainerOwner owner, string ownerLockFileName, string lockFileContent)
        {
            await createLockFileWithContentAsync(owner, ownerLockFileName, lockFileContent, false);
        }

        public static async Task ReleaseLockForOwner(IContainerOwner owner, string ownerLockFileName, string lockID = null)
        {
            await deleteLockFileAsync(owner, ownerLockFileName, lockID);
        }

        internal static Task<IInformationObject[]> RetrieveInformationObjectsAsync(string itemDirectory, Type type)
        {
            throw new NotImplementedException();
        }

        public static async Task CopyBlobBetweenOwnersA(IContainerOwner sourceOwner, string sourceItemName, IContainerOwner targetOwner, string targetItemName)
        {
            var sourceBlob = GetOwnerBlobReference(sourceOwner, sourceItemName);
            var targetBlob = GetOwnerBlobReference(targetOwner, targetItemName);
            await targetBlob.StartCopyAsync(sourceBlob);
        }

    }

    public class ReferenceOutdatedException : Exception
    {
        private IInformationObject containerObject;
        private IInformationObject referenceInstance;
        private IInformationObject masterInstance;

        public ReferenceOutdatedException(IInformationObject containerObject, IInformationObject referenceInstance, IInformationObject masterInstance)
        {
            // TODO: Complete member initialization
            this.containerObject = containerObject;
            this.referenceInstance = referenceInstance;
            this.masterInstance = masterInstance;
        }
    }
}
