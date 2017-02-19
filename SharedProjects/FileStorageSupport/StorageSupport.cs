using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TheBall.CORE.Storage
{
    public static class StorageSupport
    {
        public static async Task CopyBlobBetweenOwnersA(IContainerOwner sourceOwner, string sourceItemName, IContainerOwner targetOwner,string targetItemName )
        {
            throw new NotImplementedException();
        }

        public static async Task DeleteBlobAsync(string name)
        {
            throw new NotImplementedException();
        }

        public static async Task<byte[]> DownloadBlobByteArrayAsync(string name, bool returnNullIfMissing,
            IContainerOwner owner)
        {
            throw new NotImplementedException();
        }

        public static async Task<BlobStorageItem[]> GetBlobItemsA(IContainerOwner containerOwner,
            string directoryLocation, bool allowNoOwner = false)
        {
            throw new NotImplementedException();
        }

        public static async Task<BlobStorageItem> GetBlobStorageItem(string sourceFullPath,
            IContainerOwner owner = null)
        {
            throw new NotImplementedException();
        }

        public static async Task<string[]> ListOwnerFoldersA(string rootFolder)
        {
            throw new NotImplementedException();
        }

        public static async Task<BlobStorageItem> UploadOwnerBlobBinaryA(IContainerOwner owner, string name,
            byte[] data)
        {
            throw new NotImplementedException();
        }


        public static async Task<BlobStorageItem> UploadOwnerBlobTextAsync(IContainerOwner owner, string name, string textData)
        {
            throw new NotImplementedException();
        }

    }
}
