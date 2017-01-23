using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NDesk.Options;
using Microsoft.Azure;
using Nito.AsyncEx;
using TheBall.CORE.InstanceSupport;
using TheBall.Infra.AppUpdater;

namespace TheBall.Infra.TheBallWorkerConsole
{
    class Program
    {
        public static AppUpdateManager UpdateManager;
        private static int ExitCode = 0;

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


        static int Main(string[] args)
        {
            try
            {
                bool isTestMode = false;
                string dedicatedToOwner = null;
                string workerConfigFullPath = null;
                bool autoUpdate = false;
                string clientHandle = null;
                var optionSet = new OptionSet()
                {
                    {
                        "wc|workerConfig=", "Worker config full path",
                        wc => workerConfigFullPath = wc
                    },
                    {
                        "au|autoupdate", "Auto update worker",
                        au => autoUpdate = au != null
                    },
                    {
                        "ch|clientHandle=", "Client handle to poll for exit requests from launching process",
                        ch => clientHandle = ch
                    },
                    {
                        "t|test", "Test handle communication and update, but don't activate the real worker process",
                        t => isTestMode = t != null
                    },
                    {
                        "o|owner=", "Dedicated to owner",
                        owner => dedicatedToOwner = owner
                    }
                };
                var options = optionSet.Parse(args);
                bool hasExtraOptions = options.Count > 0;
                bool isMissingMandatory = workerConfigFullPath == null && !isTestMode;
                bool hasIdentifiedOptions = optionSet.Count > 0;
                if (hasExtraOptions || isMissingMandatory)
                {
                    Console.WriteLine("Usage: TheBallWorkerConsole.exe");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    optionSet.WriteOptionDescriptions(Console.Out);
                    return -1;
                }
                AsyncContext.Run(() => MainAsync(clientHandle, workerConfigFullPath, dedicatedToOwner, isTestMode, autoUpdate));
            }
            catch (Exception exception)
            {
                var errorFile = Path.Combine(AssemblyDirectory, "ConsoleErrorLog.txt");
                File.WriteAllText(errorFile, exception.ToString());
                Console.WriteLine("Top Exception Handler: ");
                Console.WriteLine(exception.ToString());
                return -2;
            }
            return ExitCode;
        }

        static async void MainAsync(string clientHandle, string workerConfigFullPath, string dedicatedToOwner, bool isTestMode, bool autoUpdate)
        {
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.DefaultConnectionLimit = 500;
            ServicePointManager.Expect100Continue = false;

            if (autoUpdate)
            {
                string componentName = "TheBallWorkerConsole";
                string workingRootFolder = AssemblyDirectory;
                string accountName = CloudConfigurationManager.GetSetting("ConfigAccountName");
                string shareName = CloudConfigurationManager.GetSetting("ConfigShareName");
                string sasToken = CloudConfigurationManager.GetSetting("ConfigSASToken");
                UpdateManager = await AppUpdateManager.Initialize(componentName, workingRootFolder, new AccessInfo
                {
                    AccountName = accountName,
                    ShareName = shareName,
                    SASToken = sasToken
                });
                bool needsRestart = await UpdateManager.CheckAndProcessUpdate();
                if (needsRestart)
                {
                    ExitCode = 2;
                    return;
                }
            }

            if(workerConfigFullPath == null && isTestMode == false)
                throw new NotSupportedException("Either WorkerConfigFullPath or isTestMode must be given");
            if(workerConfigFullPath != null && !isTestMode)
                throw new NotSupportedException("WorkerConfigFullPath must not be given together with isTestMode");

            ensureXDrive();
            string dedicatedToInstance;
            string dedicatedToOwnerPrefix;
            parseDedicatedParts(dedicatedToOwner, out dedicatedToInstance, out dedicatedToOwnerPrefix);

            var pipeStream = clientHandle != null
                ? new AnonymousPipeClientStream(PipeDirection.In, clientHandle)
                : null;
            var supervisor = await WorkerSupervisor.Create(pipeStream, workerConfigFullPath,isTestMode);
            try
            {
                if (isTestMode)
                    await supervisor.WaitHandleExitCommand(10);
                else
                    await supervisor.RunWorkerLoop(dedicatedToInstance, dedicatedToOwnerPrefix);
            }
            catch (Exception ex)
            {
                supervisor.AppInsightsClient?.TrackException(ex);
                throw;
            }
        }

        private static void parseDedicatedParts(string dedicatedToOwner, out string dedicatedToInstance, out string dedicatedToOwnerPrefix)
        {
            if (dedicatedToOwner != null)
            {
                var split = dedicatedToOwner.Split('_');
                dedicatedToInstance = split[0];
                dedicatedToOwnerPrefix = split[1];
            }
            else
            {
                dedicatedToInstance = null;
                dedicatedToOwnerPrefix = null;
            }
        }

        private static void ensureXDrive()
        {
            bool hasDriveX = DriveInfo.GetDrives().Any(item => item.Name.ToLower().StartsWith("X"));
            if (!hasDriveX)
            {
                var infraAccountName = CloudConfigurationManager.GetSetting("CoreFileShareAccountName");
                var infraAccountKey = CloudConfigurationManager.GetSetting("CoreFileShareAccountKey");
                bool isCloud = infraAccountName != null && infraAccountKey != null;
                if (isCloud)
                {
                    var netPath = Path.Combine(Environment.SystemDirectory, "net.exe");
                    var args = $@"use X: \\{infraAccountName}.file.core.windows.net\tbcore /u:{infraAccountName} {infraAccountKey}";
                    var startInfo = new ProcessStartInfo(netPath) {UseShellExecute = false, Arguments = args};
                    var netProc = new Process {StartInfo = startInfo};
                    netProc.Start();
                    netProc.WaitForExit();
                }
            }
        }
    }
}
