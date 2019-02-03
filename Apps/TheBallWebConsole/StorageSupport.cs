using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage.File;
using Newtonsoft.Json;

namespace TheBall.CORE.Storage
{
    public static class JSONSupport
    {
        public static T GetObjectFromData<T>(byte[] data)
        {
            using (var memStream = new MemoryStream(data))
            {
                return GetObjectFromStream<T>(memStream);
            }
        }

        public static T GetObjectFromStream<T>(Stream stream)
        {
            T result = (T)GetObjectFromStream(stream, typeof(T));
            return result;
        }

        public static object GetObjectFromStream(Stream stream, Type objectType)
        {
            var serializer = new JsonSerializer();
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                var result = serializer.Deserialize(streamReader, objectType);
                return result;
            }
            //DataContractJsonSerializer serializer = new DataContractJsonSerializer(objectType);
            //return serializer.ReadObject(stream);
        }

        public static void SerializeToJSONStream(object obj, Stream stream)
        {
            var serializer = new JsonSerializer();
            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
            {
                serializer.Serialize(streamWriter, obj);
            }
        }


        public static byte[] SerializeToJSONData(object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                SerializeToJSONStream(obj, memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    public interface IContainerOwner
    {
        string ContainerName { get; }
        string LocationPrefix { get; }
    }

    public static class Extensions
    {
        public static bool IsNoOwner(this IContainerOwner owner)
        {
            return owner.ContainerName == null && owner.LocationPrefix == null;
        }
        public static bool IsSystemOwner(this IContainerOwner owner)
        {
            return false;
        }

        public static string GetOwnerContentLocation(this IContainerOwner owner, string path)
        {
            if(!owner.IsNoOwner())
                throw new NotSupportedException("Only NoOwner supported");
            return path;
        }
    }

    public class InformationContext
    {
        public class NoOwner : IContainerOwner
        {
            public string ContainerName => null;
            public string LocationPrefix => null;
        }
        private static IContainerOwner currentOwner = new NoOwner();
        public static IContainerOwner CurrentOwner => currentOwner;
    }

    public static class StorageExt
    {
        public static BlobStorageItem ToBlobStorageItem(this ICloudBlob blob)
        {
            return new BlobStorageItem(blob.Name, blob.Properties.ContentMD5, blob.Properties.Length,
                blob.Properties.LastModified.GetValueOrDefault().UtcDateTime);
        }
    }

    public static class StorageSupport
    {
        internal static CloudBlobClient CloudClient;
        internal static CloudStorageAccount CloudAccount;
        internal static CloudBlobContainer CloudContainer;
        internal static CloudFileClient CloudFileClient;
        public static void InitializeStorage(string storageConnectionString, string containerName)
        {
            CloudAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudClient = CloudAccount.CreateCloudBlobClient();
            CloudContainer = CloudClient.GetContainerReference(containerName);
        }

        public static void InitializeFileStorage(string baseUri, string sasToken)
        {
            CloudFileClient = new CloudFileClient(new Uri(baseUri), new StorageCredentials(sasToken));
        }

        public static async Task<BlobStorageItem> GetBlobStorageItem(string name, IContainerOwner owner)
        {
            var fullPath = owner.CombinePathForOwner(name);
            var blob = await CloudContainer.GetBlobReferenceFromServerAsync(fullPath);
            return blob.ToBlobStorageItem();
        }

        public static async Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner owner, string directoryLocation, bool allowNoOwner = false)
        {
            List<BlobStorageItem> storageItems = new List<BlobStorageItem>();
            var cloudBlockBlobs = await GetBlobsWithMetadataA(owner, directoryLocation, allowNoOwner);
            var storageItemsToAddLQ = cloudBlockBlobs.Select(blob => blob.ToBlobStorageItem());
            storageItems.AddRange(storageItemsToAddLQ);
            return storageItems.ToArray();
        }

        public static async Task<BlobStorageItem> UploadOwnerBlobBinaryA(IContainerOwner currentOwner, string name, byte[] data)
        {
            var fullName = currentOwner.CombinePathForOwner(name);
            var rootContainer = CloudClient.GetRootContainerReference();
            var blob = (CloudBlockBlob) rootContainer.GetBlobReference(fullName);
            await blob.UploadFromByteArrayAsync(data, 0, data.Length);
            return blob.ToBlobStorageItem();
        }

        public static async Task<string[]> ListOwnerFoldersA(string rootFolder)
        {
            var owner = InformationContext.CurrentOwner;
            var fullPath = owner.CombinePathForOwner(rootFolder);
            BlobContinuationToken continuationToken = null;
            var result = new List<string>();
            do
            {
                var listingResult =
                    await
                        CloudContainer.ListBlobsSegmentedAsync(fullPath, true, BlobListingDetails.None, null,
                            continuationToken, null, null);
                continuationToken = listingResult.ContinuationToken;
                var foldersLQ = listingResult.Results.Where(item => item is CloudBlobDirectory).Cast<CloudBlobDirectory>().Select(item => item.Prefix);
                result.AddRange(foldersLQ);
            } while (continuationToken != null);
            return result.ToArray();
        }

        public static async Task<byte[]> DownloadBlobByteArrayAsync(string name, bool returnNullIfMissing, IContainerOwner owner)
        {
            var fullPath = owner.CombinePathForOwner(name);
            var blob = CloudContainer.GetBlockBlobReference(fullPath);
            using (var memStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(memStream);
                return memStream.ToArray();
            }
        }

        public static Task CopyBlobBetweenOwnersA(IContainerOwner sourceOwner, string sourceItemName, IContainerOwner targetOwner, string targetItemName)
        {
            throw new NotSupportedException("No multiple owners supported");
        }

        public static async Task DeleteBlobAsync(string name)
        {
            var owner = InformationContext.CurrentOwner;
            var fullName = owner.CombinePathForOwner(name);
            var blob = CloudContainer.GetBlockBlobReference(fullName);
            await blob.DeleteIfExistsAsync();
        }

        public static async Task UploadOwnerBlobTextAsync(IContainerOwner owner, string name, string textData)
        {
            var fullPath = owner.CombinePathForOwner(name);
            var blob = CloudContainer.GetBlockBlobReference(fullPath);
            await blob.UploadTextAsync(textData);
        }

        #region AzureSpecific
        public static async Task<CloudBlockBlob[]> GetBlobsWithMetadataA(IContainerOwner owner, string directoryLocation, bool allowNoOwner = false)
        {
            var fullPath = owner.CombinePathForOwner(directoryLocation);
            BlobContinuationToken continuationToken = null;
            List<CloudBlockBlob> cloudBlockBlobs = new List<CloudBlockBlob>();
            do
            {
                var blobListItems =
                    await
                        CloudClient.ListBlobsSegmentedAsync(fullPath, true, BlobListingDetails.Metadata, null, continuationToken, null, null);
                var cloudBlobsToAdd = blobListItems.Results.Cast<CloudBlockBlob>();
                cloudBlockBlobs.AddRange(cloudBlobsToAdd);
                continuationToken = blobListItems.ContinuationToken;
            } while (continuationToken != null);
            return cloudBlockBlobs.ToArray();
        }

        #endregion
    }
}