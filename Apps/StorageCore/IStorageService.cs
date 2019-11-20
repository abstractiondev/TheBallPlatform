using System.Collections;
using System.Threading.Tasks;
using TheBall.Core.Storage;

namespace TheBall.Core.StorageCore
{
    public interface IStorageService : ICoreService
    {
        string GetOwnerContentLocation(IContainerOwner owner, string location);
        string CombinePathForOwner(IContainerOwner owner, params string[] pathComponents);
        Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner owner, string contentTypeName);
    }
}