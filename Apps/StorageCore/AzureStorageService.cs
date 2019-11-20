using System.IO;
using System.Threading.Tasks;
using TheBall.Core.Storage;

namespace TheBall.Core.StorageCore
{
    public class AzureStorageService : IStorageService
    {
        public string GetOwnerContentLocation(IContainerOwner owner, string location)
        {
            var contentLocation = Path.Combine(owner.ContainerName, owner.LocationPrefix, location);
            var azureContentLocation = contentLocation.Replace("\\", "/");
            return azureContentLocation;
        }

        public string CombinePathForOwner(IContainerOwner owner, params string[] pathComponents)
        {
            var location = Path.Combine(pathComponents);
            var contentLocation = GetOwnerContentLocation(owner, location);
            return contentLocation;
        }

        public Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner owner, string contentTypeName)
        {
            throw new System.NotImplementedException();
        }
    }
}
