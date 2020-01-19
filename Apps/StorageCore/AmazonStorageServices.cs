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

        private AmazonStorageServices()
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
                    new BlobStorageItem(item.Key, item.ETag, item.ETag, item.Size, item.LastModified));
                result.AddRange(blobStorageItemLQ);
            } while (response.IsTruncated);
            return result.ToArray();
        }

        public async Task<BlobStorageItem> GetBlobItemAFunc(IContainerOwner owner, string blobPath)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var response = await BlobClient.GetObjectMetadataAsync(BucketName, blobAddress);
            var result = new BlobStorageItem(blobAddress, response.ETag, response.ETag, response.ContentLength, response.LastModified);
            return result;
        }

        public async Task DeleteBlobAFunc(string blobPath, string eTag = null)
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

        public async Task<byte[]> DownloadBlobDataAFunc(IContainerOwner owner, string blobPath, bool returnNullIfMissing,
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

        public async Task DownloadBlobStreamAFunc(IContainerOwner owner, string blobPath, Stream stream, bool returnNullIfMissing, string eTag = null)
        {
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            var response = await BlobClient.GetObjectAsync(BucketName, blobAddress);
            await response.ResponseStream.CopyToAsync(stream);
        }

        public async Task<string[]> GetLocationFoldersAFunc(IContainerOwner owner, string locationPath)
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

        public async Task<BlobStorageItem> UploadBlobDataAFunc(IContainerOwner owner, string blobPath, byte[] data, string eTag = null)
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

        public async Task<BlobStorageItem> UploadBlobTextAFunc(IContainerOwner owner, string blobPath, string text, string eTag = null)
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

        public async Task<BlobStorageItem> UploadBlobStreamAFunc(IContainerOwner owner, string blobPath, Stream stream, string eTag = null)
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
    }
}