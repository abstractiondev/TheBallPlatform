using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBall.CORE.Storage
{
    public static class StorageSupport
    {
        public static void InitializeRoot(string rootFolder)
        {
            RootFolder = rootFolder;
        }

        public static string RootFolder { get; private set; }

        private static string getFullPath(string filePath)
        {
            return Path.Combine(RootFolder, filePath);
        }

        public static async Task CopyBlobBetweenOwnersA(IContainerOwner sourceOwner, string sourceItemName, IContainerOwner targetOwner,string targetItemName )
        {
            var source = sourceOwner.GetOwnerContentLocation(sourceItemName);
            var target = targetOwner.GetOwnerContentLocation(targetItemName);
            var sourceFullPath = getFullPath(source);
            var targetFullPath = getFullPath(target);
            using (var sourceStream = File.OpenRead(sourceFullPath))
            using (var targetStream = File.OpenWrite(targetFullPath))
                await sourceStream.CopyToAsync(targetStream);
            
        }

        public static async Task DeleteBlobAsync(string name)
        {
            var fullPath = getFullPath(name);
            File.Delete(fullPath);
        }

        public static async Task<byte[]> DownloadBlobByteArrayAsync(string name, bool returnNullIfMissing,
            IContainerOwner owner)
        {
            try
            {
                var fullPath = getFullPath(owner.GetOwnerContentLocation(name));
                using (var fileStream = File.OpenRead(fullPath))
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }

            }
            catch (FileNotFoundException)
            {
                if (returnNullIfMissing)
                    return null;
                throw;
            }
        }

        public static async Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner containerOwner,
            string directoryLocation, bool allowNoOwner = false)
        {
            if(containerOwner.IsNoOwner() && allowNoOwner == false)
                throw new InvalidOperationException("No owner specified when not allowed");
            var ownerPath = containerOwner.GetOwnerContentLocation(directoryLocation);
            var fullPath = getFullPath(ownerPath);
            var directoryInfo = new DirectoryInfo(fullPath);
            var fileInfos = directoryInfo.GetFiles();
            var result = fileInfos.Select(getBlobStorageItem).ToArray();
            return result;
        }

        private static BlobStorageItem getBlobStorageItem(FileInfo fi)
        {
            return new BlobStorageItem(fi.FullName, null, fi.Length, fi.LastWriteTimeUtc);
        }

        public static async Task<BlobStorageItem> GetBlobStorageItem(string sourceFullPath,
            IContainerOwner owner = null)
        {
            if (owner == null)
                owner = InformationContext.CurrentOwner;
            var ownerPath = owner.GetOwnerContentLocation(sourceFullPath);
            var fullPath = getFullPath(ownerPath);
            var fileInfo = new FileInfo(fullPath);
            return getBlobStorageItem(fileInfo);
        }

        public static async Task<string[]> ListOwnerFoldersA(string rootFolder)
        {
            var owner = InformationContext.CurrentOwner;
            var ownerFolder = owner.GetOwnerContentLocation(rootFolder);
            var fullPath = getFullPath(ownerFolder);
            var directoryInfo = new DirectoryInfo(fullPath);
            var directories = directoryInfo.GetDirectories();
            var result = directories.Select(di => di.FullName).ToArray();
            return result;
        }

        public static async Task<BlobStorageItem> UploadOwnerBlobBinaryA(IContainerOwner owner, string name, byte[] data)
        {
            var ownerName = owner.GetOwnerContentLocation(name);
            var fullPath = getFullPath(ownerName);
            using (var fileStream = File.OpenWrite(fullPath))
                await fileStream.WriteAsync(data, 0, data.Length);
            return await GetBlobStorageItem(name, owner);
        }


        public static async Task<BlobStorageItem> UploadOwnerBlobTextAsync(IContainerOwner owner, string name, string textData)
        {
            var ownerName = owner.GetOwnerContentLocation(name);
            var fullPath = getFullPath(ownerName);
            using (var streamWriter = new StreamWriter(fullPath, false, Encoding.UTF8))
                await streamWriter.WriteAsync(textData);
            return await GetBlobStorageItem(name, owner);
        }

    }
}
