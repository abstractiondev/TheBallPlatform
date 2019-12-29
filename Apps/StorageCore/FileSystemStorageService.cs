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
        private const string RequiredSubString = "TheBallData";
        public readonly string RootPath;
        public FileSystemStorageService(string rootPath)
        {
            if(!rootPath.Contains(RequiredSubString))
                throw new ArgumentException($"FileSystemStorage requires {RequiredSubString} to be present in the path");
            RootPath = rootPath;
        }

        private void verifyRootPathRemaining(string path)
        {
            var combinedPath = Path.Combine(RootPath, path);
            DirectoryInfo fullDir = new DirectoryInfo(combinedPath);
            var fullDirPath = fullDir.FullName;
            bool startsWith = fullDirPath.StartsWith(RootPath, StringComparison.OrdinalIgnoreCase);
            if(!startsWith)
                throw new ArgumentException($"Path is invalid: {path}");
        }

        public string GetOwnerContentLocation(IContainerOwner owner, string location)
        {
            verifyRootPathRemaining(location);
            var result = Path.Combine(owner.ContainerName, owner.LocationPrefix, location);
            verifyRootPathRemaining(result);
            return result;
        }


        public string CombinePathForOwner(IContainerOwner owner, params string[] pathComponents)
        {
            var location = Path.Combine(pathComponents);
            var contentLocation = GetOwnerContentLocation(owner, location);
            return contentLocation;
        }

        public async Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner owner, string locationPath)
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

        public async Task<BlobStorageItem> GetBlobItemA(IContainerOwner owner, string blobPath)
        {
            var fileLocation = GetOwnerContentLocation(owner, blobPath);
            var fileInfo = new FileInfo(fileLocation);
            var result = new BlobStorageItem(fileInfo.FullName, null, null, fileInfo.Length, fileInfo.LastWriteTimeUtc);
            return result;
        }

        public async Task DeleteBlobA(string blobPath, string eTag = null)
        {
            verifyRootPathRemaining(blobPath);
            var fullPath = Path.Combine(RootPath, blobPath);
            var fileInfo = new FileInfo(fullPath);
            fileInfo.Delete();
        }

        public async Task<byte[]> DownloadBlobDataA(IContainerOwner owner, string blobPath, bool returnNullIfMissing,
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

        public async Task DownloadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, bool returnNullIfMissing, string eTag = null)
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

        public async Task<string[]> GetLocationFoldersA(IContainerOwner owner, string locationPath)
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

        public async Task<BlobStorageItem> UploadBlobDataA(IContainerOwner owner, string blobPath, byte[] data, string eTag = null)
        {
            var fileLocation = GetOwnerContentLocation(owner, blobPath);
            using (var fileStream = File.Create(fileLocation))
            {
                await fileStream.WriteAsync(data, 0, data.Length);
            }

            var result = await GetBlobItemA(owner, blobPath);
            return result;
        }

        public async Task<BlobStorageItem> UploadBlobTextA(IContainerOwner owner, string blobPath, string text, string eTag = null)
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

        public async Task<BlobStorageItem> UploadBlobStreamA(IContainerOwner owner, string blobPath, Stream stream, string eTag = null)
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

        public async Task<string> AcquireLogicalLockByCreatingBlobAsync(string lockLocation)
        {
            throw new NotImplementedException();
        }

        public async Task ReleaseLogicalLockByDeletingBlobAsync(string lockLocation, string lockEtag)
        {
            throw new NotImplementedException();
        }

        public async Task<string> TryClaimLockForOwnerAsync(IContainerOwner owner, string ownerLockFileName, string lockFileContent)
        {
            throw new NotImplementedException();
        }

        public async Task ReplicateClaimedLockAsync(IContainerOwner owner, string ownerLockFileName, string lockFileContent)
        {
            throw new NotImplementedException();
        }
    }
}