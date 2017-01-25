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
using TheBall.CORE.Storage;
using TheBall.Infra.AppUpdater;
using TheBall.Infra.WebServerManager;

namespace TheBall.Infra.TheBallWebConsole
{
    public class WebManager
    {
        private readonly string ConfigRootFolder;
        private readonly Stream HostPollingStream;
        private readonly WebConsoleConfig WebConfig;
        private bool IsTestMode = false;
        private readonly string TempSiteRootDir;
        private readonly string AppSiteRootDir;
        internal TelemetryClient AppInsightsClient { get; private set; }


        private WebManager(Stream hostPollingStream, WebConsoleConfig webConfig, string configRootFolder,
            string tempSiteRootDir, string appSiteRootDir)
        {
            HostPollingStream = hostPollingStream;
            WebConfig = webConfig;
            ConfigRootFolder = configRootFolder;
            TempSiteRootDir = tempSiteRootDir;
            AppSiteRootDir = appSiteRootDir;
        }

        private WebManager(Stream hostPollingStream)
        {
            HostPollingStream = hostPollingStream;
            IsTestMode = true;
        }

        internal static async Task<WebManager> Create(Stream hostPollingStream, string configFileFullPath, bool isTestMode,
            string tempSiteRootDir, string appSiteRootDir)
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

                webManager = new WebManager(hostPollingStream, webConsoleConfig, configRootFolder, tempSiteRootDir, appSiteRootDir);
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


        internal async Task RunUpdateLoop(Task autoUpdateTask)
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
                    var appRoot = Path.Combine(TempSiteRootDir, pkg.MaturityLevel);
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
                    var updateCheckTasks = updateManagers.Select(performUpdateIfRequired).ToArray();
                    await Task.WhenAll(updateCheckTasks);

                    var updateManagedItems =
                        updateCheckTasks.Select(task => task.Result).ToArray();

                    ensureIISSites(WebConfig);

                    Array.ForEach(updateManagedItems, updateIISSite);

                    updateIISBindings(WebConfig);

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

        private void ensureIISSites(WebConsoleConfig webConfig)
        {
            var appNames = WebConfig.InstanceBindings.Select(item => item.MaturityLevel).ToArray();
            Array.ForEach(appNames, appName =>
            {
                var appSitePath = Path.Combine(AppSiteRootDir, appName);
                IISSupport.CreateIISApplicationSiteIfMissing(appName, appSitePath);
            });
        }
        private void updateIISSite(Tuple<UpdateConfigItem, DateTime> configItemLastModifiedTuple)
        {
            var configItem = configItemLastModifiedTuple.Item1;
            var lastPerformedUpdateOperation = configItemLastModifiedTuple.Item2;
            var appName = configItem.MaturityLevel;
            var sourcePackageZip = Path.Combine(TempSiteRootDir, appName, "WebInterface.zip");
            var appSitePath = Path.Combine(AppSiteRootDir, appName);
            if (!Directory.Exists(appSitePath))
                Directory.CreateDirectory(appSitePath);
            var deployInfoFile = Path.Combine(appSitePath, "AutoDeployInfo.json");

            if (File.Exists(deployInfoFile))
            {
                var deployData = File.ReadAllBytes(deployInfoFile);
                var deployInfo = JSONSupport.GetObjectFromData<SiteDeploymentInfo>(deployData);
                if (deployInfo.DeploymentTime > lastPerformedUpdateOperation)
                    return;
            }
            IISSupport.DeployAppPackageContent(sourcePackageZip, appSitePath, appName);
            
            var currDeployInfo = new SiteDeploymentInfo
            {
                DeploymentTime = DateTime.UtcNow
            };
            var data = JSONSupport.SerializeToJSONData(currDeployInfo);
            File.WriteAllBytes(deployInfoFile, data);
        }

        private void updateIISBindings(WebConsoleConfig webConfig)
        {
            var instanceBindings = WebConfig.InstanceBindings.SelectMany(ib =>
                ib.Instances.SelectMany(instanceHostName => new BindingSetting[]
                {
                    new BindingSetting
                    {
                        Protocol = Protocol.Http,
                        AppName = ib.MaturityLevel,
                        HostName = instanceHostName
                    },
                    new BindingSetting
                    {
                        Protocol = Protocol.Https,
                        AppName = ib.MaturityLevel,
                        HostName = instanceHostName
                    }
                })).ToArray();
            var wwwSiteBindings = WebConfig.WwwSiteHostNames.Select(hostName =>
                new BindingSetting
                {
                    AppName = WebConfig.WwwSitesMaturityLevel,
                    HostName = hostName,
                    Protocol = Protocol.Http
                }).ToArray();
            var bindingSettings = instanceBindings.Concat(wwwSiteBindings).ToArray();
            var appNames = WebConfig.InstanceBindings.Select(item => item.MaturityLevel).ToArray();
            IISSupport.SetAppBindings(appNames, bindingSettings);
        }

        private static async Task<Tuple<UpdateConfigItem, DateTime>> performUpdateIfRequired(Tuple<AppUpdateManager, UpdateConfigItem> inputTuple)
        {
            var updateManager = inputTuple.Item1;
            var currConfig = inputTuple.Item2;
            bool rerunRequired;
            bool anyUpdatingDone = false;
            do
            {
                rerunRequired = await updateManager.CheckAndProcessUpdate(currConfig);
                anyUpdatingDone |= rerunRequired;
            } while (rerunRequired);
            if(anyUpdatingDone)
                Console.WriteLine("Updated: " + currConfig.MaturityLevel);
            var lastModified = updateManager.CurrentStatus.LatestUpdateOperationDone;
            return new Tuple<UpdateConfigItem, DateTime>(currConfig, lastModified);
        }
    }
}