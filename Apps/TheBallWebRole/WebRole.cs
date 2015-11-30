using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.ApplicationServices;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.Infra.WebServerManager;

namespace TheBallWebRole
{
    public class WebRole : RoleEntryPoint
    {
        private const string SiteContainerName = "tb-sites";
        //private const string PathTo7Zip = @"d:\bin\7z.exe";
        private const string PathTo7Zip = @"E:\TheBallInfra\7z\7z.exe";

        private CloudStorageAccount StorageAccount;
        private CloudBlobClient BlobClient;
        private CloudBlobContainer SiteContainer;
        private volatile bool IsRunning = false;
        private volatile bool TaskIsDone = true;

        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            var connStr = CloudConfigurationManager.GetSetting("StorageConnectionString");

            StorageAccount = CloudStorageAccount.Parse(connStr);
            BlobClient = StorageAccount.CreateCloudBlobClient();
            SiteContainer = BlobClient.GetContainerReference(SiteContainerName);

            PollAndSyncWebsitesFromStorage();

            IsRunning = true;
            Task.Factory.StartNew(SyncWebsitesFromStorage);
            return base.OnStart();
        }

        public override void OnStop()
        {
            IsRunning = false;
            while(!TaskIsDone)
                Thread.Sleep(200);
            base.OnStop();
        }

        //private static string StorageConnectionString => CloudConfigurationManager.GetSetting("StorageConnectionString");

        private static string TempSitesRootFolder
        {
            get
            {
                var localResource = RoleEnvironment.GetLocalResource("TempSites");
                return localResource.RootPath;
            }
        }

        private static string LiveSitesRootFolder
        {
            get
            {
                var localResource = RoleEnvironment.GetLocalResource("Sites");
                return localResource.RootPath;
            }
        }

        private void SyncWebsitesFromStorage()
        {
            while (IsRunning)
            {
                TaskIsDone = false;
                PollAndSyncWebsitesFromStorage();
                if(IsRunning)
                    Thread.Sleep(30000);
            }
            TaskIsDone = true;
        }

        private void PollAndSyncWebsitesFromStorage()
        {
            var blobs = SiteContainer.ListBlobs(null, true, BlobListingDetails.Metadata);
            var blobsInOrder = blobs.Cast<CloudBlockBlob>().OrderByDescending(blob => Path.GetExtension(blob.Name));
            foreach (CloudBlockBlob blob in blobsInOrder)
            {
                string fileName = blob.Name;
                string hostAndSiteName = Path.GetFileNameWithoutExtension(fileName);
                string tempFile = Path.Combine(TempSitesRootFolder, blob.Name);
                FileInfo currentFile = new FileInfo(tempFile);
                var blobLastModified = blob.Properties.LastModified.GetValueOrDefault().UtcDateTime;
                bool needsProcessing = !currentFile.Exists || currentFile.LastWriteTimeUtc != blobLastModified;
                try
                {
                    if (needsProcessing)
                    {
                        blob.DownloadToFile(tempFile, FileMode.Create);
                        currentFile.Refresh();
                        currentFile.LastWriteTimeUtc = blobLastModified;
                    }
                    bool isZip = fileName.ToLower().EndsWith(".zip");
                    bool isTxt = fileName.ToLower().EndsWith(".txt");
                    if(isZip)
                        UpdateIISSiteFromZip(TempSitesRootFolder, hostAndSiteName, LiveSitesRootFolder, needsProcessing);
                    else if (isTxt)
                    {
                        if (needsProcessing)
                        {
                            var txtData = blob.DownloadText();
                            UpdateIISSiteFromTxt(hostAndSiteName, txtData);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private void UpdateIISSiteFromTxt(string siteName, string txtData)
        {
            var hostHeaders = txtData.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            IISSupport.SetHostHeaders(siteName, hostHeaders);
        }

        private void UpdateIISSiteFromZip(string tempSitesRootFolder, string hostAndSiteName, string liveSitesRootFolder, bool needsUnzipping)
        {
            if (needsUnzipping)
            {
                string tempDirName = Path.Combine(tempSitesRootFolder, hostAndSiteName);
                var tempDirectory = new DirectoryInfo(tempDirName);
                if (tempDirectory.Exists)
                    tempDirectory.Delete(true);
                tempDirectory.Create();
                var unzipProcInfo = new ProcessStartInfo(PathTo7Zip, String.Format(@"x ..\{0}.zip", hostAndSiteName));
                unzipProcInfo.WorkingDirectory = tempDirName;
                var unzipProc = Process.Start(unzipProcInfo);
                unzipProc.WaitForExit();
            }

            string fullLivePath = Path.Combine(liveSitesRootFolder, hostAndSiteName);
            bool needsInitialSiteDir = !Directory.Exists(fullLivePath);
            if (needsInitialSiteDir)
            {
                var targetDir = Directory.CreateDirectory(fullLivePath);
                var currAccess = targetDir.GetAccessControl();
                currAccess.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                targetDir.SetAccessControl(currAccess);
            }
            bool needsContentUpdating = needsUnzipping || needsInitialSiteDir;
            string sourceFolder = Path.Combine(tempSitesRootFolder, hostAndSiteName);
            IISSupport.UpdateSiteWithDeploy(needsContentUpdating, sourceFolder, fullLivePath, hostAndSiteName);
        }
    }
}
