using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using TheBall.Core.Storage;
using TheBall.Core.StorageCore;

namespace TheBall.Core.Storage
{
    public static class BlobStorage
    {
        internal class StorageSupport
        {
            private static IStorageService Service => CoreServices.GetCurrent<IStorageService>();
            public const int GuidLength = 36;

            public static async Task<BlobStorageItem> GetBlobStorageItem(string sourceFullPath, IContainerOwner owner)
            {
                var blobStorageItem = await Service.GetBlobItemA(owner, sourceFullPath);
                return blobStorageItem;
            }

            public static async Task UploadOwnerBlobTextAsync(IContainerOwner owner, string name, string textData)
            {
                await Service.UploadBlobTextA(owner, name, textData);
            }

            public static async Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner owner, string directoryLocation, bool allowNoOwner = false)
            {
                var blobStorageItems = await Service.GetBlobItemsA(owner, directoryLocation);
                return blobStorageItems;
            }

            public static async Task CopyBlobBetweenOwnersA(IContainerOwner sourceOwner, string sourceItemName, IContainerOwner targetOwner, string targetItemName)
            {
                throw new NotImplementedException();
            }

            public static async Task DeleteBlobAsync(string name)
            {
                await Service.DeleteBlobA(blobPath: name);
            }

            public static async Task<byte[]> DownloadBlobByteArrayAsync(string name, bool returnNullIfMissing, IContainerOwner owner)
            {
                var blobData = await Service.DownloadBlobDataA(owner, blobPath: name, returnNullIfMissing);
                return blobData;
            }

            public static async Task<string[]> ListOwnerFoldersA(IContainerOwner owner, string rootFolder)
            {
                var locationFolders = await Service.GetLocationFoldersA(owner, locationPath: rootFolder);
                return locationFolders;
            }

            public static async Task<BlobStorageItem> UploadOwnerBlobBinaryA(IContainerOwner owner, string name, byte[] data)
            {
                var blobStorageItem = await Service.UploadBlobDataA(owner, blobPath: name, data);
                return blobStorageItem;
            }
        }

        const string InterfaceDataPrefixFolder = "TheBall.Interface/InterfaceData";
        const string ShareDataPrefixFolder = "TheBall.Interface/ShareInfo";

        public static async Task<BlobStorageItem> GetBlobStorageItemA(string sourceFullPath, IContainerOwner owner = null)
        {
            BlobStorageItem blob = await StorageSupport.GetBlobStorageItem(sourceFullPath, owner);
            return blob;
        }

        public static string GetOwnerInterfaceDataFullPath(IContainerOwner owner, string fileName)
        {
            var interfaceDataName = Path.Combine(InterfaceDataPrefixFolder, fileName).Replace("\\", "/");
            if (!interfaceDataName.StartsWith(InterfaceDataPrefixFolder + "/"))
                throw new ArgumentException("Relative filename not allowed: " + fileName);
            var ownerPrefixed = GetOwnerContentLocation(owner, interfaceDataName);
            return ownerPrefixed;
        }

        public static string GetCollaborationOwnerShareFullPath(IContainerOwner owner, IContainerOwner collaborationTarget, string shareFileName, bool isMetadataFile)
        {
            string storedFileName;
            if (isMetadataFile)
            {
                var injectIndex = shareFileName.LastIndexOf('/') + 1;
                var beforeInject = shareFileName.Substring(0, injectIndex);
                var afterInject = shareFileName.Substring(injectIndex);
                storedFileName = beforeInject + "_" + afterInject + ".json";
            }
            else
                storedFileName = shareFileName;
            var interfaceDataName = Path.Combine(ShareDataPrefixFolder, collaborationTarget.ContainerName,
                collaborationTarget.LocationPrefix, storedFileName).Replace("\\", "/");
            if (!interfaceDataName.StartsWith(ShareDataPrefixFolder + "/"))
                throw new ArgumentException("Relative filename not allowed: " + shareFileName);
            var ownerPrefixed = GetOwnerContentLocation(owner, interfaceDataName);
            return ownerPrefixed;
        }

        public static string GetOwnerContentLocation(IContainerOwner owner, string blobAddress)
        {
            string ownerPrefix = owner.ContainerName + "/" + owner.LocationPrefix + "/";
            if (blobAddress.StartsWith("grp/") || blobAddress.StartsWith("acc/") || blobAddress.StartsWith("sys/"))
            {
                if (blobAddress.StartsWith(ownerPrefix) || owner.IsSystemOwner())
                    return blobAddress;
                throw new SecurityException("Invalid reference to blob: " + blobAddress + " by owner prefix: " + owner.LocationPrefix);
            }
            return ownerPrefix + blobAddress;
        }

        public static async Task UploadCurrentOwnerBlobTextAsync(IContainerOwner owner, string name, string textData)
        {
            await StorageSupport.UploadOwnerBlobTextAsync(owner, name, textData);
        }

        public static async Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner containerOwner, string prefix, Func<BlobStorageItem, bool> filteringPredicate = null)
        {
            var blobItems = await StorageSupport.GetBlobItemsA(containerOwner, prefix);
            if (filteringPredicate != null)
                blobItems = blobItems.Where(filteringPredicate).ToArray();
            return blobItems;
        }

        public static string CombinePath(params string[] pathComponents)
        {
            var path = Path.Combine(pathComponents).Replace(@"\", @"/");
            return path;
        }

        public static string CombinePathForOwner(this IContainerOwner owner, params string[] pathComponents)
        {
            var ownerPath = StorageService.PlatformService.CombinePathForOwner(owner, pathComponents);
            return ownerPath;
        }

        public static async Task CopyBlobBetweenOwnersA(IContainerOwner sourceOwner, string sourceItemName, IContainerOwner targetOwner, string targetItemName)
        {
            await
                StorageSupport.CopyBlobBetweenOwnersA(sourceOwner, sourceItemName, targetOwner, targetItemName);
        }

        public static async Task DeleteBlobA(string blobLocation)
        {
            await StorageSupport.DeleteBlobAsync(blobLocation);
        }

        public static async Task<byte[]> GetBlobContentA(string name, bool returnNullIfMissing)
        {
            return await getBlobContentA(name, null, returnNullIfMissing );
        }

        public static async Task<byte[]> GetBlobContentFromOtherOwnerA(IContainerOwner owner, string name, bool returnNullIfMissing = false)
        {
            return await getBlobContentA(name, owner, returnNullIfMissing);
        }

        private static async Task<byte[]> getBlobContentA(string name, IContainerOwner owner, bool returnNullIfMissing)
        {
            var data = await StorageSupport.DownloadBlobByteArrayAsync(name, returnNullIfMissing, owner);
            return data;

        }

        public static async Task<BlobStorageFolder[]> GetOwnerFoldersA(IContainerOwner owner, string rootFolder)
        {
            var folders = await StorageSupport.ListOwnerFoldersA(owner, rootFolder);
            var blobFolders = folders.Select(folder => new BlobStorageFolder(folder)).ToArray();
            return blobFolders;
        }

        public static async Task<BlobStorageItem[]> GetOwnerBlobsA(IContainerOwner owner, string rootFolder)
        {
            return await GetBlobItemsA(owner, rootFolder);
        }

        public static async Task<BlobStorageItem[]> GetAbsoluteLocationBlobsA(string rootFolder)
        {
            return await StorageSupport.GetBlobItemsA(null, rootFolder, true);
        }

        public static async Task<T> GetBlobJsonContentA<T>(string name) where T : class
        {
            var blobData = await GetBlobContentA(name, true);
            if (blobData == null)
                return null;
            var data = JSONSupport.GetObjectFromData<T>(blobData);
            return data;
        }

        public class JSONSupport
        {
            public static T GetObjectFromData<T>(byte[] blobData) where T : class
            {
                throw new NotImplementedException();
            }

            public static byte[] SerializeToJSONData(object dataObject)
            {
                throw new NotImplementedException();
            }
        }

        public static async Task<BlobStorageItem> StoreBlobJsonContentA(IContainerOwner owner, string name, object dataObject)
        {
            var data = JSONSupport.SerializeToJSONData(dataObject);
            var blobStorageItem = await StorageSupport.UploadOwnerBlobBinaryA(owner, name, data);
            return blobStorageItem;
        }
    }
}
