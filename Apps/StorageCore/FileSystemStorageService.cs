using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheBall.Core.Storage;

namespace TheBall.Core.StorageCore
{
    public class FileSystemStorageService : IStorageService
    {
        public const HttpStatusCode ETagMismatchStatusCode = HttpStatusCode.PreconditionFailed;

        SemaphoreSlim MetaDataContextWriteSemaphore = new SemaphoreSlim(0, 1);
        private const char PlatformStoragePathChar = '/';
        private char PhysicalStoragePathChar = Path.DirectorySeparatorChar;

        private const string RequiredSubString = "TheBallData";
        public readonly string LogicalRootPath;
        private const string MetadataDBName = "TBStoragemetadata.sqlite";
        private readonly string MetadataDBPath;

        public FileSystemStorageService(string logicalRootPath) : this()
        {
            if(!logicalRootPath.Contains(RequiredSubString))
                throw new ArgumentException($"FileSystemStorage requires {RequiredSubString} to be present in the path");

            if(!logicalRootPath.EndsWith("/"))
                throw new ArgumentException("FileSystemStorage logical root path needs to end with /");
            LogicalRootPath = logicalRootPath;
            var physicalRootPath = convertToFileSystemStoragePath(LogicalRootPath);
            MetadataDBPath = Path.Combine(physicalRootPath, MetadataDBName);
        }

        private string addLogicalStoragePathRoot(string logicalPath)
        {
            if (logicalPath.StartsWith(LogicalRootPath))
                throw new ArgumentException("Blob path already contains root path");
            var logicalStoragePath = BlobStorage.CombinePath(LogicalRootPath, logicalPath);
            return logicalStoragePath;
        }

        private string removeLogicalStoragelPathRoot(string storagePath)
        {
            if(!storagePath.StartsWith(LogicalRootPath))
                throw new ArgumentException("Blob path does not start with root path");
            var logicalPath = storagePath.Substring(LogicalRootPath.Length);
            return logicalPath;
        }

        private string convertToFileSystemStoragePath(string platformPath)
        {
            return platformPath?.Replace(PlatformStoragePathChar, PhysicalStoragePathChar);
        }

        private string convertFromFileSystemStoragePath(string physicalPath)
        {
            return physicalPath?.Replace(PhysicalStoragePathChar, PlatformStoragePathChar);
        }

        private string convertFileSystemToLogicalPath(string fileSystemPath)
        {
            var storagePath = convertFromFileSystemStoragePath(fileSystemPath);
            var logicalPath = removeLogicalStoragelPathRoot(storagePath);
            return logicalPath;
        }

        private string convertLogicalToFileSystemPath(string logicalPath)
        {
            var storagePath = addLogicalStoragePathRoot(logicalPath);
            var fileSystemPath = convertToFileSystemStoragePath(storagePath);
            return fileSystemPath;
        }

        private void ensurePhysicalDirectory(string fileSystemPath)
        {
            var directoryName = Path.GetDirectoryName(fileSystemPath);
            Directory.CreateDirectory(directoryName);
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
            InitializeService = InitializeServiceFunc;
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

        public InitializeService InitializeService { get; }
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

        public async Task InitializeServiceFunc()
        {
            await MetaDataContext.CreateOrAttachToExistingDB(MetadataDBPath);
        }

        private FileInfo getFileInfo(string logicalPath)
        {
            var fileLocation = convertLogicalToFileSystemPath(logicalPath);
            var fileInfo = new FileInfo(fileLocation);
            return fileInfo;
        }

        public async Task<BlobStorageItem[]> GetBlobItemsAFunc(IContainerOwner owner, string locationPath)
        {
            var folderLocation = GetOwnerContentLocation(owner, locationPath);
            var fileSystemLocation = convertLogicalToFileSystemPath(folderLocation);
            var directoryInfo = new DirectoryInfo(fileSystemLocation);
            var fileInfos = directoryInfo.EnumerateFileSystemInfos("*", SearchOption.AllDirectories);
            var blobItems = fileInfos.AsParallel()
                .Where(fsItem => fsItem is FileInfo)
                .Cast<FileInfo>()
                .Select(fs => new BlobStorageItem(convertFileSystemToLogicalPath(fs.FullName), null, null, fs.Length, fs.LastWriteTimeUtc))
                .OrderBy(item => item.Name)
                .ToArray();
            return blobItems;
        }

        public async Task<BlobStorageItem> GetBlobItemAFunc(IContainerOwner owner, string blobPath)
        {
            var logicalPath = GetOwnerContentLocation(owner, blobPath);
            var fileInfo = getFileInfo(logicalPath);
            var result = new BlobStorageItem(convertFileSystemToLogicalPath(fileInfo.FullName), null, null, fileInfo.Length, fileInfo.LastWriteTimeUtc);
            return result;
        }

        public async Task<BlobStorageItem> UpdateBlobItem(string logicalPath, bool recalculateMd5 = false)
        {
            var fileInfo = getFileInfo(logicalPath);
            var result = await MetaDataWriteAsync(async ctx =>
            {
                bool isChanged = false;
                var blobItem = await ctx.BlobStorageItems.FirstOrDefaultAsync(item => item.Name == logicalPath);
                if (blobItem == null)
                {
                    blobItem = new BlobStorageItem(logicalPath, null, null, fileInfo.Length, fileInfo.LastWriteTimeUtc);
                    ctx.BlobStorageItems.Add(blobItem);
                    isChanged = true;
                }

                isChanged = isChanged || blobItem.ContentMD5 == null || blobItem.Length != fileInfo.Length ||
                                 blobItem.LastModified != fileInfo.LastWriteTimeUtc;

                if (isChanged || recalculateMd5)
                {
                    using (var fileStream = fileInfo.OpenRead())
                    {
                        var hash = await CalculateMD5Async(fileStream);
                        var contentMd5 = Convert.ToBase64String(hash);
                        if (blobItem.ContentMD5 != contentMd5)
                        {
                            blobItem.ContentMD5 = contentMd5;
                            isChanged = true;
                        }
                    }
                }
                if(isChanged)
                    blobItem.ETag = Guid.NewGuid().ToString();

                return blobItem;
            });
            return result;
        }

        public async Task<byte[]> CalculateMD5Async(Stream stream)
        {
            var hasher = MD5.Create();
            byte[] buffer = new byte[4 * 1024];
            int bytesRead;
            do
            {
                bytesRead = await stream.ReadAsync(buffer, 0, 4096);
                if (bytesRead > 0)
                {
                    hasher.TransformBlock(buffer, 0, bytesRead, null, 0);
                }
            } while (bytesRead > 0);
            hasher.TransformFinalBlock(buffer, 0, 0);
            return hasher.Hash;
        }

        public async Task DeleteBlobAFunc(string blobPath, string eTag = null)
        {
            verifyRootPathRemaining(blobPath);
            var existingBlob = await GetBlobStorageItem(blobPath, eTag);
            var fileInfo = getFileInfo(blobPath);
            fileInfo.Delete();
            if (existingBlob != null)
            {
                await MetaDataWriteAsync(async ctx =>
                {
                    var blobItem = await ctx.BlobStorageItems.FirstOrDefaultAsync();
                    if (blobItem != null)
                        ctx.BlobStorageItems.Remove(blobItem);
                });
            }
        }

        public async Task<byte[]> DownloadBlobDataAFunc(IContainerOwner owner, string blobPath, bool returnNullIfMissing,
            string eTag = null)
        {
            var logicalPath = GetOwnerContentLocation(owner, blobPath);
            var blobItem = await GetBlobStorageItem(logicalPath, eTag);
            if (blobItem == null)
            {
                if (returnNullIfMissing)
                    return null;
                throw new TBStorageException(HttpStatusCode.NotFound, null, $"Blob not exists: {logicalPath}");
            }
            var fileLocation = convertLogicalToFileSystemPath(logicalPath);
            var fsInfo = new FileInfo(fileLocation);
            if (!fsInfo.Exists)
            {
                if (returnNullIfMissing)
                    return null;
                throw new IOException($"Blob not exists: {logicalPath}");
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
            var logicalPath = GetOwnerContentLocation(owner, blobPath);
            var blobItem = await GetBlobStorageItem(logicalPath, eTag);
            if (blobItem == null)
            {
                if (returnNullIfMissing)
                    return;
                throw new TBStorageException(HttpStatusCode.NotFound, null, $"Blob not exists: {logicalPath}");
            }
            var fileLocation = convertLogicalToFileSystemPath(logicalPath);
            var fsInfo = new FileInfo(fileLocation);
            if (!fsInfo.Exists)
            {
                if (returnNullIfMissing)
                    return;
                throw new TBStorageException( HttpStatusCode.NotFound, null, $"Blob not exists: {logicalPath}");
            }

            using (var fileStream = fsInfo.OpenRead())
            {
                await fileStream.CopyToAsync(stream);
            }
        }

        public async Task<string[]> GetLocationFoldersAFunc(IContainerOwner owner, string locationPath)
        {
            var logicalPath = GetOwnerContentLocation(owner, locationPath);
            var folderLocation = convertLogicalToFileSystemPath(logicalPath);
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
            var logicalPath = GetOwnerContentLocation(owner, blobPath);
            if (eTag != null)
            {
                var verifyExistingItem = await GetBlobStorageItem(logicalPath, eTag);
            }
            var fileLocation = convertLogicalToFileSystemPath(logicalPath);
            ensurePhysicalDirectory(fileLocation);
            using (var fileStream = File.Create(fileLocation))
            {
                await fileStream.WriteAsync(data, 0, data.Length);
            }
            var hasher = MD5.Create();
            var hash = hasher.ComputeHash(data);
            var contentMd5 = Convert.ToBase64String(hash);
            var fileInfo = new FileInfo(fileLocation);
            var blobItem = await SetBlobStorageItem(logicalPath, fileInfo, contentMd5);
            return blobItem;
        }

        public async Task<BlobStorageItem> UploadBlobTextAFunc(IContainerOwner owner, string blobPath, string text, string eTag = null)
        {
            var logicalPath = GetOwnerContentLocation(owner, blobPath);
            if (eTag != null)
            {
                var verifyExistingItem = await GetBlobStorageItem(logicalPath, eTag);
            }
            var fileLocation = convertLogicalToFileSystemPath(logicalPath);
            ensurePhysicalDirectory(fileLocation);
            using (var textStream = File.CreateText(fileLocation))
            {
                await textStream.WriteAsync(text);
            }
            var hasher = MD5.Create();
            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(text));
            var contentMd5 = Convert.ToBase64String(hash);
            var fileInfo = new FileInfo(fileLocation);
            var blobItem = await SetBlobStorageItem(logicalPath, fileInfo, contentMd5);
            return blobItem;
        }

        public async Task<BlobStorageItem> UploadBlobStreamAFunc(IContainerOwner owner, string blobPath, Stream stream, string eTag = null)
        {
            var logicalPath = GetOwnerContentLocation(owner, blobPath);
            if (eTag != null)
            {
                var verifyExistingItem = await GetBlobStorageItem(logicalPath, eTag);
            }
            var fileLocation = convertLogicalToFileSystemPath(logicalPath);
            ensurePhysicalDirectory(fileLocation);
            var hasher = MD5.Create();
            using (var fileStream = File.Create(fileLocation))
            using (var hashStream = new CryptoStream(fileStream, hasher, CryptoStreamMode.Write))
            {
                await stream.CopyToAsync(hashStream);
                await hashStream.FlushAsync();
                await fileStream.FlushAsync();
            }
            var fileInfo = new FileInfo(fileLocation);
            var contentMd5 = Convert.ToBase64String(hasher.Hash);
            var blobItem = await SetBlobStorageItem(logicalPath, fileInfo, contentMd5);
            return blobItem;
        }

        public async Task<BlobStorageItem> GetBlobStorageItem(string logicalPath, string eTag = null)
        {
            var blobItem = await MetaDataReadAsync(async ctx =>
            {
                var blob = await ctx.BlobStorageItems.FirstOrDefaultAsync(item => item.Name == logicalPath);
                if (blob?.ETag != eTag)
                    throw new TBStorageException(ETagMismatchStatusCode, null, $"Etag mismatch: {logicalPath} / {eTag}");
                return blob;
            });
            return blobItem;
        }

        public async Task<BlobStorageItem> SetBlobStorageItem(string logicalPath, FileInfo fileInfo, string contentMd5)
        {
            var blobItem = await MetaDataWriteAsync(async ctx =>
            {
                var blob = await ctx.BlobStorageItems.FirstOrDefaultAsync(item => item.Name == logicalPath);
                var blobETag = Guid.NewGuid().ToString();
                if (blob == null)
                {
                    blob = new BlobStorageItem(logicalPath, contentMd5, blobETag, fileInfo.Length,
                        fileInfo.LastWriteTimeUtc);
                    ctx.BlobStorageItems.Add(blob);
                }
                else
                {
                    blob.ContentMD5 = contentMd5;
                    blob.Length = fileInfo.Length;
                    blob.LastModified = fileInfo.LastWriteTimeUtc;
                    blob.ETag = blobETag;
                    blob.Metadata = null;
                }

                return blob;
            });
            return blobItem;
        }

        public async Task MetaDataReadAsync(Func<MetaDataContext, Task> asyncFunc)
        {
            await MetaDataReadAsync<object>(async ctx =>
            {
                await asyncFunc(ctx);
                return null;
            });
        }

        public async Task<T> MetaDataReadAsync<T>(Func<MetaDataContext, Task<T>> asyncFunc)
        {
            using (var ctx = new MetaDataContext(MetadataDBPath, true))
            {
                var result = await asyncFunc(ctx);
                return result;
            }
        }

        public async Task MetaDataWriteAsync(Func<MetaDataContext, Task> asyncFunc)
        {
            await MetaDataWriteAsync<object>(async ctx =>
            {
                await asyncFunc(ctx);
                return null;
            });
        }

        public async Task<T> MetaDataWriteAsync<T>(Func<MetaDataContext, Task<T>> asyncFunc)
        {
            await MetaDataContextWriteSemaphore.WaitAsync();
            try
            {
                using (var ctx = new MetaDataContext(MetadataDBPath))
                {
                    var result = await asyncFunc(ctx);
                    await ctx.SaveChangesAsync();
                    return result;
                }
            }
            finally
            {
                MetaDataContextWriteSemaphore.Release();
            }
        }




    }
}