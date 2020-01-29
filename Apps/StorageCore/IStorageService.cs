using System.Collections;
using System.IO;
using System.Threading.Tasks;
using TheBall.Core.Storage;

namespace TheBall.Core.StorageCore
{
    public delegate Task InitializeService();
    public delegate string GetOwnerContentLocation(IContainerOwner owner, string location);
    public delegate string CombinePathForOwner(IContainerOwner owner, params string[] pathComponents);
    public delegate Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner owner, string locationPath);
    public delegate Task<BlobStorageItem> GetBlobItemA(IContainerOwner owner, string blobPath);
    public delegate Task DeleteBlobA(string blobPath, string eTag = null);
    public delegate Task<byte[]> DownloadBlobDataA(IContainerOwner owner, string blobPath, bool returnNullIfMissing, string eTag = null);
    public delegate Task DownloadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, bool returnNullIfMissing, string eTag = null);
    public delegate Task<string[]> GetLocationFoldersA(IContainerOwner owner, string locationPath);
    public delegate Task<BlobStorageItem> UploadBlobDataA(IContainerOwner owner, string blobPath, byte[] data, string eTag = null);
    public delegate Task<BlobStorageItem> UploadBlobTextA(IContainerOwner owner, string blobPath, string text, string eTag = null);
    public delegate Task<BlobStorageItem> UploadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, string eTag = null);
    public delegate Task<string> AcquireLogicalLockByCreatingBlobAsync(string lockLocation);
    public delegate Task ReleaseLogicalLockByDeletingBlobAsync(string lockLocation, string lockEtag);
    public delegate Task<string> TryClaimLockForOwnerAsync(IContainerOwner owner, string ownerLockFileName, string lockFileContent);
    public delegate Task ReplicateClaimedLockAsync(IContainerOwner owner, string ownerLockFileName, string lockFileContent);

    public interface IStorageService : ICoreService
    {
        InitializeService InitializeService { get; }
        GetOwnerContentLocation GetOwnerContentLocation { get; }
        CombinePathForOwner CombinePathForOwner { get; }
        GetBlobItemsA GetBlobItemsA { get; }
        GetBlobItemA GetBlobItemA { get; }
        DeleteBlobA DeleteBlobA { get; }
        DownloadBlobDataA DownloadBlobDataA { get; }
        DownloadBlobStreamA DownloadBlobStreamA { get; }
        GetLocationFoldersA GetLocationFoldersA { get; }
        UploadBlobDataA UploadBlobDataA { get; }
        UploadBlobTextA UploadBlobTextA { get; }
        UploadBlobStreamA UploadBlobStreamA { get; }
        AcquireLogicalLockByCreatingBlobAsync AcquireLogicalLockByCreatingBlobAsync { get; }
        ReleaseLogicalLockByDeletingBlobAsync ReleaseLogicalLockByDeletingBlobAsync { get; }
        TryClaimLockForOwnerAsync TryClaimLockForOwnerAsync { get; }
        ReplicateClaimedLockAsync ReplicateClaimedLockAsync { get; }
    }
}