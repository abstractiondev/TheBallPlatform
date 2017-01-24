using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using TheBall.CORE.InstanceSupport;
using TheBall.Infra.AppUpdater;

namespace TheBall.Infra.TheBallWebConsole
{
    public class WebManager
    {
        private readonly string ConfigRootFolder;
        private readonly Stream HostPollingStream;
        private readonly WebConsoleConfig WebConfig;
        private bool IsTestMode = false;
        internal TelemetryClient AppInsightsClient { get; private set; }


        private WebManager(Stream hostPollingStream, WebConsoleConfig webConfig, string configRootFolder)
        {
            HostPollingStream = hostPollingStream;
            WebConfig = webConfig;
            ConfigRootFolder = configRootFolder;
        }

        private WebManager(Stream hostPollingStream)
        {
            HostPollingStream = hostPollingStream;
            IsTestMode = true;
        }

        internal static async Task<WebManager> Create(Stream hostPollingStream, string configFileFullPath, bool isTestMode)
        {
            WebManager webManager;
            if (isTestMode)
            {
                webManager = new WebManager(hostPollingStream);
            }
            else
            {
                var webConsoleConfig = await WebConsoleConfig.GetConfig(configFileFullPath);
                string configRootFolder = Path.GetDirectoryName(configFileFullPath);

                webManager = new WebManager(hostPollingStream, webConsoleConfig, configRootFolder);
                await webManager.InitializeRuntime();
            }
            return webManager;
        }

        private async Task InitializeRuntime()
        {
            var infraConfigFullPath = Path.Combine(ConfigRootFolder, "InfraShared", "InfraConfig.json");
            await RuntimeConfiguration.InitializeRuntimeConfigs(infraConfigFullPath);
            TelemetryConfiguration.Active.InstrumentationKey =
                InfraSharedConfig.Current.AppInsightInstrumentationKey;
            AppInsightsClient = new TelemetryClient();
            RuntimeSupport.ExceptionReportHandler =
                (exception, properties) => AppInsightsClient.TrackException(exception, properties);
        }

        internal async Task WaitHandleExitCommand(int timeoutSeconds)
        {
            var pipeStream = HostPollingStream;
            var reader = pipeStream != null ? new StreamReader(pipeStream) : null;
            bool keepWorkerRunning = true;
            var pipeMessageAwaitable = reader?.ReadToEndAsync();
            while (keepWorkerRunning)
            {
                List<Task> awaitList = new List<Task>();
                awaitList.Add(pipeMessageAwaitable);
                awaitList.Add(Task.Delay(timeoutSeconds * 1000));
                await Task.WhenAny(awaitList);
                if (pipeMessageAwaitable.IsCompleted)
                    keepWorkerRunning = false;
                else
                    throw new InvalidOperationException($"Test cancel not happened within {timeoutSeconds} second timeout");
            }
        }


        private void InitStuff()
        {
            var storageAccountName = CloudConfigurationManager.GetSetting("CoreFileShareAccountName");
            var storageAccountKey = CloudConfigurationManager.GetSetting("CoreFileShareAccountKey");
            var StorageAccount = new CloudStorageAccount(new StorageCredentials(storageAccountName, storageAccountKey), true);

            var BlobClient = StorageAccount.CreateCloudBlobClient();
            var SiteContainerName = "";
            var InstanceSiteContainer = BlobClient.GetContainerReference(SiteContainerName);

            string hostsFileContents =
@"127.0.0.1 dev
127.0.0.1   test
127.0.0.1   prod
127.0.0.1   websites
";
            var hostsFilePath = Path.Combine(Environment.SystemDirectory, "drivers", "etc", "hosts");
            File.WriteAllText(hostsFilePath, hostsFileContents);


        }

        internal async Task RunUpdateLoop(Task autoUpdateTask, string tempSiteRootDir)
        {
            var pipeStream = HostPollingStream;
            var reader = pipeStream != null ? new StreamReader(pipeStream) : null;
            try
            {
                var pollingIntervalSeconds = WebConfig.PollingIntervalSeconds;

                string startupLogPath = Path.Combine(Program.AssemblyDirectory, "ConsoleStartupLog.txt");
                var startupMessage = "Starting up process (UTC): " + DateTime.UtcNow.ToString() +
                                     " with interval seconds: " + pollingIntervalSeconds;
                File.WriteAllText(startupLogPath, startupMessage);

                var pipeMessageAwaitable = reader?.ReadToEndAsync();
                bool keepConsoleRunning = true;

                var updateManagerTasks = WebConfig.PackageData.Select(async pkg =>
                {
                    var appRoot = Path.Combine(tempSiteRootDir, pkg.MaturityLevel);
                    if (!Directory.Exists(appRoot))
                        Directory.CreateDirectory(appRoot);
                    var updateManager = await AppUpdateManager.Initialize(pkg.Name, appRoot, pkg.AccessInfo);
                    return new Tuple<AppUpdateManager, UpdateConfigItem>(updateManager, pkg);
                }).ToArray();
                await Task.WhenAll(updateManagerTasks);
                var updateManagers = updateManagerTasks.Select(task => task.Result).ToArray();

                while (keepConsoleRunning)
                {
                    // Perform updates if required
                    var updatingTasks = updateManagers.Select(performUpdateIfRequired).ToArray();
                    await Task.WhenAll(updatingTasks);

                    List<Task> awaitList = new List<Task>();
                    if (pipeMessageAwaitable != null)
                        awaitList.Add(pipeMessageAwaitable);
                    if (autoUpdateTask != null)
                        awaitList.Add(autoUpdateTask);

                    // Poll for delay until check updates againb
                    var pollingDelay = Task.Delay(pollingIntervalSeconds*1000);
                    awaitList.Add(pollingDelay);
                    await Task.WhenAny(awaitList);

                    bool isCanceling = pipeMessageAwaitable != null && pipeMessageAwaitable.IsCompleted;
                    bool isUpdating = autoUpdateTask != null && autoUpdateTask.IsCompleted;
                    if (isUpdating || isCanceling)
                    {
                        var pipeMessage = isCanceling ? pipeMessageAwaitable.Result : "Updating";
                        var shutdownLogPath = Path.Combine(Program.AssemblyDirectory, "ConsoleShutdownLog.txt");
                        File.AppendAllText(shutdownLogPath,
                            "Quitting for message (UTC): " + pipeMessage + " " + DateTime.UtcNow.ToString());
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ReportException();
                throw;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                    pipeStream.Dispose();
                }
            }
        }

        private static async Task performUpdateIfRequired(Tuple<AppUpdateManager, UpdateConfigItem> inputTuple)
        {
            var updateManager = inputTuple.Item1;
            var currConfig = inputTuple.Item2;
            bool rerunRequired;
            do
            {
                rerunRequired = await updateManager.CheckAndProcessUpdate(currConfig);
            } while (rerunRequired);
        }
    }
}