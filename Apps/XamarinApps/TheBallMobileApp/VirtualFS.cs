using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using TheBall.Support.DeviceClient;

namespace TheBallMobileApp
{
    public class VirtualFS
    {
        public readonly string StorageFolderLocation;

        public static VirtualFS Current { get; private set; }

        public static void InitializeVFS(string storageFolderLocation)
        {
            Current = new VirtualFS(storageFolderLocation);
        }

        public class VFSItem
        {
            public string FileName;
            public string StorageFileName;
            public string ContentMD5;
            public long ContentLength;
            //public DateTime LastModifiedTime;
        }

        public Dictionary<string, VFSItem>  FileLocationDictionary = new Dictionary<string, VFSItem>();
        public Dictionary<string, VFSItem[]> ContentHashDictionary = new Dictionary<string, VFSItem[]>();

        private VirtualFS(string storageFolderLocation)
        {
            StorageFolderLocation = storageFolderLocation;
        }

        public ContentItemLocationWithMD5[] GetContentRelativeFromRoot(string rootLocation)
        {
            throw new NotImplementedException();
        }

        public void RemoveLocalContent(string targetFullName)
        {
            var fsItem = FileLocationDictionary[targetFullName];
            var contentHashKey = fsItem.ContentMD5;
            var allContentLinks = ContentHashDictionary[contentHashKey];
            FileLocationDictionary.Remove(targetFullName);
            if (allContentLinks.Length == 1)
            {
                ContentHashDictionary.Remove(contentHashKey);
                File.Delete(fsItem.StorageFileName);
            }
            else
            {
                ContentHashDictionary[contentHashKey] =
                    allContentLinks.Where(link => link.FileName != targetFullName).ToArray();
            }
            SaveChanges();
        }

        private void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Stream GetLocalTargetStreamForWrite(ContentItemLocationWithMD5 targetLocationItem)
        {
            if(fileExists(targetLocationItem.ContentLocation))
                RemoveLocalContent(targetLocationItem.ContentLocation);
            var fsItem = new VFSItem
            {
                ContentMD5 = targetLocationItem.ContentMD5,
                FileName = targetLocationItem.ContentLocation,
                StorageFileName = targetLocationItem.ContentMD5 + Path.GetExtension(targetLocationItem.ContentLocation)
            };
            var storageFileName = getStorageFullPath(fsItem.StorageFileName);
            return File.Create(storageFileName);
        }

        private string getStorageFullPath(string storageFileName)
        {
            return Path.Combine(StorageFolderLocation, storageFileName);
        }

        private bool fileExists(string contentLocation)
        {
            return FileLocationDictionary.ContainsKey(contentLocation);
        }

        public void UpdateMetadataAfterWrite(ContentItemLocationWithMD5 targetLocationItem)
        {
            var fsItem = FileLocationDictionary[targetLocationItem.ContentLocation];
            var fileInfo = new FileInfo(getStorageFullPath(fsItem.StorageFileName));
            fsItem.ContentLength = fileInfo.Length;
            var contentHashKey = fsItem.ContentMD5;
            VFSItem[] allLinkItems;
            if (ContentHashDictionary.TryGetValue(contentHashKey, out allLinkItems))
            {
                allLinkItems = allLinkItems.Concat(new VFSItem[] { fsItem }).ToArray();
                ContentHashDictionary[contentHashKey] = allLinkItems;
            }
            else
            {
                ContentHashDictionary.Add(contentHashKey, new VFSItem[] {fsItem});
            }
            SaveChanges();
        }
    }
}