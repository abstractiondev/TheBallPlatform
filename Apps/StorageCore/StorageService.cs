
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

        public Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner owner, string locationPath)
        {
            return ServiceProvider.GetBlobItemsA(owner, locationPath);
        }

        public async Task<BlobStorageItem> GetBlobItemA(IContainerOwner owner, string blobPath)
        {
            return await ServiceProvider.GetBlobItemA(owner, blobPath);
        }

        public async Task<BlobStorageItem> UploadBlobTextA(IContainerOwner owner, string blobPath, string text)
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteBlobA(string blobPath)
        {
            throw new System.NotImplementedException();
        }

        public async Task<byte[]> DownloadBlobDataA(IContainerOwner owner, string blobPath, bool returnNullIfMissing)
        {
            throw new System.NotImplementedException();
        }

        public async Task<string[]> GetLocationFoldersA(IContainerOwner owner, string locationPath)
        {
            throw new System.NotImplementedException();
        }

        public async Task<BlobStorageItem> UploadBlobDataA(IContainerOwner owner, string blobPath, byte[] data)
        {
            throw new System.NotImplementedException();
        }
    }
}