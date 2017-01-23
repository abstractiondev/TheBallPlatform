using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.File;
using TheBall.CORE.Storage;

namespace TheBall.Infra.AppUpdater
{
    public class AppUpdateManager
    {
        const string UpdateSubfolderName = "tbupdates";
        public static async Task<AppUpdateManager>  Initialize(string componentName, string appRootFolder, AccessInfo accessInfo)
        {
            var updateManager = new AppUpdateManager();
            updateManager.AccessInfo = accessInfo;
            updateManager.AppRootFolder = appRootFolder;
            updateManager.UpdateWorkFolder = Path.Combine(appRootFolder, UpdateSubfolderName);
            updateManager.ComponentName = componentName;
            updateManager.UpdateConfigFileClient = getCloudFileClient(accessInfo.AccountName, accessInfo.SASToken);
            updateManager.UpdateFileName = $"{componentName}.zip"; ;
            updateManager.UpdateStatusFile = Path.Combine(appRootFolder, "AutoUpdateStatus.json");
            updateManager.CurrentStatus = await updateManager.GetCurrentUpdateStatus();
            return updateManager;
        }

        public UpdateConfigItem CurrentStatus { get; set; }

        public string UpdateStatusFile { get; set; }

        public string UpdateFileName { get; set; }

        public string UpdateWorkFolder { get; set; }

        public string CurrentCommit { get; set; }

        private static CloudFileClient getCloudFileClient(string accountName, string sasToken )
        {
            var baseUri = $"https://{accountName}.file.core.windows.net";
            var client = new CloudFileClient(new Uri(baseUri), new StorageCredentials(sasToken));
            return client;
        }

        public AccessInfo AccessInfo { get; set; }

        public CloudFileClient UpdateConfigFileClient { get; set; }

        public string CurrentMaturityLevel => CurrentStatus.MaturityLevel;

        public string CurrentBuildNumber => CurrentStatus.BuildNumber;

        public string ComponentName { get; set; }

        public string AppRootFolder { get; set; }

        private CloudFile getFileRef(CloudFileClient client, string shareName, string fileName)
        {
            var share = client.GetShareReference(shareName);
            var rootDir = share.GetRootDirectoryReference();
            var fileRef = rootDir.GetFileReference(fileName);
            return fileRef;
        }

        public async Task<UpdateConfigItem> GetCurrentUpdateStatus()
        {
            if (!File.Exists(UpdateStatusFile))
            {
                return new UpdateConfigItem
                {
                    Name = ComponentName
                };
            }
            var data = File.ReadAllBytes(UpdateStatusFile);
            return JSONSupport.GetObjectFromData<UpdateConfigItem>(data);
        }

        public async Task<UpdateConfig> FetchConfiguration()
        {
            var configFile = getFileRef(UpdateConfigFileClient, AccessInfo.ShareName, "UpdateConfig.json");
            byte[] updateConfigData;
            using (var stream = new MemoryStream())
            {
                await configFile.DownloadToStreamAsync(stream);
                updateConfigData = stream.ToArray();
            }
            var updateConfig = JSONSupport.GetObjectFromData<UpdateConfig>(updateConfigData);
            return updateConfig;
        }

        public async Task<UpdateConfigItem> PollUpdate()
        {
            var updateConfig = await FetchConfiguration();
            var myConfig = updateConfig.PackageData.SingleOrDefault(config => config.Name == ComponentName);
            if(myConfig == null)
                throw new InvalidDataException($"Configuration not found for component: {ComponentName}");
            bool isDifferent = myConfig.BuildNumber != CurrentBuildNumber ||
                               myConfig.MaturityLevel != CurrentMaturityLevel;
            return isDifferent ? myConfig : null;
        }

        public UpdatingStatus GetUpdateInProgress()
        {
            return CurrentStatus.UpdatingStatus;
        }

        public void MarkForUpdate(UpdateConfigItem configItem, UpdatingStatus updatingStatus)
        {
            CurrentStatus = configItem;
            CurrentStatus.UpdatingStatus = updatingStatus;
            saveCurrentStatus();
        }

        private void saveCurrentStatus()
        {
            var data = JSONSupport.SerializeToJSONData(CurrentStatus);
            File.WriteAllBytes(UpdateStatusFile, data);
        }

        public void MarkUpdateDone()
        {
            CurrentStatus.UpdatingStatus = UpdatingStatus.UpToDate;
            saveCurrentStatus();
        }

        public async Task FetchUpdate(UpdateConfigItem configItem)
        {
            var accessInfo = configItem.AccessInfo;
            var fileClient = getCloudFileClient(accessInfo.AccountName, accessInfo.SASToken);
            var updateDirName = configItem.UpdateDirName;
            var updateFileName = UpdateFileName;
            var targetFolder = Path.Combine(UpdateWorkFolder, updateDirName);

            if (!Directory.Exists(UpdateWorkFolder))
                Directory.CreateDirectory(UpdateWorkFolder);
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);
            var updateFileTargetPath = Path.Combine(targetFolder, updateFileName);
            if (!File.Exists(updateFileTargetPath))
            {
                var updateFileSourcePath = $"{updateDirName}/{updateFileName}";
                var updateSourceCloudFile = getFileRef(fileClient, accessInfo.ShareName, updateFileSourcePath);
                await updateSourceCloudFile.DownloadToFileAsync(updateFileTargetPath, FileMode.Create);
            }
        }

        public async Task UnpackUpdateOverApp(UpdateConfigItem configItem)
        {
            var updateDirName = configItem.UpdateDirName;
            var updateFileName = UpdateFileName;
            var updateFileFolder = Path.Combine(UpdateWorkFolder, updateDirName);
            var updateFilePath = Path.Combine(updateFileFolder, updateFileName);
            ZipFile.ExtractToDirectory(updateFilePath, AppRootFolder);
        }

        const string ToBeUpdatedSearchPattern = "*.*";
        const string UpdateReplacePostfix = "_toupdate";
        const string UpdateReplacedSearchPattern = ToBeUpdatedSearchPattern + UpdateReplacePostfix;

        public async Task PrepareExistingForUpdate()
        {
            var filesToRename = Directory.GetFiles(AppRootFolder, ToBeUpdatedSearchPattern, SearchOption.AllDirectories)
                .Where(fileName => fileName.StartsWith(UpdateWorkFolder) == false && fileName != UpdateStatusFile)
                .ToArray();
            Array.ForEach(filesToRename, fileName =>
            {
                var fileInfo = new FileInfo(fileName);
                var newPath = Path.Combine(fileInfo.DirectoryName, fileName + UpdateReplacePostfix);
                fileInfo.MoveTo(newPath);
            });
        }

        public async Task CleanupAfterUpdate()
        {
            var filesToDelete =
                Directory.GetFiles(AppRootFolder, UpdateReplacedSearchPattern, SearchOption.AllDirectories)
                    .Where(fileName => fileName.StartsWith(UpdateWorkFolder) == false)
                    .ToArray();
            Array.ForEach(filesToDelete, File.Delete);
        }

        public async Task<bool> CheckAndProcessUpdate()
        {
            bool needsRestart = false;
            var updatingStatus = GetUpdateInProgress();
            if (updatingStatus == UpdatingStatus.UpdatingFetchAndUnpack)
            {
                await CleanupAfterUpdate();
                await PrepareExistingForUpdate();
                await UnpackUpdateOverApp(CurrentStatus);
                MarkForUpdate(CurrentStatus, UpdatingStatus.UpdatingCleanUp);
                needsRestart = true;
            } else if (updatingStatus == UpdatingStatus.UpdatingCleanUp)
            {
                await CleanupAfterUpdate();
                MarkUpdateDone();
                needsRestart = true;
            }
            else // UpToDate
            {
                var config = await PollUpdate();
                if (config != null)
                {
                    await FetchUpdate(config);
                    MarkForUpdate(config, UpdatingStatus.UpdatingFetchAndUnpack);
                    needsRestart = true;
                }
            }
            return needsRestart;
        }
    }
}
