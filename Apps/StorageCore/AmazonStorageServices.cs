using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Azure.Storage;
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
            var response = await BlobClient.GetObjectMetadataAsync(BucketName, blobAddress);
            var result = new BlobStorageItem(blobAddress, response.ETag, response.ContentLength, response.LastModified);
            return result;
        }

        public async Task DeleteBlobA(string blobPath, string eTag = null)
        {
            //var blobObject = new 
            var getObjectRequest = new GetObjectRequest()
            {
                BucketName = BucketName,
                Key =  blobPath,
                EtagToMatch = eTag,
            };
            var blob = await BlobClient.GetObjectAsync(getObjectRequest);
            if (blob.HttpStatusCode == HttpStatusCode.PreconditionFailed)
                throw new TBStorageException(HttpStatusCode.PreconditionFailed);
            await BlobClient.DeleteObjectAsync(BucketName, blobPath);
        }

        public async Task<byte[]> DownloadBlobDataA(IContainerOwner owner, string blobPath, bool returnNullIfMissing,
            string eTag = null)
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

        public async Task DownloadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, bool returnNullIfMissing, string eTag = null)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var response = await BlobClient.GetObjectAsync(BucketName, blobAddress);
            await response.ResponseStream.CopyToAsync(stream);
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

        public async Task<BlobStorageItem> UploadBlobDataA(IContainerOwner owner, string blobPath, byte[] data, string eTag = null)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var req = new PutObjectRequest
            {
                Key = blobAddress,
                BucketName = BucketName,
                
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

        public async Task<BlobStorageItem> UploadBlobTextA(IContainerOwner owner, string blobPath, string text, string eTag = null)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var req = new PutObjectRequest
            {
                Key = blobAddress,
                BucketName = BucketName, 
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

        public async Task<BlobStorageItem> UploadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, string eTag = null)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var req = new PutObjectRequest
            {
                Key = blobAddress,
                BucketName = BucketName,
            };
            PutObjectResponse response;
            req.InputStream = stream;
            response = await BlobClient.PutObjectAsync(req);

            var result = await GetBlobItemA(owner, blobPath);
            return result;
        }
    }
}