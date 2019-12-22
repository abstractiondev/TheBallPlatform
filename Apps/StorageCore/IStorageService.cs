using System.Collections;
using System.IO;
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
        Task DeleteBlobA(string blobPath, string eTag = null);
        Task<byte[]> DownloadBlobDataA(IContainerOwner owner, string blobPath, bool returnNullIfMissing,
            string eTag = null);
        Task DownloadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, bool returnNullIfMissing, string eTag = null);
        Task<string[]> GetLocationFoldersA(IContainerOwner owner, string locationPath);
        Task<BlobStorageItem> UploadBlobDataA(IContainerOwner owner, string blobPath, byte[] data, string eTag = null);
        Task<BlobStorageItem> UploadBlobTextA(IContainerOwner owner, string blobPath, string text, string eTag = null);
        Task<BlobStorageItem> UploadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, string eTag = null);
    }
}