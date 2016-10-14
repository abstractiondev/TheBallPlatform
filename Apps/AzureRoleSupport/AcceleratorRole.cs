using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TheBall.Infra.AzureRoleSupport
{
    public abstract class AcceleratorRole : RoleEntryPoint
    {
        protected abstract string AppPackageContainerName { get; }

        protected string InfraToolsDir => CloudConfigurationManager.GetSetting("InfraToolsRootFolder");

        //private const string PathTo7Zip = @"d:\bin\7z.exe";
        private string PathTo7Zip => Path.Combine(InfraToolsDir, @"7z\7z.exe");

        private CloudStorageAccount StorageAccount;
        private CloudBlobClient BlobClient;
        private CloudBlobContainer AppContainer;

        protected abstract AppTypeInfo[] ValidAppTypes { get; }

        private Dictionary<string, AppTypeInfo> AppTypeDict = new Dictionary<string, AppTypeInfo>();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("TheBallRole is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            AppTypeDict = new Dictionary<string, AppTypeInfo>();
            foreach (var workerType in ValidAppTypes)
                AppTypeDict.Add(workerType.AppType, workerType);

            string appInsightsKeyPath = Path.Combine(InfraToolsDir, @"AppInsightsKey.txt");
            if (File.Exists(appInsightsKeyPath))
            {
                var appInsightsKey = File.ReadAllText(appInsightsKeyPath);
                TelemetryConfiguration.Active.InstrumentationKey = appInsightsKey;
            }

            // Set the maximum number of concurrent connections
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = 100;

            var storageAccountName = CloudConfigurationManager.GetSetting("CoreFileShareAccountName");
            var storageAccountKey = CloudConfigurationManager.GetSetting("CoreFileShareAccountKey");
            StorageAccount = new CloudStorageAccount(new StorageCredentials(storageAccountName, storageAccountKey), true);

            BlobClient = StorageAccount.CreateCloudBlobClient();
            AppContainer = BlobClient.GetContainerReference(AppPackageContainerName);


            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("TheBallRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("TheBallRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("TheBallRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                Trace.TraceInformation("Working");
                while (!cancellationToken.IsCancellationRequested)
                {
                    // TODO Polling update and launching
                    await PollAndUpdateStartAppIfNeeded();
                    // Poll or exit on cancel
                    await Task.Delay(30000, cancellationToken);
                }

                var allManagers = AppTypeDict.Values.Select(appType => appType.CurrentManager).Where(value => value != null);
                var shutdownTasks = allManagers.Select(app => app.ShutdownAppConsole()).ToArray();
                await Task.WhenAll(shutdownTasks);
            }
            catch (Exception exception)
            {
                File.WriteAllText(Path.Combine(AppRootFolder, "RunError.txt"), exception.ToString());
                throw;
            }
        }

        private async Task PollAndUpdateStartAppIfNeeded()
        {
            var appFilesDownloaded = await PollAndDownloadAppPackageFromStorage();
            var appTypes = AppTypeDict.Keys.ToArray();
            foreach (var appTypeName in appTypes)
            {
                var appType = AppTypeDict[appTypeName];
                var currentManager = appType.CurrentManager;
                var appTypeDownloaded = appFilesDownloaded.FirstOrDefault(item => item.AppType == appTypeName);
                bool needsUpdating = appTypeDownloaded != null;
                if (needsUpdating)
                {
                    var zipFileRelativeToAppType = @"..\" + appTypeDownloaded.AppPackageName;
                    if (currentManager != null)
                        await currentManager.ShutdownAppConsole();
                    currentManager = null;
                    unzipFiles(appTypeName, zipFileRelativeToAppType);
                }
                if (currentManager == null)
                {
                    var consoleExePath = appType.AppExecutablePath;
                    if (File.Exists(consoleExePath))
                    {
                        var appConfigPath = appType.AppConfigPath;
                        currentManager = new AppManager(consoleExePath, appConfigPath);
                        await currentManager.StartAppConsole();
                    }
                    appType.CurrentManager = currentManager;
                }
            }
        }

        private void unzipFiles(string workerType, string zipFileName)
        {
            var workerTypedDir = Path.Combine(AppRootFolder, workerType);
            if (Directory.Exists(workerTypedDir))
            {
                Directory.Delete(workerTypedDir, true);
            }
            Directory.CreateDirectory(workerTypedDir);
            var unzipProcInfo = new ProcessStartInfo(PathTo7Zip, String.Format(@"x -y {0}", zipFileName));
            unzipProcInfo.WorkingDirectory = workerTypedDir;
            var unzipProc = Process.Start(unzipProcInfo);
            unzipProc.WaitForExit();
        }


        private async Task<AppTypeInfo[]> PollAndDownloadAppPackageFromStorage()
        {
            //var blobSegment = await AppContainer.ListBlobsSegmentedAsync("", true, BlobListingDetails.Metadata, null, null, null, null);
            //var blobs = blobSegment.Results;
            var blobs = AppContainer.ListBlobs(null, true, BlobListingDetails.Metadata);
            var blobsInOrder = blobs.Cast<CloudBlockBlob>().OrderByDescending(blob => Path.GetExtension(blob.Name));
            var appTypesDownloaded = new List<AppTypeInfo>();
            foreach (CloudBlockBlob blob in blobsInOrder)
            {
                string blobFileName = blob.Name;
                string fileName = Path.GetFileName(blobFileName);
                var matchingAppType = ValidAppTypes.FirstOrDefault(appType => appType.AppPackageName == fileName);
                if (matchingAppType == null)
                    continue;
                string appFolderFile = Path.Combine(AppRootFolder, fileName);
                FileInfo currentFile = new FileInfo(appFolderFile);
                if (!currentFile.Directory.Exists)
                    currentFile.Directory.Create();
                var blobLastModified = blob.Properties.LastModified.GetValueOrDefault().UtcDateTime;
                bool needsProcessing = !currentFile.Exists || currentFile.LastWriteTimeUtc != blobLastModified;
                try
                {
                    if (needsProcessing)
                    {
                        await blob.DownloadToFileAsync(appFolderFile, FileMode.Create);
                        currentFile.Refresh();
                        currentFile.LastWriteTimeUtc = blobLastModified;
                        appTypesDownloaded.Add(matchingAppType);
                    }
                }
                catch (Exception exception)
                {
                    var errorFileName = Path.Combine(AppRootFolder, "LastError.txt");
                    File.WriteAllText(errorFileName, exception.ToString());
                }
            }
            return appTypesDownloaded.ToArray();
        }



        protected abstract string AppRootFolder { get; }



    }
}
