using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
    public enum AzureRoleType
    {
        WorkerRole,
        WebRole
    }


    public abstract class AcceleratorRole : RoleEntryPoint
    {
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

        const string InitialUpdatingConsoleName = "InitialUpdatingConsole.exe";

        protected abstract string AppConfigPath { get; }
        protected abstract string AppRootFolder { get; }
        protected abstract string ComponentName { get; }
        protected abstract AzureRoleType RoleType { get; }
        protected string InfraToolsDir => CloudConfigurationManager.GetSetting("InfraToolsRootFolder");

        //private const string PathTo7Zip = @"d:\bin\7z.exe";
        private string PathTo7Zip => Path.Combine(InfraToolsDir, @"7z\7z.exe");

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private string SourceConsolePath => Path.Combine(AssemblyDirectory, InitialUpdatingConsoleName);
        private string TargetConsolePath => Path.Combine(AppRootFolder, ComponentName + ".exe");

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
            ensureUpdatedConsole().Wait();

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


            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("TheBallRole has been started");

            return result;
        }

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

        private async Task ensureUpdatedConsole()
        {
            if (RoleType == AzureRoleType.WorkerRole)
            {
                if (!File.Exists(TargetConsolePath))
                {
                    File.Copy(SourceConsolePath, TargetConsolePath);
                }
                //var appManager = new AppManager(TargetConsolePath, AppConfigPath);
                //await runUpdater(appManager);
            }
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
            if (RoleType == AzureRoleType.WebRole)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // TODO Polling update and launching
                    // Poll or exit on cancel
                    try
                    {
                        await Task.Delay(30000, cancellationToken);
                    }
                    catch (TaskCanceledException) // Expected
                    {

                    }
                }
                return;
            }
            try
            {
                Trace.TraceInformation("Working");
                AppManager currManager = new AppManager(TargetConsolePath, AppConfigPath);
                bool clientExited = false;
                EventHandler setUpdateNeeded = (sender, args) =>
                {
                    Trace.TraceInformation("Client exited - possibly due to updating need");
                    clientExited = true;
                };
                while (!cancellationToken.IsCancellationRequested)
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
                            await currManager.StartAppConsole(false, true, setUpdateNeeded);
                        }
                        else
                        {
                            await currManager.ShutdownAppConsole(true);
                            throw new InvalidOperationException($"Unhandled exit of client with exit code: {currManager.LatestExitCode.GetValueOrDefault(Int32.MinValue)}");
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
                await currManager.ShutdownAppConsole(true);
            }
            catch (Exception exception)
            {
                File.WriteAllText(Path.Combine(AppRootFolder, "RunError.txt"), exception.ToString());
                throw;
            }
        }
    }
}
