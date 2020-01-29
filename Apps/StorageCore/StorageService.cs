
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

        public InitializeService InitializeService => ServiceProvider.InitializeService;
        public GetOwnerContentLocation GetOwnerContentLocation => ServiceProvider.GetOwnerContentLocation;

        public CombinePathForOwner CombinePathForOwner => ServiceProvider.CombinePathForOwner;

        public GetBlobItemsA GetBlobItemsA => ServiceProvider.GetBlobItemsA;

        public GetBlobItemA GetBlobItemA => ServiceProvider.GetBlobItemA;

        public DeleteBlobA DeleteBlobA => ServiceProvider.DeleteBlobA;

        public DownloadBlobDataA DownloadBlobDataA => ServiceProvider.DownloadBlobDataA;

        public DownloadBlobStreamA DownloadBlobStreamA => ServiceProvider.DownloadBlobStreamA;

        public GetLocationFoldersA GetLocationFoldersA => ServiceProvider.GetLocationFoldersA;

        public UploadBlobDataA UploadBlobDataA => ServiceProvider.UploadBlobDataA;

        public UploadBlobTextA UploadBlobTextA => ServiceProvider.UploadBlobTextA;

        public UploadBlobStreamA UploadBlobStreamA => ServiceProvider.UploadBlobStreamA;

        public AcquireLogicalLockByCreatingBlobAsync AcquireLogicalLockByCreatingBlobAsync => ServiceProvider.AcquireLogicalLockByCreatingBlobAsync;

        public ReleaseLogicalLockByDeletingBlobAsync ReleaseLogicalLockByDeletingBlobAsync => ServiceProvider.ReleaseLogicalLockByDeletingBlobAsync;

        public TryClaimLockForOwnerAsync TryClaimLockForOwnerAsync => ServiceProvider.TryClaimLockForOwnerAsync;

        public ReplicateClaimedLockAsync ReplicateClaimedLockAsync => ServiceProvider.ReplicateClaimedLockAsync;
    }
}