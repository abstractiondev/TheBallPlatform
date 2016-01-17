using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
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
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

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
                WorkerManager currentManager = null;
                while (!cancellationToken.IsCancellationRequested)
                {
                    // TODO Polling update and launching
                    currentManager = await PollAndUpdateStartWorkerIfNeeded(currentManager);
                    // Poll or exit on cancel
                    await Task.Delay(30000, cancellationToken);
                }

                // Clean up worker role console
                if (currentManager != null)
                    await currentManager.ShutdownWorkerConsole();
            }
            catch(Exception exception)
            {
                File.WriteAllText(Path.Combine(WorkerFolder, "RunError.txt"), exception.ToString());
                throw;
            }
        }

        private async Task<WorkerManager> PollAndUpdateStartWorkerIfNeeded(WorkerManager currentManager)
        {
            var filesDownloaded = await PollAndDownloadWorkerPackageFromStorage();
            bool needsUpdating = filesDownloaded.Length > 0;
            if (needsUpdating)
            {
                if (currentManager != null) 
                    await currentManager.ShutdownWorkerConsole();
                currentManager = null;
                unzipFiles(filesDownloaded);
            }
            if (currentManager == null)
            {
                var directory = new DirectoryInfo(WorkerFolder);
                var files = directory.GetFiles("*Console.exe");
                var consoleExePath = files.First().FullName;
                currentManager = new WorkerManager(consoleExePath);
                await currentManager.StartWorkerConsole();
            }
            return currentManager;
        }

        private void unzipFiles(string[] filesDownloaded)
        {
            foreach (var fileName in filesDownloaded)
            {
                var unzipProcInfo = new ProcessStartInfo(PathTo7Zip, String.Format(@"x -y {0}", fileName));
                unzipProcInfo.WorkingDirectory = WorkerFolder;
                var unzipProc = Process.Start(unzipProcInfo);
                unzipProc.WaitForExit();
            }
        }


        private async Task<string[]> PollAndDownloadWorkerPackageFromStorage()
        {
            var blobSegment = await InstanceWorkerContainer.ListBlobsSegmentedAsync(null, true, BlobListingDetails.Metadata, null, null, null, null);
            var blobs = blobSegment.Results;
            var blobsInOrder = blobs.Cast<CloudBlockBlob>().OrderByDescending(blob => Path.GetExtension(blob.Name));
            List<string> filesDownloaded = new List<string>();
            foreach (CloudBlockBlob blob in blobsInOrder)
            {
                string blobFileName = blob.Name;
                string fileName = Path.GetFileName(blobFileName);
                string workerFolderFile = Path.Combine(WorkerFolder, blob.Name);
                FileInfo currentFile = new FileInfo(workerFolderFile);
                var blobLastModified = blob.Properties.LastModified.GetValueOrDefault().UtcDateTime;
                bool needsProcessing = !currentFile.Exists || currentFile.LastWriteTimeUtc != blobLastModified;
                try
                {
                    if (needsProcessing)
                    {
                        await blob.DownloadToFileAsync(workerFolderFile, FileMode.Create);
                        currentFile.Refresh();
                        currentFile.LastWriteTimeUtc = blobLastModified;
                        filesDownloaded.Add(fileName);
                    }
                }
                catch (Exception exception)
                {
                    var errorFileName = Path.Combine(WorkerFolder, "LastError.txt");
                    File.WriteAllText(errorFileName, exception.ToString());
                }
            }
            return filesDownloaded.ToArray();
        }



        private static string WorkerFolder
        {
            get
            {
                var localResource = RoleEnvironment.GetLocalResource("WorkerFolder");
                return localResource.RootPath;
            }
        }

    }
}
