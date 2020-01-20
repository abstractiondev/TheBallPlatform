using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api.Common;
using TheBall.Core.Storage;

namespace TheBall.Core.StorageCore
{
    public class FileSystemStorageService : IStorageService
    {
        private const char PlatformStoragePathChar = '/';
        private char PhysicalStoragePathChar = Path.PathSeparator;

        private const string RequiredSubString = "TheBallData";
        public readonly string LogicalRootPath;
        public FileSystemStorageService(string logicalRootPath) : base()
        {
            if(!logicalRootPath.Contains(RequiredSubString))
                throw new ArgumentException($"FileSystemStorage requires {RequiredSubString} to be present in the path");
            if(!logicalRootPath.EndsWith("/"))
                throw new ArgumentException("FileSystemStorage logical root path needs to end with /");
            LogicalRootPath = logicalRootPath;
        }

        private string convertLogicalToStoragePath(string logicalPath)
        {
            if (logicalPath.StartsWith(LogicalRootPath))
                throw new ArgumentException("Blob path already contains root path");
            return Path.Combine(LogicalRootPath, logicalPath);
        }

        private string convertStorageToLogicalPath(string storagePath)
        {
            if(!storagePath.StartsWith(LogicalRootPath))
                throw new ArgumentException("Blob path does not start with root path");
            var logicalPath = storagePath.Substring(LogicalRootPath.Length);
            return logicalPath;
        }

        private string convertToPhysicalStoragePath(string platformPath)
        {
            return platformPath?.Replace(PlatformStoragePathChar, PhysicalStoragePathChar);
        }

        private string convertFromPhysicalStoragePath(string physicalPath)
        {
            return physicalPath?.Replace(PhysicalStoragePathChar, PlatformStoragePathChar);
        }

        private void verifyRootPathRemaining(string path)
        {
            var combinedPath = Path.Combine(LogicalRootPath, path);
            DirectoryInfo fullDir = new DirectoryInfo(combinedPath);
            var fullDirPath = fullDir.FullName;
            bool startsWith = fullDirPath.StartsWith(LogicalRootPath, StringComparison.OrdinalIgnoreCase);
            if(!startsWith)
                throw new ArgumentException($"Path is invalid: {path}");
        }

        private FileSystemStorageService()
        {
            GetOwnerContentLocation = BlobStorage.GetOwnerContentLocationFunc;
            CombinePathForOwner = BlobStorage.CombinePathForOwnerFunc;
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

        public GetOwnerContentLocation GetOwnerContentLocation { get;  }
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

        public async Task<BlobStorageItem[]> GetBlobItemsAFunc(IContainerOwner owner, string locationPath)
        {
            var folderLocation = GetOwnerContentLocation(owner, locationPath);
            var directoryInfo = new DirectoryInfo(folderLocation);
            var fileInfos = directoryInfo.EnumerateFileSystemInfos("*", SearchOption.AllDirectories);
            var blobItems = fileInfos.AsParallel()
                .Where(fsItem => fsItem is FileInfo)
                .Cast<FileInfo>()
                .Select(fs => new BlobStorageItem(fs.FullName, null, null, fs.Length, fs.LastWriteTimeUtc))
                .OrderBy(item => item.Name)
                .ToArray();
            return blobItems;
        }

        public async Task<BlobStorageItem> GetBlobItemAFunc(IContainerOwner owner, string blobPath)
        {
            var fileLocation = GetOwnerContentLocation(owner, blobPath);
            var fileInfo = new FileInfo(fileLocation);
            var result = new BlobStorageItem(fileInfo.FullName, null, null, fileInfo.Length, fileInfo.LastWriteTimeUtc);
            return result;
        }

        public async Task DeleteBlobAFunc(string blobPath, string eTag = null)
        {
            verifyRootPathRemaining(blobPath);
            var fullPath = Path.Combine(LogicalRootPath, blobPath);
            var fileInfo = new FileInfo(fullPath);
            fileInfo.Delete();
        }

        public async Task<byte[]> DownloadBlobDataAFunc(IContainerOwner owner, string blobPath, bool returnNullIfMissing,
            string eTag = null)
        {
            var fileLocation = GetOwnerContentLocation(owner, blobPath);
            var fsInfo = new FileInfo(fileLocation);
            if (!fsInfo.Exists)
            {
                if (returnNullIfMissing)
                    return null;
                throw new IOException($"Blob not exists: {fsInfo.FullName}");
            }

            using (var fileStream = fsInfo.OpenRead())
            {
                var data = new byte[fsInfo.Length];
                await fileStream.ReadAsync(data, 0, (int) fsInfo.Length);
                fileStream.Close();
                return data;
            }
        }

        public async Task DownloadBlobStreamAFunc(IContainerOwner owner, string blobPath, Stream stream, bool returnNullIfMissing, string eTag = null)
        {
            var fileLocation = GetOwnerContentLocation(owner, blobPath);
            var fsInfo = new FileInfo(fileLocation);
            if (!fsInfo.Exists)
            {
                if (returnNullIfMissing)
                    return;
                throw new TBStorageException( HttpStatusCode.NotFound, null, $"Blob not exists: {fsInfo.FullName}");
            }

            using (var fileStream = fsInfo.OpenRead())
            {
                await fileStream.CopyToAsync(stream);
            }
        }

        public async Task<string[]> GetLocationFoldersAFunc(IContainerOwner owner, string locationPath)
        {
            var folderLocation = GetOwnerContentLocation(owner, locationPath);
            var directoryInfo = new DirectoryInfo(folderLocation);
            var fsInfos = directoryInfo.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly);
            var folderNames = fsInfos.AsParallel()
                .Where(fsItem => fsItem is DirectoryInfo)
                .Cast<DirectoryInfo>()
                .Select(item => item.FullName)
                .OrderBy(item => item)
                .ToArray();
            return folderNames;
        }

        public async Task<BlobStorageItem> UploadBlobDataAFunc(IContainerOwner owner, string blobPath, byte[] data, string eTag = null)
        {
            var fileLocation = GetOwnerContentLocation(owner, blobPath);
            using (var fileStream = File.Create(fileLocation))
            {
                await fileStream.WriteAsync(data, 0, data.Length);
            }

            var result = await GetBlobItemA(owner, blobPath);
            return result;
        }

        public async Task<BlobStorageItem> UploadBlobTextAFunc(IContainerOwner owner, string blobPath, string text, string eTag = null)
        {
            var fileLocation = GetOwnerContentLocation(owner, blobPath);
            var blobAddress = GetOwnerContentLocation(owner, blobPath);
            using (var textStream = File.CreateText(fileLocation))
            {
                await textStream.WriteAsync(text);
            }
            var result = await GetBlobItemA(owner, blobPath);
            return result;
        }

        public async Task<BlobStorageItem> UploadBlobStreamAFunc(IContainerOwner owner, string blobPath, Stream stream, string eTag = null)
        {
            var fileLocation = GetOwnerContentLocation(owner, blobPath);
            using (var fileStream = File.Create(fileLocation))
            {
                await stream.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }
            var result = await GetBlobItemA(owner, blobPath);
            return result;
        }

    }
}