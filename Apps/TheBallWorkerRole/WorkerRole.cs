using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Nito.AsyncEx;

namespace TheBallWorkerRole
{
    internal static class TaskExt
    {
        public static Task AsAwaitable(this CancellationToken token)
        {
            var ev = new AsyncManualResetEvent();
            token.Register(() => ev.Set());
            return ev.WaitAsync();
        }
    }

    public class WorkerRole : RoleEntryPoint
    {
        private const string SiteContainerName = "tb-instanceworkers";
        //private const string PathTo7Zip = @"d:\bin\7z.exe";
        private const string PathTo7Zip = @"E:\TheBallInfra\7z\7z.exe";

        private CloudStorageAccount StorageAccount;
        private CloudBlobClient BlobClient;
        private CloudBlobContainer InstanceWorkerContainer;

        private string[] ValidWorkerInstanceTypes = { "Dev", "Test", "Stage", "Prod" };
        private Dictionary<string, WorkerManager> WorkerTypeManagersDict = new Dictionary<string, WorkerManager>();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("TheBallWorkerRole is running");

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
            WorkerTypeManagersDict = new Dictionary<string, WorkerManager>();
            foreach(var workerType in ValidWorkerInstanceTypes)
                WorkerTypeManagersDict.Add(workerType, null);

            const string appInsightsKeyPath = @"E:\TheBallInfra\AppInsightsKey.txt";
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
            InstanceWorkerContainer = BlobClient.GetContainerReference(SiteContainerName);


            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("TheBallWorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("TheBallWorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("TheBallWorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                Trace.TraceInformation("Working");
                while (!cancellationToken.IsCancellationRequested)
                {
                    // TODO Polling update and launching
                    await PollAndUpdateStartWorkerIfNeeded();
                    // Poll or exit on cancel
                    await Task.Delay(30000, cancellationToken);
                }

                var allWorkers = WorkerTypeManagersDict.Values.Where(value => value != null);
                var shutdownTasks = allWorkers.Select(worker => worker.ShutdownWorkerConsole()).ToArray();
                await Task.WhenAll(shutdownTasks);
            }
            catch(Exception exception)
            {
                File.WriteAllText(Path.Combine(WorkerRootFolder, "RunError.txt"), exception.ToString());
                throw;
            }
        }

        private async Task PollAndUpdateStartWorkerIfNeeded()
        {
            var workerFilesDownloaded = await PollAndDownloadWorkerPackageFromStorage();
            var managerTypes = WorkerTypeManagersDict.Keys.ToArray();
            foreach (var managerType in managerTypes)
            {
                var currentManager = WorkerTypeManagersDict[managerType];
                var workerTypeDownloaded = workerFilesDownloaded.FirstOrDefault(item => item.Item1 == managerType);
                bool needsUpdating = workerTypeDownloaded != null;
                if (needsUpdating)
                {
                    var workerType = workerTypeDownloaded.Item1;
                    var zipFileName = workerTypeDownloaded.Item2;
                    if (currentManager != null)
                        await currentManager.ShutdownWorkerConsole();
                    currentManager = null;
                    unzipFiles(workerType, zipFileName);
                }
                if (currentManager == null)
                {
                    var workerFolder = Path.Combine(WorkerRootFolder, managerType);
                    var directory = new DirectoryInfo(workerFolder);
                    var files = directory.GetFiles("*Console.exe");
                    var consoleExePath = files.FirstOrDefault()?.FullName;
                    if (consoleExePath != null)
                    {
                        currentManager = new WorkerManager(consoleExePath);
                        await currentManager.StartWorkerConsole();
                    }
                    WorkerTypeManagersDict[managerType] = currentManager;
                }
            }
        }

        private void unzipFiles(string workerType, string zipFileName)
        {
            var workerTypedDir = Path.Combine(WorkerRootFolder, workerType);
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


        private async Task<Tuple<string, string>[]> PollAndDownloadWorkerPackageFromStorage()
        {
            //var blobSegment = await InstanceWorkerContainer.ListBlobsSegmentedAsync("", true, BlobListingDetails.Metadata, null, null, null, null);
            //var blobs = blobSegment.Results;
            var blobs = InstanceWorkerContainer.ListBlobs(null, true, BlobListingDetails.Metadata);
            var blobsInOrder = blobs.Cast<CloudBlockBlob>().OrderByDescending(blob => Path.GetExtension(blob.Name));
            List<Tuple<string, string>> workerFilesDownloaded = new List<Tuple<string, string>>();
            var validFileNames = ValidWorkerInstanceTypes.Select(typeName => typeName + ".zip").ToArray();
            foreach (CloudBlockBlob blob in blobsInOrder)
            {
                string blobFileName = blob.Name;
                string fileName = Path.GetFileName(blobFileName);
                if (!validFileNames.Contains(fileName))
                    continue;
                var workerType = fileName.Replace(".zip", "");
                string workerFolderFile = Path.Combine(WorkerRootFolder, workerType, fileName);
                FileInfo currentFile = new FileInfo(workerFolderFile);
                if(!currentFile.Directory.Exists)
                    currentFile.Directory.Create();
                var blobLastModified = blob.Properties.LastModified.GetValueOrDefault().UtcDateTime;
                bool needsProcessing = !currentFile.Exists || currentFile.LastWriteTimeUtc != blobLastModified;
                try
                {
                    if (needsProcessing)
                    {
                        await blob.DownloadToFileAsync(workerFolderFile, FileMode.Create);
                        currentFile.Refresh();
                        currentFile.LastWriteTimeUtc = blobLastModified;
                        workerFilesDownloaded.Add(new Tuple<string, string>(workerType, fileName));
                    }
                }
                catch (Exception exception)
                {
                    var errorFileName = Path.Combine(WorkerRootFolder, "LastError.txt");
                    File.WriteAllText(errorFileName, exception.ToString());
                }
            }
            return workerFilesDownloaded.ToArray();
        }



        private static string WorkerRootFolder
        {
            get
            {
                var localResource = RoleEnvironment.GetLocalResource("WorkerFolder");
                return localResource.RootPath;
            }
        }

    }
}
