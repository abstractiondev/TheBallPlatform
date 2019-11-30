using System.Collections;
using System.Threading.Tasks;
using TheBall.Core.Storage;

namespace TheBall.Core.StorageCore
{
    public interface IStorageService : ICoreService
    {
        string GetOwnerContentLocation(IContainerOwner owner, string location);
        string CombinePathForOwner(IContainerOwner owner, params string[] pathComponents);
        Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner owner, string locationPath);
        Task<BlobStorageItem> GetBlobItemA(IContainerOwner owner, string blobPath);
        Task DeleteBlobA(string blobPath);
        Task<byte[]> DownloadBlobDataA(IContainerOwner owner, string blobPath, bool returnNullIfMissing);
        Task<string[]> GetLocationFoldersA(IContainerOwner owner, string locationPath);
        Task<BlobStorageItem> UploadBlobDataA(IContainerOwner owner, string blobPath, byte[] data);
        Task<BlobStorageItem> UploadBlobTextA(IContainerOwner owner, string blobPath, string data);
    }
}