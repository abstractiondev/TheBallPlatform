using System;
using System.Threading.Tasks;

namespace TheBall.CORE.Storage
{
    public static class JSONSupport
    {
        public static T GetObjectFromData<T>(byte[] blobData)
        {
            throw new System.NotImplementedException();
        }

        public static object SerializeToJSONData(object dataObject)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IContainerOwner
    {
        string ContainerName { get; set; }
        string LocationPrefix { get; set; }
    }

    public static class Extensions
    {
        public static bool IsSystemOwner(this IContainerOwner owner)
        {
            return owner.ContainerName == "sys" && owner.LocationPrefix == "AAA";
        }

        public static string GetOwnerContentLocation(this IContainerOwner owner, string path)
        {
            return owner.ContainerName + "/" + owner.LocationPrefix + "/" + path;
        }
    }

    public class InformationContext
    {
        public static IContainerOwner CurrentOwner { get; set; }
    }

    public static class StorageSupport
    {
        public static async Task<BlobStorageItem> GetBlobStorageItem(string sourceFullPath, IContainerOwner owner)
        {
            throw new System.NotImplementedException();
        }

        public static Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner containerOwner, string directoryLocation)
        {
            throw new System.NotImplementedException();
        }

        public static Task<BlobStorageItem> UploadOwnerBlobBinaryA(IContainerOwner currentOwner, string name, object data)
        {
            throw new System.NotImplementedException();
        }

        internal static Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner owner, string rootFolder, bool v)
        {
            throw new NotImplementedException();
        }

        public static async Task<string[]> ListOwnerFoldersA(string rootFolder)
        {
            throw new NotImplementedException();
        }

        public static Task<byte[]> DownloadBlobByteArrayAsync(string name, bool returnNullIfMissing, IContainerOwner owner)
        {
            throw new NotImplementedException();
        }

        public static Task CopyBlobBetweenOwnersA(IContainerOwner sourceOwner, string sourceItemName, IContainerOwner targetOwner, string targetItemName)
        {
            throw new NotImplementedException();
        }

        public static Task DeleteBlobAsync(string name)
        {
            throw new NotImplementedException();
        }

        public static Task UploadOwnerBlobTextAsync(IContainerOwner owner, string metadataFullPath, string jsonData)
        {
            throw new NotImplementedException();
        }
    }
}