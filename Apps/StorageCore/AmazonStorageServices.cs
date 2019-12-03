using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using TheBall.Core.Storage;

namespace TheBall.Core.StorageCore
{
    public class AmazonStorageServices : IStorageService
    {
        private RegionEndpoint BucketRegion = RegionEndpoint.EUNorth1;
        private string BucketName;
        private AmazonS3Client BlobClient;

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
            var searchRoot = ownerRootFolder;
            var result = new List<BlobStorageItem>();
            var request = new ListObjectsV2Request()
            {
                BucketName = BucketName,
                Prefix = searchRoot,
                MaxKeys = 100
            };
            ListObjectsV2Response response;
            do
            {
                response = await BlobClient.ListObjectsV2Async(request);
                var blobStorageItemLQ = response.S3Objects.Select(item =>
                    new BlobStorageItem(item.Key, item.ETag, item.Size, item.LastModified));
                result.AddRange(blobStorageItemLQ);
            } while (response.IsTruncated);
            return result.ToArray();
        }

        public async Task<BlobStorageItem> GetBlobItemA(IContainerOwner owner, string blobPath)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var response = await BlobClient.GetObjectAsync(BucketName, blobAddress);
            var result = new BlobStorageItem(response.Key, response.ETag, response.ContentLength, response.LastModified);
            return result;
        }

        public async Task DeleteBlobA(string blobPath)
        {
            await BlobClient.DeleteObjectAsync(BucketName, blobPath);
        }

        public async Task<byte[]> DownloadBlobDataA(IContainerOwner owner, string blobPath, bool returnNullIfMissing)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var response = await BlobClient.GetObjectAsync(BucketName, blobAddress);
            byte[] data;
            using (var memoryStream = new MemoryStream())
            {
                await response.ResponseStream.CopyToAsync(memoryStream);
                data = memoryStream.ToArray();
            }
            return data;
        }

        public async Task<string[]> GetLocationFoldersA(IContainerOwner owner, string locationPath)
        {
            var ownerRootFolder = GetOwnerContentLocation(owner, locationPath);
            var searchRoot = ownerRootFolder;
            var result = new List<string>();
            var request = new ListObjectsV2Request()
            {
                BucketName = BucketName,
                Prefix = searchRoot,
                Delimiter = "/",
                MaxKeys = 100
            };
            ListObjectsV2Response response;
            do
            {
                response = await BlobClient.ListObjectsV2Async(request);
                result.AddRange(response.CommonPrefixes);
            } while (response.IsTruncated);
            return result.ToArray();
        }

        public async Task<BlobStorageItem> UploadBlobDataA(IContainerOwner owner, string blobPath, byte[] data)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var req = new PutObjectRequest
            {
                Key = blobAddress,
                BucketName = BucketName
            };
            PutObjectResponse response;
            using (var memoryStream = new MemoryStream(data))
            {
                req.InputStream = memoryStream;
                response = await BlobClient.PutObjectAsync(req);
            }

            var result = await GetBlobItemA(owner, blobPath);
            return result;
        }

        public async Task<BlobStorageItem> UploadBlobTextA(IContainerOwner owner, string blobPath, string text)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var req = new PutObjectRequest
            {
                Key = blobAddress,
                BucketName = BucketName
            };
            PutObjectResponse response;
            using (var memoryStream = new MemoryStream())
            using (var stream = new StreamWriter(memoryStream, Encoding.UTF8))
            {
                stream.Write(text);
                stream.Flush();
                memoryStream.Position = 0;
                req.InputStream = memoryStream;
                response = await BlobClient.PutObjectAsync(req);
            }

            var result = await GetBlobItemA(owner, blobPath);
            return result;
        }
    }
}