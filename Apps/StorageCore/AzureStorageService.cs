using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using TheBall.Core.Storage;

namespace TheBall.Core.StorageCore
{
    public class AzureStorageService : IStorageService
    {
        public CloudStorageAccount StorageAccount { get; set; }
        private CloudBlobClient BlobClient;
        private CloudBlobContainer BlobContainer;

        public AzureStorageService()
        {

        }


        private CloudBlockBlob getOwnerBlobReference(IContainerOwner owner, string blobPath)
        {
            string blobAddress = GetOwnerContentLocation(owner, blobPath);
            CloudBlockBlob blob = BlobContainer.GetBlockBlobReference(blobAddress);
            return blob;
        }

        public string GetOwnerContentLocation(IContainerOwner owner, string location)
        {
            var contentLocation = Path.Combine(owner.ContainerName, owner.LocationPrefix, location);
            var azureContentLocation = contentLocation.Replace("\\", "/");
            return azureContentLocation;
        }

        public string CombinePathForOwner(IContainerOwner owner, params string[] pathComponents)
        {
            var location = Path.Combine(pathComponents);
            var contentLocation = GetOwnerContentLocation(owner, location);
            return contentLocation;
        }

        public async Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner owner, string locationPath)
        {
            var ownerRootFolder = GetOwnerContentLocation(owner, locationPath);
            var searchRoot = BlobContainer.Name + "/" + ownerRootFolder;
            var result = new List<BlobStorageItem>();
            BlobContinuationToken continuationToken = null;
            do
            {
                var listingResult =
                    await
                        BlobClient.ListBlobsSegmentedAsync(searchRoot, true, BlobListingDetails.Metadata, null,
                            continuationToken, null, null);
                continuationToken = listingResult.ContinuationToken;
                var blobStorageItemLQ = listingResult.Results.Where(item => item is CloudBlob).Cast<CloudBlockBlob>().Select(item => 
                    new BlobStorageItem(item.Name, item.Properties.ContentMD5, item.Properties.Length, item.Properties.LastModified));
                result.AddRange(blobStorageItemLQ);
            } while (continuationToken != null);
            return result.ToArray();
        }

        public async Task<string[]> GetLocationFoldersA(IContainerOwner owner, string locationPath)
        {
            var ownerRootFolder = GetOwnerContentLocation(owner, locationPath);
            var searchRoot = BlobContainer.Name + "/" + ownerRootFolder;
            var result = new List<string>();
            BlobContinuationToken continuationToken = null;
            do
            {
                var listingResult =
                    await
                        BlobClient.ListBlobsSegmentedAsync(searchRoot, false, BlobListingDetails.Metadata, null,
                            continuationToken, null, null);
                continuationToken = listingResult.ContinuationToken;
                var foldersLQ = listingResult.Results.Where(item => item is CloudBlobDirectory).Cast<CloudBlobDirectory>().Select(item => item.Prefix);
                result.AddRange(foldersLQ);
            } while (continuationToken != null);
            return result.ToArray();
        }



        public async Task<BlobStorageItem> GetBlobItemA(IContainerOwner owner, string blobPath)
        {
            var blob = getOwnerBlobReference(owner, blobPath);
            await blob.FetchAttributesAsync();
            var blobStorageItem = new BlobStorageItem(blob.Name, blob.Properties.ContentMD5, blob.Properties.Length, blob.Properties.LastModified);
            return blobStorageItem;
        }

        public async Task<BlobStorageItem> UploadBlobTextA(IContainerOwner owner, string blobPath, string data)
        {
            var blob = getOwnerBlobReference(owner, blobPath);
            await blob.UploadTextAsync(data);
            return new BlobStorageItem(blob.Name, blob.Properties.ContentMD5, blob.Properties.Length, blob.Properties.LastModified);
        }

        public async Task DeleteBlobA(string blobPath)
        {
            var blob = BlobContainer.GetBlockBlobReference(blobPath);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<byte[]> DownloadBlobDataA(IContainerOwner owner, string blobPath, bool returnNullIfMissing)
        {
            var blob = getOwnerBlobReference(owner, blobPath);
            byte[] data = null;
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    await blob.DownloadToStreamAsync(memoryStream);
                    data = memoryStream.ToArray();
                }
                catch (StorageException scEx)
                {
                    if (returnNullIfMissing)
                    {
                        if (scEx.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound ||
                            scEx.RequestInformation.HttpStatusCode == (int)HttpStatusCode.BadRequest)
                            return null;
                    }
                    throw;
                }

            }
            return data;
        }

        public async Task<BlobStorageItem> UploadBlobDataA(IContainerOwner owner, string blobPath, byte[] data)
        {
            var blob = getOwnerBlobReference(owner, blobPath);
            await blob.UploadFromByteArrayAsync(data, 0, data.Length);
            return new BlobStorageItem(blob.Name, blob.Properties.ContentMD5, blob.Properties.Length, blob.Properties.LastModified);
        }
    }
}
