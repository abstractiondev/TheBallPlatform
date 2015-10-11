using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using PCLStorage;
using ProtoBuf;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;

namespace TheBall.Support.VirtualStorage
{

    public class SQLiteFS
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public static SQLiteFS Current { get; private set; }

        public static async Task InitializeSQLiteFS(ISQLitePlatform sqlitePlatform)
        {
            //var localPersonalPath = FileSystem.Current.LocalStorage.Path;
            //var virtualFSPath = Path.Combine(localPersonalPath, "VFS");
            var fsFolder = "_SQLFS";
            await InitializeSQLiteFS(fsFolder, sqlitePlatform);
        }

        public string FileSystemFolder { get; }
        public string ContentDataFolder { get; }
        private SQLiteAsyncConnection SQLConnection;
        private ISQLitePlatform SQLitePlatform;

        private string DBFileName
        {
            get { return Path.Combine(FileSystemFolder, "FSMetaData.sqlite"); }
        }

        private static async Task InitializeSQLiteFS(string fsFolder, ISQLitePlatform sqlitePlatform)
        {
            Current = new SQLiteFS(fsFolder, sqlitePlatform);
            await FileSystem.Current.LocalStorage.CreateFolderAsync(Current.FileSystemFolder, CreationCollisionOption.OpenIfExists);
            await FileSystem.Current.LocalStorage.CreateFolderAsync(Current.ContentDataFolder, CreationCollisionOption.OpenIfExists);
            Current.SQLitePlatform = sqlitePlatform;
            await Current.InitDB();
        }

        private async Task InitDB()
        {
            var dbPath = getFilesystemAbsolutePath(DBFileName);
            var sqlitePlatform = SQLitePlatform;
            var connectionFactory = new Func<SQLiteConnectionWithLock>(
                  () => new SQLiteConnectionWithLock(sqlitePlatform,
                    new SQLiteConnectionString(dbPath, storeDateTimeAsTicks: false)));
            SQLConnection = new SQLiteAsyncConnection(connectionFactory);

            await SQLConnection.CreateTablesAsync(typeof (FileNameData), typeof (ContentStorageData));
        }

        private SQLiteFS(string fsFolder, ISQLitePlatform sqlitePlatform)
        {
            FileSystemFolder = fsFolder;
            ContentDataFolder = Path.Combine(FileSystemFolder, "Data");
        }

        public async Task<ContentItemLocationWithMD5[]> GetContentRelativeFromRoot(string rootLocation)
        {
            await _semaphore.WaitAsync();
            try
            {
                var fileNameTable = SQLConnection.Table<FileNameData>();
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
                var contentData = SQLConnection.GetAsync<ContentStorageData>(contentMd5);
                await SQLConnection.DeleteAsync(contentData, recursive: true);
                var storageFile = ContentStorageData.getStorageFileName(contentMd5);
                var storageFullName = getStorageFullPath(storageFile);
                var file = await FileSystem.Current.LocalStorage.GetFileAsync(storageFullName);
                await file.DeleteAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task RemoveLocalContent(string targetFullName, bool ignoreMissing)
        {
            await _semaphore.WaitAsync();
            try
            {
                var fileData = await SQLConnection.GetWithChildrenAsync<FileNameData>(targetFullName, recursive: true);
                if (fileData == null)
                {
                    if(ignoreMissing)                    
                        return;
                    throw new InvalidDataException("Missing metadata for local content: " + targetFullName);
                }
                if (fileData.ContentStorageData.FileNames.Count == 1)
                    await RemoveLocalContentByMD5(fileData.ContentMD5);
                else
                    await SQLConnection.DeleteAsync(fileData);
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
                var fileData = await SQLConnection.GetAsync<FileNameData>(targetFullName);
                string storageFileName = fileData.StorageFileName;
                var storageFullName = getStorageFullPath(storageFileName);
                var file = await FileSystem.Current.LocalStorage.GetFileAsync(storageFullName);
                return await file.OpenAsync(FileAccess.Read);
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
                var storageFileName = toFileNameSafeMD5(contentMD5);
                var storageFullName = getStorageFullPath(storageFileName);
                var file =
                    await
                        FileSystem.Current.LocalStorage.CreateFileAsync(storageFullName,
                            CreationCollisionOption.ReplaceExisting);
                return await file.OpenAsync(FileAccess.ReadAndWrite);
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

                var storageData = await SQLConnection.GetAsync<ContentStorageData>(contentMD5);
                if (storageData != null)
                {
                    fileNameData.ContentStorageData = storageData;
                    await SQLConnection.InsertAsync(fileNameData);
                    return null;
                }

                var contentDataItem = new ContentStorageData
                {
                    ContentMD5 = targetLocationItem.ContentMD5,
                    ContentLength = -1,
                };
                contentDataItem.FileNames.Add(fileNameData);
                await SQLConnection.InsertWithChildrenAsync(contentDataItem, recursive: true);

                var storageFullPath = getStorageFullPath(contentDataItem.StorageFileName);
                var file = await FileSystem.Current.LocalStorage.CreateFileAsync(storageFullPath,
                    CreationCollisionOption.ReplaceExisting);
                return await file.OpenAsync(FileAccess.ReadAndWrite);
            }
            finally
            {
                _semaphore.Release();
            }

        }

        private string getFilesystemAbsolutePath(string filePath)
        {
            return Path.Combine(FileSystem.Current.LocalStorage.Path, filePath);
        }

        private string getStorageFullPath(string storageFileName)
        {
            return Path.Combine(ContentDataFolder, storageFileName);
        }

        public async Task<ContentSyncRequest> CreateFullSyncRequest(string stagingRoot, string[] requestedFolders, Func<byte[], byte[]> md5HashComputer)
        {
            await _semaphore.WaitAsync();
            try
            {
                var fileDataTable = SQLConnection.Table<FileNameData>();
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
                var storageData = await SQLConnection.GetWithChildrenAsync<ContentStorageData>(contentMd5, recursive:true);
                if (initialAdd)
                    storageData.ContentLength = initialAddContentLength;
                var existingNameData = storageData.FileNames;
                var namesToDelete = existingNameData.Where(item => fullNames.Contains(item.FileName) == false);
                var namesToAdd =
                    fullNames.Where(name => existingNameData.All(item => item.FileName != name)).ToArray();
                var nameDataToAdd = namesToAdd.Select(fileName => new FileNameData
                {
                    FileName = fileName,
                    ContentMD5 = storageData.ContentMD5
                }).ToArray();
                await SQLConnection.RunInTransactionAsync((SQLiteConnection conn) =>
                {
                    conn.DeleteAll(namesToDelete);
                    conn.InsertAll(nameDataToAdd);
                    conn.Commit();
                });
            }
            finally
            {
                _semaphore.Release();
            }
        }

    }
}