using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using ProtoBuf;
using TheBall.Support.DeviceClient;

namespace TheBallMobileApp
{

    [ProtoContract]
    public class VirtualFS
    {
        private object MyLock = new object();

        [ProtoMember(1)]
        public readonly string StorageFolderLocation;
        [ProtoMember(2)]
        public Dictionary<string, VFSItem> FileLocationDictionary = new Dictionary<string, VFSItem>();
        [ProtoMember(3)]
        public Dictionary<string, VFSItem[]> ContentHashDictionary = new Dictionary<string, VFSItem[]>();

        public static VirtualFS Current { get; private set; }

        public VirtualFS()
        {
            
        }

        static VirtualFS()
        {
            var localPersonalPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var virtualFSPath = Path.Combine(localPersonalPath, "VFS");
            if (!Directory.Exists(virtualFSPath))
                Directory.CreateDirectory(virtualFSPath);
            InitializeVFS(virtualFSPath);
        }

        private string VFSMetaFileName
        {
            get { return Path.Combine(StorageFolderLocation, "VirtualFS.protobuf"); }
        }

        private static void InitializeVFS(string storageFolderLocation)
        {
            if(Directory.Exists(storageFolderLocation) == false)
                throw new ArgumentException("Directory does not exist: " + storageFolderLocation, "storageFolderLocation");
            Current = new VirtualFS(storageFolderLocation);
            Current = Current.LoadFrom();
        }

        private VirtualFS LoadFrom()
        {
            string vfsMetaFileName = VFSMetaFileName;
            VirtualFS result;
            if (!File.Exists(vfsMetaFileName))
            {
                result = this;
            }
            else
            {
                using (var stream = File.OpenRead(vfsMetaFileName))
                {
                    result = Serializer.Deserialize<VirtualFS>(stream);
                }
            }
            return result;
        }

        [ProtoContract]
        public class VFSItem
        {
            [ProtoMember(1)]
            public string FileName;
            [ProtoMember(2)]
            public string StorageFileName;
            [ProtoMember(3)]
            public string ContentMD5;
            [ProtoMember(4)]
            public long ContentLength;
            //public DateTime LastModifiedTime;
        }


        private VirtualFS(string storageFolderLocation)
        {
            StorageFolderLocation = storageFolderLocation;
        }

        public ContentItemLocationWithMD5[] GetContentRelativeFromRoot(string rootLocation)
        {
            lock (MyLock)
            {
                var allFileNames = FileLocationDictionary.Keys;
                var filesBelongingToRootLQ = allFileNames.Where(fileName => fileName.StartsWith(rootLocation));
                var contentItems = filesBelongingToRootLQ.Select(fileName =>
                {
                    var vfsItem = FileLocationDictionary[fileName];
                    var relativeFileName = fileName.Substring(rootLocation.Length);
                    return new ContentItemLocationWithMD5
                    {
                        ContentLocation = relativeFileName,
                        ContentMD5 = vfsItem.ContentMD5
                    };
                }).ToArray();
                return contentItems;
            }
        }

        public void RemoveLocalContent(string targetFullName)
        {
            lock (MyLock)
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
        }

        private void SaveChanges()
        {
            var realFile = VFSMetaFileName;
            var tmpFile = realFile + "_tmp";
            using (var stream = File.Create(tmpFile))
            {
                Serializer.Serialize(stream, this);
            }
            File.Copy(tmpFile, realFile, true);
        }

        private static string toFileNameSafeMD5(string md5)
        {
            return md5.Replace("+", "-").Replace("/", "_");
        }

        private static string toRealMD5(string fileNameSafeMD5)
        {
            return fileNameSafeMD5.Replace("-", "+").Replace("_", "/");
        }

        public Stream GetLocalTargetStreamForRead(string targetFullName)
        {
            lock (MyLock)
            {
                VFSItem fsItem;
                if (!FileLocationDictionary.TryGetValue(targetFullName, out fsItem))
                {
                    string[] allKeys = FileLocationDictionary.Keys.Cast<string>().ToArray();
                    return null;
                }
                var storageFullName = getStorageFullPath(fsItem.StorageFileName);
                return File.OpenRead(storageFullName);
            }
        }

        public Stream GetLocalTargetStreamForWrite(ContentItemLocationWithMD5 targetLocationItem)
        {
            lock (MyLock)
            {
                if (fileExists(targetLocationItem.ContentLocation))
                    RemoveLocalContent(targetLocationItem.ContentLocation);
                var fsItem = new VFSItem
                {
                    ContentMD5 = targetLocationItem.ContentMD5,
                    FileName = targetLocationItem.ContentLocation,
                    StorageFileName =
                        toFileNameSafeMD5(targetLocationItem.ContentMD5) +
                        Path.GetExtension(targetLocationItem.ContentLocation)
                };
                FileLocationDictionary.Add(targetLocationItem.ContentLocation, fsItem);
                var storageFileName = getStorageFullPath(fsItem.StorageFileName);
                return File.Create(storageFileName);
            }
        }

        private string getStorageFullPath(string storageFileName)
        {
            return Path.Combine(StorageFolderLocation, storageFileName);
        }

        private bool fileExists(string contentLocation)
        {
            lock (MyLock)
            {
                return FileLocationDictionary.ContainsKey(contentLocation);
            }
        }

        public void UpdateMetadataAfterWrite(ContentItemLocationWithMD5 targetLocationItem)
        {
            lock (MyLock)
            {
                var fsItem = FileLocationDictionary[targetLocationItem.ContentLocation];
                var fileInfo = new FileInfo(getStorageFullPath(fsItem.StorageFileName));
                fsItem.ContentLength = fileInfo.Length;
                var contentHashKey = fsItem.ContentMD5;
                VFSItem[] allLinkItems;
                if (ContentHashDictionary.TryGetValue(contentHashKey, out allLinkItems))
                {
                    allLinkItems = allLinkItems.Concat(new VFSItem[] {fsItem}).ToArray();
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
}