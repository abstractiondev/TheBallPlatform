using System;
using System.IO;
using System.Reflection;
using Microsoft.Azure;
using NDesk.Options;
using Nito.AsyncEx;
using TheBall.Infra.AppUpdater;

namespace TheBall.Infra.InitialUpdatingConsole
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
                //Debugger.Launch();
                bool isTestMode = false;
                string applicationConfigFullPath = null;
                bool autoUpdate = false;
                string clientHandle = null;
                var optionSet = new OptionSet()
                {
                    {
                        "ac|applicationConfig=", "Application config full path",
                        ac => applicationConfigFullPath = ac
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
                        "t|test", "Test handle communication and update, but don't activate the real application process",
                        t => isTestMode = t != null
                    }
                };
                var options = optionSet.Parse(args);
                bool hasExtraOptions = options.Count > 0;
                bool isMissingMandatory = applicationConfigFullPath == null && !isTestMode;
                bool hasIdentifiedOptions = optionSet.Count > 0;
                if (hasExtraOptions || isMissingMandatory)
                {
                    Console.WriteLine("Usage: AppNameHere.exe");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    optionSet.WriteOptionDescriptions(Console.Out);
                    return -1;
                }
                AsyncContext.Run(() => MainAsync(clientHandle, applicationConfigFullPath, isTestMode, autoUpdate));
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

        static async void MainAsync(string clientHandle, string applicationConfigFullPath, 
            bool isTestMode, bool autoUpdate)
        {
            if (autoUpdate)
            {
                string componentName = Assembly.GetEntryAssembly().GetName().Name;
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
                }
            }

        }
    }
}
