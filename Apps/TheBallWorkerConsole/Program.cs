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
using Nito.AsyncEx;
using TheBall.CORE.InstanceSupport;
using TheBall.CORE.Storage;
using TheBall.Infra.AppUpdater;

namespace TheBall.Infra.TheBallWorkerConsole
{
    class Program
    {
        const int UpdatePollingIntervalMs = 20000;
        public static AppUpdateManager UpdateManager;
        private static int ExitCode = 0;
        const string ApplicationConfigFullPath = @"X:\Configs\WorkerConsole.json";

        const string ComponentName = "TheBallWorkerConsole";

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
                //Debugger.Launch();
                bool isTestMode = false;
                string dedicatedToOwner = null;
                string applicationConfigFullPath = null;
                string updateAccessInfoFile = null;
                bool autoUpdate = false;
                string clientHandle = null;
                string timeout = null;
            var optionSet = new OptionSet()
            {
                {

                    "ac|applicationConfig=", "Application config full path",
                    ac => applicationConfigFullPath = ac
                },
                /*
                {
                    "o|owner=", "Dedicated to owner",
                    (key, owner) => dedicatedToOwner = owner
                },
                {
                    "au|autoupdate", "Auto update worker",
                    (key, au) => autoUpdate = au != null
                },
                {
                    "upacc|updateAccessFile=", "Update access info file",
                    (key, upacc) => updateAccessInfoFile = upacc
                },
                {
                    "ch|clientHandle=", "Client handle to poll for exit requests from launching process",
                    (key, ch) => clientHandle = ch
                },
                {
                    "t|test", "Test handle communication and update, but don't activate the real worker process",
                    (key, t) => isTestMode = t != null
                },
                {
                    "envcfg|useEnvConfig", //"Use environment variables as config instead of default CloudConfigurationManager",
                    (key, env) => UseEnvironmentVariablesAsConfig = env != null
                },*/
                {
                    "timeout=",
                    to => timeout = to
                }
            };
                var options = optionSet.Parse(args);
                applicationConfigFullPath = applicationConfigFullPath ?? Environment.GetEnvironmentVariable(nameof(ApplicationConfigFullPath)) ?? ApplicationConfigFullPath;
                Console.WriteLine($"Using appconfig from: {applicationConfigFullPath}");
                //var options = args.ToList();
                //List<string> optionSet = new List<string>();
                bool hasExtraOptions = options.Count > 0;
                bool isMissingMandatory = applicationConfigFullPath == null && !isTestMode;
                bool hasIdentifiedOptions = optionSet.Count > 0;
                if (hasExtraOptions || isMissingMandatory)
                {
                    Console.WriteLine($"Usage: {ComponentName}.exe");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    //optionSet.WriteOptionDescriptions(Console.Out);
                    return -1;
                }
                AccessInfo updateAccessInfo = null;
                if (autoUpdate)
                {
                    if (String.IsNullOrEmpty(updateAccessInfoFile))
                    {
                        string accountName = GetConfig("ConfigAccountName");
                        string shareName = GetConfig("ConfigShareName");
                        string sasToken = GetConfig("ConfigSASToken");
                        updateAccessInfo = new AccessInfo
                        {
                            AccountName = accountName,
                            ShareName = shareName,
                            SASToken = sasToken
                        };
                    }
                    else
                    {
                        var fileContent = File.ReadAllBytes(updateAccessInfoFile);
                        updateAccessInfo = JSONSupport.GetObjectFromData<AccessInfo>(fileContent);
                    }

                }
                AsyncContext.Run(() => MainAsync(clientHandle, applicationConfigFullPath, dedicatedToOwner, isTestMode, updateAccessInfo));
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

        static async void MainAsync(string clientHandle, string workerConfigFullPath, string dedicatedToOwner, bool isTestMode, AccessInfo updateAccessInfo)
        {
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.DefaultConnectionLimit = 500;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            bool autoUpdate = updateAccessInfo != null;
            bool isDebugging = Debugger.IsAttached;
            if (autoUpdate)
            {
                string componentName = ComponentName;
                string workingRootFolder = AssemblyDirectory;
                UpdateManager = await AppUpdateManager.Initialize(componentName, workingRootFolder, updateAccessInfo);
                if (!isDebugging)
                {
                    bool needsRestart = await UpdateManager.CheckAndProcessUpdate();
                    if (needsRestart)
                    {
                        ExitCode = 2;
                        return;
                    }
                }
            }

            if(workerConfigFullPath == null && isTestMode == false)
                throw new NotSupportedException("Either WorkerConfigFullPath or isTestMode must be given");

            //ensureXDrive();
            string dedicatedToInstance;
            string dedicatedToOwnerPrefix;
            parseDedicatedParts(dedicatedToOwner, out dedicatedToInstance, out dedicatedToOwnerPrefix);

            var pipeStream = clientHandle != null
                ? new AnonymousPipeClientStream(PipeDirection.In, clientHandle)
                : null;
            var supervisor = await WorkerSupervisor.Create(pipeStream, workerConfigFullPath, isTestMode);
            try
            {
                if (isTestMode)
                    await supervisor.WaitHandleExitCommand(10);
                else
                {
                    var changeDetector = autoUpdate ? ChangeDetector.Create(new[]
                    {
                        new ChangeMonitoredItem(@"X:\Configs\WorkerConsole.json"), 
                        UpdateManager.GetUpdateConfigChangeMonitoredItem(), 
                    }) : null;
                    await supervisor.RunWorkerLoop(changeDetector?.MonitorItemsAsync(UpdatePollingIntervalMs), dedicatedToInstance, dedicatedToOwnerPrefix);
                }
            }
            catch (Exception ex)
            {
                supervisor.AppInsightsClient?.TrackException(ex);
                throw;
            }
        }

        private static void parseDedicatedParts(string dedicatedToOwner, out string dedicatedToInstance, out string dedicatedToOwnerPrefix)
        {
            if (String.IsNullOrEmpty(dedicatedToOwner) == false)
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
            bool hasDriveX = DriveInfo.GetDrives().Any(item => item.Name.ToLower().StartsWith("x"));
            if (!hasDriveX)
            {
                var infraAccountName = GetConfig("CoreFileShareAccountName");
                var infraAccountKey = GetConfig("CoreFileShareAccountKey");
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

        private static string GetConfig(string configItem)
        {
            string configValue = UseEnvironmentVariablesAsConfig
                ? Environment.GetEnvironmentVariable(configItem)
                : throw new NotSupportedException();
            return configValue;
        }

        public static bool UseEnvironmentVariablesAsConfig { get; set; }

    }
}
