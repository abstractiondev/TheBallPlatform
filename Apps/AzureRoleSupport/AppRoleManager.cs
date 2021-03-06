﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace TheBall.Infra.AzureRoleSupport
{
    public class AppRoleManager
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private const string InitialUpdatingConsoleName = "InitialUpdatingConsole.exe";

        public AppRoleManager(string infraToolsDir)
        {
            InfraToolsDir = infraToolsDir;
        }

        public void Run()
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

        public void OnStart(RoleAppInfo[] roleApplications)
        {
            RoleApplications = roleApplications;
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            // Set the maximum number of concurrent connections
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = 100;

            var updatingTasks = RoleApplications.Select(ensureUpdatedConsole).ToArray();
            Task.WaitAll(updatingTasks);

            string appInsightsKeyPath = Path.Combine(InfraToolsDir, @"AppInsightsKey.txt");
            if (File.Exists(appInsightsKeyPath))
            {
                var appInsightsKey = File.ReadAllText(appInsightsKeyPath);
                TelemetryConfiguration.Active.InstrumentationKey = appInsightsKey;
            }
        }

        public void OnStop()
        {
            Trace.TraceInformation("TheBallRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();
        }

        public static RoleAppInfo GetWebConsoleAppInfo(string appRootFolder, string tempSiteRootDir, string appSiteRootDir)
        {
            return new RoleAppInfo()
            {
                ComponentName = "TheBallWebConsole",
                AppConfigPath = @"X:\Configs\WebConsole.json",
                RoleSpecificManagerArgs = $"--tempsiterootdir {tempSiteRootDir} --appsiterootdir {appSiteRootDir}",
                AppRootFolder = appRootFolder,
                AppType = RoleAppType.WebConsole
            };
        }

        public static RoleAppInfo GetWorkerConsoleAppInfo(string appRootFolder)
        {
            return new RoleAppInfo()
            {
                ComponentName = "TheBallWorkerConsole",
                AppConfigPath = @"X:\Configs\WorkerConsole.json",
                RoleSpecificManagerArgs = null,
                AppRootFolder = appRootFolder,
                AppType = RoleAppType.WorkerConsole
            };
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public RoleAppInfo[] RoleApplications { get; private set; }
        protected bool IsMultiRole => RoleApplications.Length > 1;
        protected readonly string InfraToolsDir;
        private string PathTo7Zip => Path.Combine(InfraToolsDir, @"7z\7z.exe");

        public CancellationTokenSource CancellationTokenSource
        {
            set => cancellationTokenSource = value;
            get => cancellationTokenSource;
        }

        private string SourceConsolePath => Path.Combine(AssemblyDirectory, InitialUpdatingConsoleName);

        private async Task runUpdater(AppManager appManager)
        {
            int execCounter = 0;
            while (appManager.LatestExitCode.GetValueOrDefault(Int32.MaxValue) > 0 && execCounter < 10)
            {
                await appManager.StartAppConsole(true, true);
                await appManager.ShutdownAppConsole(true);
                execCounter++;
            }
            if (appManager.LatestExitCode < 0)
                throw new InvalidOperationException($"AppManager update failed with exit code: {appManager.LatestExitCode}");
            bool updated = execCounter > 1;
            if (updated)
                Trace.TraceInformation("Role console succesfully updated");
        }

        private async Task ensureUpdatedConsole(RoleAppInfo appInfo)
        {
            if (!File.Exists(appInfo.TargetConsolePath))
            {
                File.Copy(SourceConsolePath, appInfo.TargetConsolePath);
            }
            var appManager = new AppManager(appInfo.TargetConsolePath, appInfo.AppConfigPath);
            await runUpdater(appManager);
        }

        public class AppManagerInfo
        {
            public AppManager AppManager { get; }
            public string ManagerArgs { get; }
            public RoleAppInfo AppInfo { get; }

            public AppManagerInfo(AppManager appManager, string managerArgs, RoleAppInfo appInfo)
            {
                AppManager = appManager;
                ManagerArgs = managerArgs;
                AppInfo = appInfo;
            }
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                Trace.TraceInformation("Working");
                var managers =
                    RoleApplications.Select(appInfo => new AppManagerInfo(new AppManager(appInfo.TargetConsolePath, appInfo.AppConfigPath), appInfo.RoleSpecificManagerArgs, appInfo))
                        .ToArray();
                var managerTasks = managers.Select(manager => executeManager(manager, cancellationToken)).ToArray();
                await Task.WhenAll(managerTasks);
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        private async Task executeManager(AppRoleManager.AppManagerInfo currManagerPack, CancellationToken cancellationToken)
        {
            var currManager = currManagerPack.AppManager;
            bool clientExited = false;
            EventHandler setUpdateNeeded = (sender, args) =>
            {
                Trace.TraceInformation("Client exited - possibly due to updating need");
                clientExited = true;
            };
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!currManager.IsRunning)
                    {
                        bool initialStartup = currManager.LatestExitCode == null;
                        Trace.TraceInformation("Not running");
                        bool updateNeeded = clientExited && currManager.LatestExitCode >= 0;
                        if (updateNeeded)
                        {
                            Trace.TraceInformation("Updating");
                            await runUpdater(currManager);
                            clientExited = false;
                        }
                        if (initialStartup || updateNeeded)
                        {
                            Trace.TraceInformation("Starting");
                            await currManager.StartAppConsole(false, true, setUpdateNeeded,
                                currManagerPack.ManagerArgs);
                        }
                        else
                        {
                            var unhandledExistMessage =
                                $"Unhandled exit of client with exit code: {currManager.LatestExitCode.GetValueOrDefault(Int32.MinValue)}";
                            if (!IsMultiRole)
                            {
                                await currManager.ShutdownAppConsole(true);
                                throw new InvalidOperationException(unhandledExistMessage);
                            }
                            // Graceful restart as other roles are still running OK
                            var errfileFullPath = Path.Combine(currManagerPack.AppInfo.AppRootFolder,
                                "MultiroleRestartingError.txt");
                            var errorMessage =
                                $"{DateTime.UtcNow.ToString("u")}: {unhandledExistMessage}{Environment.NewLine}";
                            File.AppendAllText(errfileFullPath, errorMessage);
                            await currManager.ShutdownAppConsole(false);
                            clientExited = false;
                            await currManager.StartAppConsole(false, true, setUpdateNeeded, currManagerPack.ManagerArgs);
                        }
                    }
                    try
                    {
                        await Task.Delay(1000, cancellationToken);
                    }
                    catch (TaskCanceledException) // Expected
                    {
                    }
                }
                catch (Exception exception)
                {
                    var erroringApp = currManagerPack.AppInfo;
                    File.WriteAllText(Path.Combine(erroringApp.AppRootFolder, "RunError.txt"), exception.ToString());
                    throw;
                }
            }
            await currManager.ShutdownAppConsole(true);
        }
    }
}