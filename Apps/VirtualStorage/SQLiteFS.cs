using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProtoBuf;

namespace TheBall.Support.VirtualStorage
{

    public class SQLiteFS
    {
        private const string SQLFSDIR = "_SQLFS";
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public static SQLiteFS Current { get; private set; }
        public static string FSRootFolder { get; private set; }

        public static async Task InitializeSQLiteFS(string rootFolder, string connectionSubFolderName = null)
        {
            if(!Path.IsPathRooted(rootFolder))
                throw new ArgumentException("Filesystem root path must be rooted", nameof(rootFolder));
            var fsFolder = connectionSubFolderName != null ? Path.Combine(rootFolder, SQLFSDIR, connectionSubFolderName) : Path.Combine(rootFolder, SQLFSDIR);
            Current = new SQLiteFS(fsFolder);
            Directory.CreateDirectory(Current.FileSystemFolder);
            Directory.CreateDirectory(Current.ContentDataFolder);
            await Current.InitDB();
        }

        public string FileSystemFolder { get; }
        public string ContentDataFolder { get; }
        private SqliteConnection SQLConnection;
        private VFSContext CurrentDB;

        private string DBFileName
        {
            get { return Path.Combine(FileSystemFolder, "FSMetaData.sqlite"); }
        }

        private async Task InitDB()
        {
            var dbPath = DBFileName;
            CurrentDB = await VFSContext.CreateContext(dbPath);
            await SyncContentWithMetadata();
        }

        private SQLiteFS(string fsFolder)
        {
            FileSystemFolder = fsFolder;
            ContentDataFolder = Path.Combine(FileSystemFolder, "Data");
        }

        public async Task<ContentItemLocationWithMD5[]> GetContentRelativeFromRoot(string rootLocation)
        {
            await _semaphore.WaitAsync();
            try
            {
                var fileNameTable = CurrentDB.FileNameDataTable;
                var fileNameDatas = await fileNameTable.Where(item => item.FileName.StartsWith(rootLocation)).ToListAsync();
                string[] allFileNames = fileNameDatas.Select(item => item.FileName).ToArray();
                var filesBelongingToRootLQ = allFileNames.Where(fileName => fileName.StartsWith(rootLocation));
                var contentItems = filesBelongingToRootLQ.Select(fileName =>
                {
                    FileNameData vfsItem = null; // = FileLocationDictionary[fileName];
                    var relativeFileName = fileName.Substring(rootLocation.Length);
                    return new ContentItemLocationWithMD5
                    {
                        ContentLocation = relativeFileName,
                        ContentMD5 = vfsItem.ContentMD5
                    };
                }).ToArray();
                return contentItems;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task RemoveLocalContentByMD5(string contentMd5)
        {
            await _semaphore.WaitAsync();
            try
            {
                var contentData =
                    await CurrentDB.ContentStorageDataTable.SingleAsync(item => item.ContentMD5 == contentMd5);
                CurrentDB.Remove(contentData);
                await CurrentDB.SaveChangesAsync();
                var storageFile = ContentStorageData.getStorageFileName(contentMd5);
                var storageFullName = getStorageFullPath(storageFile);
                var file = new FileInfo(storageFullName);
                file.Delete();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task RemoveLocalContent(string targetFullName, bool ignoreMissing = false)
        {
            await _semaphore.WaitAsync();
            try
            {
                var fileData =
                    await CurrentDB.FileNameDataTable.Include(item => item.ContentStorageData).SingleOrDefaultAsync(item => item.FileName == targetFullName);
                if (fileData == null)
                {
                    if(ignoreMissing)                    
                        return;
                    throw new InvalidDataException("Missing metadata for local content: " + targetFullName);
                }
                if (fileData.ContentStorageData.FileNames.Count == 1)
                    await RemoveLocalContentByMD5(fileData.ContentMD5);
                else
                {
                    CurrentDB.FileNameDataTable.Remove(fileData);
                    await CurrentDB.SaveChangesAsync();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private static string toFileNameSafeMD5(string md5)
        {
            return md5.Replace("+", "-").Replace("/", "_");
        }

        private static string toRealMD5(string fileNameSafeMD5)
        {
            return fileNameSafeMD5.Replace("-", "+").Replace("_", "/");
        }

        public async Task<Stream> GetLocalTargetStreamForRead(string targetFullName)
        {
            await _semaphore.WaitAsync();
            try
            {
                var fileData = await CurrentDB.FileNameDataTable.SingleAsync(item => item.FileName == targetFullName);
                string storageFileName = fileData.StorageFileName;
                var storageFullName = getStorageFullPath(storageFileName);
                var file = new FileInfo(storageFullName);
                return file.OpenRead();
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Stream> GetLocalTargetStreamForWrite(string contentMD5)
        {
            await _semaphore.WaitAsync();
            try
            {
                var storageData = new ContentStorageData
                {
                    ContentMD5 = contentMD5,
                    ContentLength = -1
                };
                await CurrentDB.ContentStorageDataTable.AddAsync(storageData);
                await CurrentDB.SaveChangesAsync();
                var storageFileName = toFileNameSafeMD5(contentMD5);
                var storageFullName = getStorageFullPath(storageFileName);
                var file = new FileInfo(storageFullName);
                if(file.Exists)
                    file.Delete();
                return file.OpenWrite();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Stream> GetLocalTargetStreamForWrite(ContentItemLocationWithMD5 targetLocationItem)
        {
            await _semaphore.WaitAsync();
            try
            {
                var fileName = targetLocationItem.ContentLocation;
                var contentMD5 = targetLocationItem.ContentMD5;
                await RemoveLocalContent(targetLocationItem.ContentLocation, ignoreMissing:true);
                var fileNameData = new FileNameData
                {
                    FileName = fileName,
                    ContentMD5 = contentMD5
                };

                var storageData = await CurrentDB.ContentStorageDataTable.SingleAsync(item => item.ContentMD5 == targetLocationItem.ContentMD5);
                if (storageData != null)
                {
                    fileNameData.ContentStorageData = storageData;
                    await CurrentDB.AddAsync(fileNameData);
                    return null;
                }

                var contentDataItem = new ContentStorageData
                {
                    ContentMD5 = targetLocationItem.ContentMD5,
                    ContentLength = -1,
                };
                contentDataItem.FileNames.Add(fileNameData);
                await CurrentDB.AddAsync(contentDataItem);
                await CurrentDB.SaveChangesAsync();
                var storageFullPath = getStorageFullPath(contentDataItem.StorageFileName);
                var file = new FileInfo(storageFullPath);
                if(file.Exists)
                    file.Delete();
                return file.OpenWrite();
            }
            finally
            {
                _semaphore.Release();
            }

        }

        private string getStorageFullPath(string storageFileName)
        {
            return Path.Combine(ContentDataFolder, storageFileName);
        }

        private async Task SyncContentWithMetadata()
        {
            var contentFolder = new DirectoryInfo(ContentDataFolder);
            var actualContent = contentFolder.GetFiles();
            var fileContent = actualContent.ToList();
            fileContent.Sort((a, b) => String.CompareOrdinal(a.Name, b.Name));

            var contentMetadataTable = CurrentDB.ContentStorageDataTable;
            var storageMetadata = await contentMetadataTable.ToListAsync();
            storageMetadata.Sort((a, b) => String.CompareOrdinal(a.StorageFileName, b.StorageFileName));

            var fileNameMetadataTable = CurrentDB.FileNameDataTable;
            var fileNameMetaItems = await fileNameMetadataTable.ToListAsync();
            fileNameMetaItems.Sort((a, b) => String.CompareOrdinal(a.StorageFileName, b.StorageFileName));
            var fileNameMetadata = fileNameMetaItems.GroupBy(item => item.StorageFileName).ToList();

            // Step-by-step with all lists in parallel, remove all matching elements
            int fileIX = 0;
            int fileCount = fileContent.Count;
            int storageMetaIX = 0;
            int storageMetaCount = storageMetadata.Count;
            int fileNameMetaIX = 0;
            int fileNameMetaCount = fileNameMetadata.Count;
            while (fileIX < fileCount && storageMetaIX < storageMetaCount && fileNameMetaIX < fileNameMetaCount)
            {
                var file = fileContent[fileIX];
                var storageMeta = storageMetadata[storageMetaIX];
                var fileNameMeta = fileNameMetadata[fileNameMetaIX];
                bool allMatching = getMatchingOrAdvanceSmallest(file, ref fileIX, storageMeta, ref storageMetaIX,
                    fileNameMeta, ref fileNameMetaIX);
                if (allMatching)
                {
                    fileContent[fileIX++] = null;
                    storageMetadata[storageMetaIX++] = null;
                    fileNameMetadata[fileNameMetaIX++] = null;
                }
            }

            var filesToDelete = fileContent.Where(item => item != null).ToArray();
            var storageMetaToDelete = storageMetadata.Where(item => item != null).ToArray();
            var fileNameMetaToDelete = fileNameMetadata.Where(item => item != null).SelectMany(item => item).ToArray();

            foreach (var file in filesToDelete)
                file.Delete();


            CurrentDB.RemoveRange(fileNameMetaToDelete);
            CurrentDB.RemoveRange(storageMetaToDelete);
            await CurrentDB.SaveChangesAsync();
        }

        private bool getMatchingOrAdvanceSmallest(FileInfo file, ref int fileIx, ContentStorageData storageMeta, ref int storageMetaIx, IGrouping<string, FileNameData> fileNameMeta, ref int fileNameMetaIx)
        {
            string fileName = file.Name;
            string storageName = storageMeta.StorageFileName;
            string fileMetaName = fileNameMeta.Key;
            if (fileName == storageName && fileName == fileMetaName)
                return true;
            string smallest = String.CompareOrdinal(fileName, storageName) < 0
                ? fileName
                : (String.CompareOrdinal(storageName, fileMetaName) < 0 ? storageName : fileMetaName);
            if (fileName == smallest)
                fileIx++;
            if (storageName == smallest)
                storageMetaIx++;
            if (fileMetaName == smallest)
                fileNameMetaIx++;
            return false;
        }

        public async Task<ContentSyncRequest> CreateFullSyncRequest(string stagingRoot, string[] requestedFolders, Func<byte[], byte[]> md5HashComputer)
        {
            await _semaphore.WaitAsync();
            try
            {
                var fileDataTable = CurrentDB.FileNameDataTable;
                var fileDatas = await fileDataTable.Where(item => item.FileName.StartsWith(stagingRoot)).ToListAsync();
                var allFileItems = fileDatas.Select(item => new { item.FileName, item.ContentMD5 }).ToArray();
                Uri rootUri;
                if (stagingRoot.EndsWith("\\") || stagingRoot.EndsWith("/"))
                    rootUri = new Uri(stagingRoot);
                else
                    rootUri = new Uri(stagingRoot + "/");
                var syncItems = allFileItems.Select(item =>
                    new
                    {
                        RelativeName = rootUri.MakeRelativeUri(new Uri(item.FileName)).OriginalString,
                        FullName = item.FileName,
                        ContentMD5 = item.ContentMD5
                    }).ToArray();
                var groupAndAccountFiles =
                    syncItems.Where(item => item.RelativeName.StartsWith("grp") || item.RelativeName.StartsWith("acc"))
                        .ToArray();
                var ownerGrp = groupAndAccountFiles.GroupBy(item =>
                {
                    if (item.RelativeName.StartsWith("acc"))
                        return "account";
                    var paths = item.RelativeName.Split('/');
                    return Path.Combine(paths[0], paths[1]).Replace('\\', '/');
                });
                var ownerFolderGrouped = ownerGrp.Select(groupItems =>
                {
                    var ownerPrefix = groupItems.Key;

                    var tempFolderGrp = groupItems.GroupBy(item =>
                    {
                        var paths = item.RelativeName.Split('/');
                        return paths[2];
                    });
                    var folderContent = tempFolderGrp.Select(folderGrp =>
                    {
                        var folderName = folderGrp.Key;
                        var folderContents = folderGrp.Select(item => new RemoteSyncSupport.FolderContent
                        {
                            ContentMD5 = item.ContentMD5,
                            RelativeName = item.RelativeName.Substring(item.RelativeName.IndexOf('/', 4) + 1)
                        });
                        var fullMD5Hash = RemoteSyncSupport.GetFolderMD5Hash(folderContents, md5HashComputer);
                        return new
                        {
                            FolderName = folderName,
                            FullMD5Hash = fullMD5Hash
                        };

                    });
                    return new
                    {
                        OwnerPrefix = ownerPrefix,
                        Folders = folderContent
                    };
                }).ToArray();

                var allMD5s =
                    allFileItems.Select(fileItem => fileItem.ContentMD5).Distinct().ToArray();

                var contentOwners = ownerFolderGrouped.Select(ownerItem => new ContentSyncRequest.ContentOwner
                {
                    OwnerPrefix = ownerItem.OwnerPrefix,
                    ContentFolders = ownerItem.Folders.Select(folderItem => new ContentSyncRequest.ContentFolder
                    {
                        Name = folderItem.FolderName,
                        FullMD5Hash = folderItem.FullMD5Hash
                    }).ToArray()
                }).ToArray();
                var syncRequest = new ContentSyncRequest
                {
                    ContentMD5s = allMD5s,
                    RequestedFolders = requestedFolders,
                    ContentOwners = contentOwners
                };
                return syncRequest;
            }
            finally
            {
                _semaphore.Release();
            }

        }

        public async Task UpdateContentNameData(string contentMd5, string[] fullNames, bool initialAdd = false, long initialAddContentLength = -1)
        {
            await _semaphore.WaitAsync();
            try
            {
                var storageData =
                    await CurrentDB.ContentStorageDataTable.SingleAsync(item => item.ContentMD5 == contentMd5);
                if (initialAdd)
                    storageData.ContentLength = initialAddContentLength;
                var existingNameData = storageData.FileNames;
                var namesToDelete = existingNameData.Where(item => fullNames.Contains(item.FileName) == false);
                var nameDataTable = CurrentDB.FileNameDataTable;
                var namesToUpdate = await nameDataTable.Where(item => fullNames.Contains(item.FileName)).ToListAsync();
                var namesToAdd =
                    fullNames.Where(name => existingNameData.All(item => item.FileName != name) && namesToUpdate.All(item => item.FileName != name)).ToArray();

                var nameDataToAdd = namesToAdd.Select(fileName => new FileNameData
                {
                    FileName = fileName,
                    ContentMD5 = storageData.ContentMD5
                }).ToArray();

                foreach (var name in namesToUpdate)
                    name.ContentMD5 = storageData.ContentMD5;

                CurrentDB.RemoveRange(namesToDelete);
                await CurrentDB.AddRangeAsync(nameDataToAdd);
                CurrentDB.UpdateRange(namesToUpdate);
                await CurrentDB.SaveChangesAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

    }
}