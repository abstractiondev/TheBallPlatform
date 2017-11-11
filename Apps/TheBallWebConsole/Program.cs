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
using CommandLine.Options;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Nito.AsyncEx;
using TheBall.CORE.Storage;
using TheBall.Infra.AppUpdater;
using TheBall.Infra.TheBallWebConsole;

namespace TheBall.Infra.TheBallWebConsole
{
    class Program
    {
        const int UpdatePollingIntervalMs = 20000;
        public static AppUpdateManager UpdateManager;
        private static int ExitCode = 0;

        const string ComponentName = "TheBallWebConsole";
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
                string applicationConfigFullPath = null;
                string updateAccessInfoFile = null;
                bool autoUpdate = false;
                string clientHandle = null;
                string tempSiteRootDir = null;
                string appSiteRootDir = null;
                var optionSet = new OptionSet()
                {
                    {
                        "ac|applicationConfig=", "Application config full path",
                        (key, ac) => applicationConfigFullPath = ac
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
                        "tempsiterootdir=", "TempSite root dir location for preparing site update packages",
                        (key, tsrd) => tempSiteRootDir = tsrd
                    },
                    {
                        "appsiterootdir=", "AppSite root dir location for deploying site update packages",
                        (key, asrd) => appSiteRootDir = asrd
                    },
                    {
                        "envcfg|useEnvConfig", "Use environment variables as config instead of default CloudConfigurationManager",
                        (key, env) => UseEnvironmentVariablesAsConfig = env != null
                    }
                };
                var options = optionSet.Parse(args);
                bool hasExtraOptions = options.Count > 0;
                bool isMissingMandatory = (String.IsNullOrEmpty(applicationConfigFullPath) 
                    || String.IsNullOrEmpty(tempSiteRootDir) || String.IsNullOrEmpty(appSiteRootDir)
                    ) && !isTestMode;
                bool hasIdentifiedOptions = optionSet.Count > 0;
                if (hasExtraOptions || isMissingMandatory)
                {
                    Console.WriteLine($"Usage: {ComponentName}.exe");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    optionSet.WriteOptionDescriptions(Console.Out);
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
                AsyncContext.Run(() => MainAsync(clientHandle, applicationConfigFullPath, isTestMode, updateAccessInfo, tempSiteRootDir, appSiteRootDir));
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

        static async void MainAsync(string clientHandle, string applicationConfigFullPath, bool isTestMode, AccessInfo updateAccessInfo, 
            string tempSiteRootDir, string appSiteRootDir)
        {
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.DefaultConnectionLimit = 500;
            ServicePointManager.Expect100Continue = false;

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

            if (applicationConfigFullPath == null && isTestMode == false)
                throw new NotSupportedException("Either ApplicationConfigFullPath or isTestMode must be given");

            ensureXDrive();

            var pipeStream = clientHandle != null
                ? new AnonymousPipeClientStream(PipeDirection.In, clientHandle)
                : null;
            var webManager = await WebManager.Create(pipeStream, applicationConfigFullPath, isTestMode, tempSiteRootDir, appSiteRootDir);
            try
            {
                if (isTestMode)
                    await webManager.WaitHandleExitCommand(10);
                else
                {
                    var changeDetector = autoUpdate ? ChangeDetector.Create(new[]
                    {
                        new ChangeMonitoredItem(@"X:\Configs\WebConsole.json"),
                        UpdateManager.GetUpdateConfigChangeMonitoredItem(),
                    }) : null;
                    await webManager.RunUpdateLoop(changeDetector?.MonitorItemsAsync(UpdatePollingIntervalMs));
                }
            }
            catch (Exception ex)
            {
                webManager.AppInsightsClient?.TrackException(ex);
                throw;
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
                    var startInfo = new ProcessStartInfo(netPath) { UseShellExecute = false, Arguments = args };
                    var netProc = new Process { StartInfo = startInfo };
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
