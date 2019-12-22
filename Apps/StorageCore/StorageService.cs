
using System.IO;
using System.Threading.Tasks;
using TheBall.Core.Storage;

namespace TheBall.Core.StorageCore
{
    public class StorageService : IStorageService
    {
        public IStorageService ServiceProvider;

        public static StorageService PlatformService { get; private set; }

        public void InitializeAsPlatformService()
        {
            PlatformService = this;
        }

        public StorageService(IStorageService serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }


        public string GetOwnerContentLocation(IContainerOwner owner, string location)
        {
            return ServiceProvider.GetOwnerContentLocation(owner, location);
        }

        public string CombinePathForOwner(IContainerOwner owner, string[] pathComponents)
        {
            return ServiceProvider.CombinePathForOwner(owner, pathComponents);
        }

        public async Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner owner, string locationPath)
        {
            return await ServiceProvider.GetBlobItemsA(owner, locationPath);
        }

        public async Task<BlobStorageItem> GetBlobItemA(IContainerOwner owner, string blobPath)
        {
            return await ServiceProvider.GetBlobItemA(owner, blobPath);
        }

        public async Task<BlobStorageItem> UploadBlobTextA(IContainerOwner owner, string blobPath, string text, string eTag = null)
        {
            return await ServiceProvider.UploadBlobTextA(owner, blobPath, text, eTag);
        }

        public async Task DeleteBlobA(string blobPath, string eTag = null)
        {
            await ServiceProvider.DeleteBlobA(blobPath, eTag);
        }

        public async Task<byte[]> DownloadBlobDataA(IContainerOwner owner, string blobPath, bool returnNullIfMissing,
            string eTag = null)
        {
            return await ServiceProvider.DownloadBlobDataA(owner, blobPath, returnNullIfMissing, eTag);
        }

        public async Task DownloadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, bool returnNullIfMissing,
            string eTag = null)
        {
            await ServiceProvider.DownloadBlobStreamA(owner, blobPath, stream, returnNullIfMissing, eTag);
        }

        public async Task<string[]> GetLocationFoldersA(IContainerOwner owner, string locationPath)
        {
            return await ServiceProvider.GetLocationFoldersA(owner, locationPath);
        }

        public async Task<BlobStorageItem> UploadBlobDataA(IContainerOwner owner, string blobPath, byte[] data, string eTag = null)
        {
            return await ServiceProvider.UploadBlobDataA(owner, blobPath, data, eTag);
        }
        public async Task<BlobStorageItem> UploadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, string eTag = null)
        {
            return await ServiceProvider.UploadBlobStreamA(owner, blobPath, stream, eTag);
        }

    }
}