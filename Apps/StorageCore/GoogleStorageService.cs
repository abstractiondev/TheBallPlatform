using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using TheBall.Core.Storage;

namespace TheBall.Core.StorageCore
{
    public class GoogleStorageService : IStorageService
    {
        private Bucket BlobContainer;
        private StorageClient BlobClient;
        public GoogleStorageService()
        {
            GetOwnerContentLocation = BlobStorage.GetOwnerContentLocationFunc;
            CombinePathForOwner = BlobStorage.CombinePathForOwnerFunc;
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


        public async Task<BlobStorageItem[]> GetBlobItemsAFunc(IContainerOwner owner, string locationPath)
        {
            const int batchSize = 100;
            var prefix = GetOwnerContentLocation(owner, locationPath);
            var listRequest = BlobClient.ListObjectsAsync(BlobContainer.Name, prefix);
            bool hasMoreData;
            var result = new List<BlobStorageItem>();
            do
            {
                var blobs = await listRequest.ReadPageAsync(batchSize);
                var blobItems = blobs.Select(blob =>
                        new BlobStorageItem(blob.Name, blob.Md5Hash, blob.ETag, (long) blob.Size.GetValueOrDefault(0),
                            blob.Updated))
                    .ToArray();
                result.AddRange(blobItems);
                hasMoreData = blobItems.Length >= batchSize;
            } while (hasMoreData);

            return result.ToArray();
        }

        public async Task<BlobStorageItem> GetBlobItemAFunc(IContainerOwner owner, string blobPath)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var blob = await BlobClient.GetObjectAsync(BlobContainer.Name, blobAddress);
            var result = new BlobStorageItem(blob.Name, blob.Md5Hash, blob.ETag, (long) blob.Size.GetValueOrDefault(), blob.Updated);
            return result;
        }

        public async Task DeleteBlobAFunc(string blobPath, string eTag)
        {
            var blobObject = new Object()
            {
                Bucket = BlobContainer.Name,
                Name = blobPath,
                ETag = eTag
            };
            await BlobClient.DeleteObjectAsync(blobObject);
        }

        public async Task<byte[]> DownloadBlobDataAFunc(IContainerOwner owner, string blobPath, bool returnNullIfMissing,
            string eTag = null)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            byte[] data;
            var blobObject = new Object()
            {
                Bucket = BlobContainer.Name,
                Name = blobAddress,
                ETag = eTag
            };
            using (var memoryStream = new MemoryStream())
            {
                await BlobClient.DownloadObjectAsync(blobObject, memoryStream, new DownloadObjectOptions() {});
                data = memoryStream.ToArray();
            }
            return data;
        }

        public async Task DownloadBlobStreamAFunc(IContainerOwner owner, string blobPath, Stream stream, bool returnNullIfMissing,
            string eTag = null)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var blobObject = new Object()
            {
                Bucket = BlobContainer.Name,
                Name = blobAddress,
                ETag = eTag
            };
            await BlobClient.DownloadObjectAsync(blobObject, stream, new DownloadObjectOptions() { });
        }

        public async Task<string[]> GetLocationFoldersAFunc(IContainerOwner owner, string locationPath)
        {
            throw new System.NotImplementedException();
        }

        public async Task<BlobStorageItem> UploadBlobDataAFunc(IContainerOwner owner, string blobPath, byte[] data, string eTag = null)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var contentType = BlobStorage.GetMimeType(blobPath);
            var blobObject = new Object()
            {
                Bucket = BlobContainer.Name,
                Name = blobAddress,
                ETag = eTag,
                ContentType = contentType,
            };
            using (var memoryStream = new MemoryStream(data))
            {
                await BlobClient.UploadObjectAsync(blobObject, memoryStream);
            }

            var result = await GetBlobItemA(owner, blobPath);
            return result;
        }

        public async Task<BlobStorageItem> UploadBlobTextAFunc(IContainerOwner owner, string blobPath, string text, string eTag = null)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var contentType = BlobStorage.GetMimeType(blobPath);
            var blobObject = new Object()
            {
                Bucket = BlobContainer.Name,
                Name = blobAddress,
                ETag = eTag,
                ContentType = contentType,
            };
            using (var memoryStream = new MemoryStream())
            using(var stream = new StreamWriter(memoryStream, Encoding.UTF8))
            {
                stream.Write(text);
                stream.Flush();
                memoryStream.Position = 0;
                await BlobClient.UploadObjectAsync(blobObject, memoryStream);
            }

            var result = await GetBlobItemA(owner, blobPath);
            return result;
        }

        public async Task<BlobStorageItem> UploadBlobStreamAFunc(IContainerOwner owner, string blobPath, Stream stream, string eTag = null)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var contentType = BlobStorage.GetMimeType(blobPath);
            var blobObject = new Object()
            {
                Bucket = BlobContainer.Name,
                Name = blobAddress,
                ETag = eTag,
                ContentType = contentType,
            };
            await BlobClient.UploadObjectAsync(blobObject, stream);
            var result = await GetBlobItemA(owner, blobPath);
            return result;
        }

    }
}