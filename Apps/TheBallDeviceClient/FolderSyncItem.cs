using System;
using System.IO;

namespace TheBall.Support.DeviceClient
{
    [Serializable]
    public class FolderSyncItem
    {
        public static FolderSyncItem CreateFromStagingFolderDirectory(string stagingRootFolder, string stagingSubfolder)
        {
            if (stagingSubfolder.StartsWith("DEV_") == false && stagingSubfolder.StartsWith("LIVE_") == false)
                return null;
            string remoteFolder;
            string syncType = null;
            if (stagingSubfolder.StartsWith("DEV_"))
            {
                remoteFolder = stagingSubfolder.Substring(4);
                syncType = "DEV";
            }
            else
            {
                if (stagingSubfolder == "LIVE_wwwsite")
                {
                    remoteFolder = "wwwsite";
                    syncType = "wwwsite";
                }
                else
                {
                    remoteFolder = stagingSubfolder;
                    syncType = "LIVE";
                }
            }
            if(string.IsNullOrEmpty(remoteFolder))
                throw new InvalidDataException("Invalid remote staging subfolder: " + stagingSubfolder);
            var folderSyncItem = new FolderSyncItem
                {
                    LocalFullPath = Path.Combine(stagingRootFolder, stagingSubfolder),
                    RemoteEntry = remoteFolder,
                    SyncDirection = "UP",
                    SyncItemName = "DYNAMIC",
                    SyncType = syncType
                };
            return folderSyncItem;
        }

        public static FolderSyncItem CreateFromStagingRemoteDataFolder(string stagingRootFolder, string syncEntry)
        {
            if (string.IsNullOrEmpty(syncEntry))
                return null;
            bool isFile = syncEntry.StartsWith("F:");
            if (!isFile && syncEntry.EndsWith("/") == false)
                syncEntry += "/";
            string localName = isFile ? syncEntry.Substring(2) : syncEntry;
            string localFullPath = Path.Combine(stagingRootFolder, localName);
            var folderSyncItem = new FolderSyncItem
                {
                    LocalFullPath = localFullPath,
                    RemoteEntry = syncEntry,
                    SyncDirection = "DOWN",
                    SyncItemName = "DYNAMIC",
                    SyncType = "DEV",
                    IsFile = isFile
                };
            return folderSyncItem;
        }

        public string LocalFullPath;
        public string RemoteEntry;
        public string SyncItemName;
        public string SyncDirection;
        public string SyncType;
        public bool IsFile;

        public void Validate()
        {
            if (RemoteEntry == "/")
                throw new ArgumentException("Root remote folder (/) not supported");
            if (RemoteEntry.EndsWith("/") == false && RemoteEntry.StartsWith("F:") == false)
                RemoteEntry += "/";
            if (SyncDirection != "UP" && SyncDirection != "DOWN")
                throw new ArgumentException("syncDirection must be either UP or DOWN");
            if (SyncType != "DEV" && SyncType != "wwwsite")
                throw new ArgumentException("syncType must be either DEV or wwwsite");
            if (SyncType == "wwwsite" && RemoteEntry != "wwwsite/")
                throw new ArgumentException("remoteFolder must also be wwwsite when syncType is wwwsite");
        }
    }
}