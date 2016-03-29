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
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.Infra.AzureRoleSupport;
using TheBall.Infra.WebServerManager;

namespace TheBallWebRole
{
    public class WebRole : AcceleratorRole
    {
        const string TheBallWebConsoleName = "TheBallWebConsole.exe";
        protected override string AppPackageContainerName => "tb-instancesites";
        protected override string AppRootFolder => RoleEnvironment.GetLocalResource("TempSites").RootPath;
        protected override AppTypeInfo[] ValidAppTypes => new []
        {
            new AppTypeInfo("Dev", Path.Combine(AppRootFolder, "Dev", TheBallWebConsoleName), Path.Combine(AppRootFolder, "Dev.config")),
            new AppTypeInfo("Test", Path.Combine(AppRootFolder, "Test", TheBallWebConsoleName), Path.Combine(AppRootFolder, "Test.config")),
            new AppTypeInfo("Prod", Path.Combine(AppRootFolder, "Prod", TheBallWebConsoleName), Path.Combine(AppRootFolder, "Prod.config")), 
        };
    }

    public class WebRolex : RoleEntryPoint
    {
        private const string SiteContainerName = "tb-instancesites";
        //private const string PathTo7Zip = @"d:\bin\7z.exe";
        private const string PathTo7Zip = @"E:\TheBallInfra\7z\7z.exe";

        private CloudStorageAccount StorageAccount;
        private CloudBlobClient BlobClient;
        private CloudBlobContainer InstanceSiteContainer;
        private volatile bool IsRunning = false;
        private volatile bool TaskIsDone = true;

        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            var storageAccountName = CloudConfigurationManager.GetSetting("CoreFileShareAccountName");
            var storageAccountKey = CloudConfigurationManager.GetSetting("CoreFileShareAccountKey");
            StorageAccount = new CloudStorageAccount(new StorageCredentials(storageAccountName, storageAccountKey), true);

            BlobClient = StorageAccount.CreateCloudBlobClient();
            InstanceSiteContainer = BlobClient.GetContainerReference(SiteContainerName);

            string hostsFileContents = 
@"127.0.0.1 dev
127.0.0.1   test
127.0.0.1   prod
127.0.0.1   websites
";
            var hostsFilePath = Path.Combine(Environment.SystemDirectory, "drivers", "etc", "hosts");
            File.WriteAllText(hostsFilePath, hostsFileContents);

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
            var appZipNames = new[] {"Dev.zip", "Test.zip", "Prod.zip"};
            var configTxtNames = new[] {"websites.txt", "BindingData.txt"};
            var blobs = InstanceSiteContainer.ListBlobs(null, true, BlobListingDetails.Metadata);
            var blobsInOrder = blobs.Cast<CloudBlockBlob>().OrderByDescending(blob => Path.GetExtension(blob.Name));
            foreach (CloudBlockBlob blob in blobsInOrder)
            {
                string blobFileName = blob.Name;
                string fileName = Path.GetFileName(blobFileName);
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
                    //bool isZip = fileName.ToLower().EndsWith(".zip");
                    bool isAppZip = appZipNames.Contains(fileName);
                    bool isConfigTxt = configTxtNames.Contains(fileName);
                    if (isAppZip)
                    {
                        string appSiteName = Path.GetFileNameWithoutExtension(fileName);
                        processAppSitePolling(appSiteName, fileName, needsProcessing);
                        if(appSiteName == "Prod")
                            processAppSitePolling("websites", fileName, needsProcessing);
                    }
                    else if (isConfigTxt)
                    {
                        if (needsProcessing)
                        {
                            var txtData = blob.DownloadText();
                            if(fileName == "websites.txt")
                                UpdateIISSiteFromTxt("websites", txtData);
                            else if (fileName == "BindingData.txt")
                                IISSupport.UpdateInstanceBindings(txtData, "Dev", "Test", "Prod");
                        }
                    }
                }
                catch(Exception exception)
                {
                    var errorFileName = Path.Combine(TempSitesRootFolder, "LastError.txt");
                    File.WriteAllText(errorFileName, exception.ToString());
                }
            }
        }


        private void processAppSitePolling(string appSiteName, string zipFileName, bool needsProcessing)
        {
            string appSiteFolder = Path.Combine(LiveSitesRootFolder, appSiteName);
            bool needsInitialDeployment = !Directory.Exists(appSiteFolder);
            if (needsInitialDeployment)
                DeployAppFromZip(TempSitesRootFolder, appSiteName, zipFileName, LiveSitesRootFolder);
            EnsureIISSite(appSiteName, appSiteFolder);
            bool needsUpdateDeployment = needsProcessing && !needsInitialDeployment;
            if (needsUpdateDeployment)
                DeployAppFromZip(TempSitesRootFolder, appSiteName, zipFileName, LiveSitesRootFolder);
        }

        private void UpdateIISSiteFromTxt(string siteName, string txtData)
        {
            var hostHeaders = txtData.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            IISSupport.EnsureHttpHostHeaders(siteName, hostHeaders);
        }

        private void EnsureIISSite(string appSiteName, string appSiteFolder)
        {
            IISSupport.CreateIISApplicationSiteIfMissing(appSiteName, appSiteFolder);
        }

        private void DeployAppFromZip(string tempSitesRootFolder, string appSiteName, string zipFileName, string liveRootFolder)
        {
            string appTempDirName = Path.Combine(tempSitesRootFolder, appSiteName);
            var tempDirectory = new DirectoryInfo(appTempDirName);
            if (tempDirectory.Exists)
                tempDirectory.Delete(true);
            tempDirectory.Create();
            var unzipProcInfo = new ProcessStartInfo(PathTo7Zip, String.Format(@"x ..\{0}", zipFileName));
            unzipProcInfo.WorkingDirectory = appTempDirName;
            var unzipProc = Process.Start(unzipProcInfo);
            unzipProc.WaitForExit();

            string appLiveFolder = Path.Combine(liveRootFolder, appSiteName);

            IISSupport.DeployAppSiteContent(appTempDirName, appLiveFolder);
        }

        [Obsolete("Separate IIS site creation from its deployment", true)]
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
