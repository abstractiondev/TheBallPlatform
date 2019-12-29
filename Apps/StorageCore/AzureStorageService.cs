using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

        public BlobRequestOptions DefaultBlobRequestOptions { get; } =
            new BlobRequestOptions() {StoreBlobContentMD5 = true};


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
            contentLocation = contentLocation.Replace("\\", "/");
            return contentLocation;
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
                    new BlobStorageItem(item.Name, item.Properties.ContentMD5, item.Properties.ETag, item.Properties.Length, item.Properties.LastModified));
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
            var blobStorageItem = new BlobStorageItem(blob.Name, blob.Properties.ContentMD5, blob.Properties.ETag, blob.Properties.Length, blob.Properties.LastModified);
            return blobStorageItem;
        }

        public async Task<BlobStorageItem> UploadBlobTextA(IContainerOwner owner, string blobPath, string text, string eTag = null)
        {
            var blob = getOwnerBlobReference(owner, blobPath);
            try
            {
                await blob.UploadTextAsync(text, Encoding.UTF8,
                    eTag != null ? AccessCondition.GenerateIfMatchCondition(eTag) : null, DefaultBlobRequestOptions,
                    new OperationContext());
            }
            catch (StorageException scEx)
            {
                throw new TBStorageException((HttpStatusCode) scEx.RequestInformation.HttpStatusCode, scEx);
            }

            return new BlobStorageItem(blob.Name, blob.Properties.ContentMD5, blob.Properties.ETag, blob.Properties.Length, blob.Properties.LastModified);
        }

        public async Task DeleteBlobA(string blobPath, string eTag = null)
        {
            var blob = BlobContainer.GetBlockBlobReference(blobPath);
            try
            {
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.None,
                    eTag != null ? AccessCondition.GenerateIfMatchCondition(eTag) : null, DefaultBlobRequestOptions,
                    new OperationContext());
            }
            catch (StorageException scEx)
            {
                throw new TBStorageException((HttpStatusCode) scEx.RequestInformation.HttpStatusCode, scEx);
            }
        }

        public async Task<byte[]> DownloadBlobDataA(IContainerOwner owner, string blobPath, bool returnNullIfMissing,
            string eTag = null)
        {
            var blob = getOwnerBlobReference(owner, blobPath);
            byte[] data = null;
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    await blob.DownloadToStreamAsync(memoryStream, eTag != null ? AccessCondition.GenerateIfMatchCondition(eTag) : null, DefaultBlobRequestOptions, new OperationContext());
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

                    var statusCode = scEx.RequestInformation.HttpStatusCode;
                    throw new TBStorageException((HttpStatusCode) statusCode, scEx);
                }

            }
            return data;
        }

        public async Task DownloadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, bool returnNullIfMissing, string eTag = null)
        {
            var blob = getOwnerBlobReference(owner, blobPath);
            byte[] data = null;
            try
            {
                await blob.DownloadToStreamAsync(stream, eTag != null ? AccessCondition.GenerateIfMatchCondition(eTag) : null, DefaultBlobRequestOptions, new OperationContext());
            }
            catch (StorageException scEx)
            {
                if (returnNullIfMissing)
                {
                    if (scEx.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound ||
                        scEx.RequestInformation.HttpStatusCode == (int)HttpStatusCode.BadRequest)
                        return;
                }
                var statusCode = scEx.RequestInformation.HttpStatusCode;
                throw new TBStorageException((HttpStatusCode)statusCode, scEx);
            }
        }


        public async Task<BlobStorageItem> UploadBlobDataA(IContainerOwner owner, string blobPath, byte[] data, string eTag = null)
        {
            var blob = getOwnerBlobReference(owner, blobPath);
            try
            {
                await blob.UploadFromByteArrayAsync(data, 0, data.Length,
                    eTag != null ? AccessCondition.GenerateIfMatchCondition(eTag) : null, DefaultBlobRequestOptions, new OperationContext());
            }
            catch (StorageException scEx)
            {
                throw new TBStorageException((HttpStatusCode)scEx.RequestInformation.HttpStatusCode, scEx);
            }
            return new BlobStorageItem(blob.Name, blob.Properties.ContentMD5, blob.Properties.ETag, blob.Properties.Length, blob.Properties.LastModified);
        }

        public async Task<BlobStorageItem> UploadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, string eTag = null)
        {
            var blob = getOwnerBlobReference(owner, blobPath);
            try
            {
                await blob.UploadFromStreamAsync(stream, 
                    eTag != null ? AccessCondition.GenerateIfMatchCondition(eTag) : null, DefaultBlobRequestOptions, new OperationContext());
            }
            catch (StorageException scEx)
            {
                throw new TBStorageException((HttpStatusCode)scEx.RequestInformation.HttpStatusCode, scEx);
            }
            return new BlobStorageItem(blob.Name, blob.Properties.ContentMD5, blob.Properties.ETag, blob.Properties.Length, blob.Properties.LastModified);
        }

        public async Task<string> AcquireLogicalLockByCreatingBlobAsync(string lockLocation)
        {
            throw new NotImplementedException();
        }

        public async Task ReleaseLogicalLockByDeletingBlobAsync(string lockLocation, string lockEtag)
        {
            throw new NotImplementedException();
        }

        public async Task<string> TryClaimLockForOwnerAsync(IContainerOwner owner, string ownerLockFileName, string lockFileContent)
        {
            throw new NotImplementedException();
        }

        public async Task ReplicateClaimedLockAsync(IContainerOwner owner, string ownerLockFileName, string lockFileContent)
        {
            throw new NotImplementedException();
        }
    }
}
