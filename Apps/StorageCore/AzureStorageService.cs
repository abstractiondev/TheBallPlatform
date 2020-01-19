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
            GetOwnerContentLocation = GetOwnerContentLocationFunc;
            CombinePathForOwner = CombinePathForOwnerFunc;
            GetBlobItemsA = GetBlobItemsAFunc;
            GetBlobItemA = GetBlobItemAFunc;
            DeleteBlobA = DeleteBlobAFunc;
            DownloadBlobDataA = DownloadBlobDataAFunc;
            DownloadBlobStreamA = DownloadBlobStreamAFunc;
            GetLocationFoldersA = GetLocationFoldersAFunc;
            UploadBlobDataA = UploadBlobDataAFunc;
            UploadBlobTextA = UploadBlobTextAFunc;
            UploadBlobStreamA = UploadBlobStreamAFunc;
        }

        public GetOwnerContentLocation GetOwnerContentLocation { get; }
        public CombinePathForOwner CombinePathForOwner { get; }
        public GetBlobItemsA GetBlobItemsA { get; }
        public GetBlobItemA GetBlobItemA { get; }
        public DeleteBlobA DeleteBlobA { get; }
        public DownloadBlobDataA DownloadBlobDataA { get; }
        public DownloadBlobStreamA DownloadBlobStreamA { get; }
        public GetLocationFoldersA GetLocationFoldersA { get; }
        public UploadBlobDataA UploadBlobDataA { get; }
        public UploadBlobTextA UploadBlobTextA { get; }
        public UploadBlobStreamA UploadBlobStreamA { get; }
        public AcquireLogicalLockByCreatingBlobAsync AcquireLogicalLockByCreatingBlobAsync { get; }
        public ReleaseLogicalLockByDeletingBlobAsync ReleaseLogicalLockByDeletingBlobAsync { get; }
        public TryClaimLockForOwnerAsync TryClaimLockForOwnerAsync { get; }
        public ReplicateClaimedLockAsync ReplicateClaimedLockAsync { get; }


        private CloudBlockBlob getOwnerBlobReference(IContainerOwner owner, string blobPath)
        {
            string blobAddress = GetOwnerContentLocation(owner, blobPath);
            CloudBlockBlob blob = BlobContainer.GetBlockBlobReference(blobAddress);
            return blob;
        }

        public string GetOwnerContentLocationFunc(IContainerOwner owner, string location)
        {
            var contentLocation = Path.Combine(owner.ContainerName, owner.LocationPrefix, location);
            contentLocation = contentLocation.Replace("\\", "/");
            return contentLocation;
        }

        public string CombinePathForOwnerFunc(IContainerOwner owner, params string[] pathComponents)
        {
            var location = Path.Combine(pathComponents);
            var contentLocation = GetOwnerContentLocation(owner, location);
            return contentLocation;
        }

        public async Task<BlobStorageItem[]> GetBlobItemsAFunc(IContainerOwner owner, string locationPath)
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

        public async Task<string[]> GetLocationFoldersAFunc(IContainerOwner owner, string locationPath)
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



        public async Task<BlobStorageItem> GetBlobItemAFunc(IContainerOwner owner, string blobPath)
        {
            var blob = getOwnerBlobReference(owner, blobPath);
            await blob.FetchAttributesAsync();
            var blobStorageItem = new BlobStorageItem(blob.Name, blob.Properties.ContentMD5, blob.Properties.ETag, blob.Properties.Length, blob.Properties.LastModified);
            return blobStorageItem;
        }

        public async Task<BlobStorageItem> UploadBlobTextAFunc(IContainerOwner owner, string blobPath, string text, string eTag = null)
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

        public async Task DeleteBlobAFunc(string blobPath, string eTag = null)
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

        public async Task<byte[]> DownloadBlobDataAFunc(IContainerOwner owner, string blobPath, bool returnNullIfMissing,
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

        public async Task DownloadBlobStreamAFunc(IContainerOwner owner, string blobPath, Stream stream, bool returnNullIfMissing, string eTag = null)
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


        public async Task<BlobStorageItem> UploadBlobDataAFunc(IContainerOwner owner, string blobPath, byte[] data, string eTag = null)
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

        public async Task<BlobStorageItem> UploadBlobStreamAFunc(IContainerOwner owner, string blobPath, Stream stream, string eTag = null)
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

    }
}
