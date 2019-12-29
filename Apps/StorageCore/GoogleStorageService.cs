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

        public async Task<BlobStorageItem> GetBlobItemA(IContainerOwner owner, string blobPath)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var blob = await BlobClient.GetObjectAsync(BlobContainer.Name, blobAddress);
            var result = new BlobStorageItem(blob.Name, blob.Md5Hash, blob.ETag, (long) blob.Size.GetValueOrDefault(), blob.Updated);
            return result;
        }

        public async Task DeleteBlobA(string blobPath, string eTag)
        {
            var blobObject = new Object()
            {
                Bucket = BlobContainer.Name,
                Name = blobPath,
                ETag = eTag
            };
            await BlobClient.DeleteObjectAsync(blobObject);
        }

        public async Task<byte[]> DownloadBlobDataA(IContainerOwner owner, string blobPath, bool returnNullIfMissing,
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

        public async Task DownloadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, bool returnNullIfMissing,
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

        public async Task<string[]> GetLocationFoldersA(IContainerOwner owner, string locationPath)
        {
            throw new System.NotImplementedException();
        }

        public async Task<BlobStorageItem> UploadBlobDataA(IContainerOwner owner, string blobPath, byte[] data, string eTag = null)
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

        public async Task<BlobStorageItem> UploadBlobTextA(IContainerOwner owner, string blobPath, string text, string eTag = null)
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

        public async Task<BlobStorageItem> UploadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, string eTag = null)
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

        public async Task<string> AcquireLogicalLockByCreatingBlobAsync(string lockLocation)
        {
            throw new System.NotImplementedException();
        }

        public async Task ReleaseLogicalLockByDeletingBlobAsync(string lockLocation, string lockEtag)
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> TryClaimLockForOwnerAsync(IContainerOwner owner, string ownerLockFileName, string lockFileContent)
        {
            throw new System.NotImplementedException();
        }

        public async Task ReplicateClaimedLockAsync(IContainerOwner owner, string ownerLockFileName, string lockFileContent)
        {
            throw new System.NotImplementedException();
        }
    }
}