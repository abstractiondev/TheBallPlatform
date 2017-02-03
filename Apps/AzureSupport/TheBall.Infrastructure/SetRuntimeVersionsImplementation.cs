using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.File;
using TheBall.CORE.Storage;
using TheBall.Infrastructure.INT;

namespace TheBall.Infrastructure
{
    public class SetRuntimeVersionsImplementation
    {
        public static CloudFileShare GetTarget_MainConfigShare()
        {
            var fileClient = StorageSupport.CurrStorageAccount.CreateCloudFileClient();
            var share = fileClient.GetShareReference("tbcore");
            return share;
        }

        public static async Task<UpdateConfig> GetTarget_UpdateConfigAsync(CloudFileShare mainConfigShare)
        {
            var file = mainConfigShare.GetRootDirectoryReference().GetFileReference("Updateconfig.json");
            var data = await file.DownloadByteArrayAsync();
            return JSONSupport.GetObjectFromData<UpdateConfig>(data);
        }

        public static async Task<WebConsoleConfig> GetTarget_WebConsoleConfigAsync(CloudFileShare mainConfigShare)
        {
            var file = mainConfigShare.GetRootDirectoryReference().GetFileReference("Configs/WebConsole.json");
            var data = await file.DownloadByteArrayAsync();
            return JSONSupport.GetObjectFromData<WebConsoleConfig>(data);
        }

        public static CloudFileShare GetTarget_DeploymentShare(UpdateConfig updateConfig)
        {
            var accessInfo = updateConfig.PackageData.First().AccessInfo;
            var accountName = accessInfo.AccountName;
            var sasToken = accessInfo.SASToken;
            var shareName = accessInfo.ShareName;

            var baseUri = $"https://{accountName}.file.core.windows.net";
            var fileClient = new CloudFileClient(new Uri(baseUri), new StorageCredentials(sasToken));
            var share = fileClient.GetShareReference(shareName);
            return share;
        }

        private static Regex PackageInfoRegex =
            new Regex(@"(?<BuildNumber>[^_^/]+)_(?<MaturityLevel>[^_]+)_(?<Commit>[^_^\s]+)",
                RegexOptions.Compiled);

        public static async Task<DeploymentPackages> GetTarget_DeploymentPackagesAsync(CloudFileShare deploymentShare)
        {
            var rootDir = deploymentShare.GetRootDirectoryReference();
            FileContinuationToken continuationToken = null;
            List<UpdateConfigItem> packageItems = new List<UpdateConfigItem>();
            do
            {
                var segmentedResult = await rootDir.ListFilesAndDirectoriesSegmentedAsync(continuationToken);
                var packageItemsLQ = segmentedResult.Results
                    .Select(item => ParseUpdateConfigItemParts(item.Uri.AbsolutePath))
                    .Where(item => item != null);
                packageItems.AddRange(packageItemsLQ);
                continuationToken = segmentedResult.ContinuationToken;
            } while (continuationToken != null);
            return new DeploymentPackages
            {
                PackageData = packageItems.ToArray()
            };
        }

        public static UpdateConfigItem ParseUpdateConfigItemParts(string uriPath)
        {
            var match = PackageInfoRegex.Match(uriPath);
            if (!match.Success)
                return null;
            var buildNumber = match.Groups["BuildNumber"].Value;
            var maturityLevel = match.Groups["MaturityLevel"].Value;
            var commit = match.Groups["Commit"].Value;
            return new UpdateConfigItem
            {
                BuildNumber = buildNumber,
                MaturityLevel = maturityLevel,
                Commit = commit
            };
        }

        private static string getPackageKey(UpdateConfigItem item)
        {
            return String.Join("_", item.BuildNumber, item.MaturityLevel, item.Commit);
        }

        public static void ExecuteMethod_ValidateRequestedVersionsAgainstDeploymentPackages(UpdateConfig runtimeVersionData, DeploymentPackages deploymentPackages)
        {
            var existingPackageDict =
                deploymentPackages.PackageData.ToDictionary(getPackageKey);
            var invalids =
                runtimeVersionData.PackageData.Where(item => !existingPackageDict.ContainsKey(getPackageKey(item)))
                    .ToArray();
            if (invalids.Length > 0)
            {
                var invalidKeys = invalids.Select(getPackageKey).ToArray();
                throw new InvalidDataException("Invalid version info(s): " + String.Join(", ", invalidKeys));
            }
        }

        public static void ExecuteMethod_UpdatePlatformConfigurations(UpdateConfig runtimeVersionData, UpdateConfig updateConfig, WebConsoleConfig webConsoleConfig)
        {
            var runtimeVersions = runtimeVersionData.PackageData.ToDictionary(item => item.Name);
            var dataToUpdate = updateConfig.PackageData.Concat(webConsoleConfig.PackageData).ToArray();
            foreach (var versionItem in dataToUpdate.Where(item => runtimeVersions.ContainsKey(item.Name)))
            {
                var runtimeVersion = runtimeVersions[versionItem.Name];
                versionItem.BuildNumber = runtimeVersion.BuildNumber;
                versionItem.MaturityLevel = runtimeVersion.MaturityLevel;
                versionItem.Commit = runtimeVersion.Commit;
            }
        }

        public static async Task ExecuteMethod_SaveConfigurationAsync(CloudFileShare mainConfigShare, UpdateConfig updateConfig, WebConsoleConfig webConsoleConfig)
        {
            var rootDir = mainConfigShare.GetRootDirectoryReference();

            var updateConfigFile = rootDir.GetFileReference("UpdateConfig.json");
            var updateConfigData = JSONSupport.SerializeToJSONData(updateConfig);
            await updateConfigFile.UploadFromByteArrayAsync(updateConfigData, 0, updateConfigData.Length);

            var webConsoleConfigFile = rootDir.GetFileReference("Configs/WebConsole.json");
            var webConsoleConfigData = JSONSupport.SerializeToJSONData(webConsoleConfig);
            await webConsoleConfigFile.UploadFromByteArrayAsync(webConsoleConfigData, 0, webConsoleConfigData.Length);
        }
    }
}