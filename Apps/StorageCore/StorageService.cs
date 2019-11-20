
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

        public Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner owner, string contentTypeName)
        {
            throw new System.NotImplementedException();
        }
    }
}