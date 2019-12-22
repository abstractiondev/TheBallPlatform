using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.File;
using TheBall.Core.Storage;
using TheBall.Infrastructure.INT;

namespace TheBall.Infrastructure
{
    public class UpdateInfraDataInterfaceObjectsImplementation
    {
        public static CloudFileShare GetTarget_MainConfigShare()
        {
            //var fileClient = StorageSupport.CurrStorageAccount.CreateCloudFileClient();
            //var share = fileClient.GetShareReference("tbcore");
            //return share;
            throw new NotImplementedException();
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

        public static async Task ExecuteMethod_StoreObjectsAsync(UpdateConfig updateConfig, WebConsoleConfig webConsoleConfig, DeploymentPackages deploymentPackages)
        {
            var storeOperationTasks = new[]
            {
                ObjectStorage.StoreInterfaceObject(updateConfig),
                ObjectStorage.StoreInterfaceObject(webConsoleConfig),
                ObjectStorage.StoreInterfaceObject(deploymentPackages)
            };
            await Task.WhenAll(storeOperationTasks);
        }
    }
}