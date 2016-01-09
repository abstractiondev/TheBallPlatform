using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Nito.AsyncEx;
using TheBall.CORE.InstanceSupport;

namespace TheBall.Infra.TheBallWorkerConsole
{
    class Program
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


        static void Main(string[] args)
        {
            try
            {
                AsyncContext.Run(() => MainAsync(args));
            }
            catch (Exception exception)
            {
                var errorFile = Path.Combine(AssemblyDirectory, "ConsoleErrorLog.txt");
                File.WriteAllText(errorFile, exception.ToString());
                throw;
            }
        }

        static async void MainAsync(string[] args)
        {
            ensureXDrive();
            var workerConfigFullPath = args.Length > 0 ? args[0] : null;
            if (workerConfigFullPath == null)
                throw new ArgumentNullException(nameof(args), "Config full path cannot be null (first  argument)");
            
            var configDirPath = new FileInfo(workerConfigFullPath).Directory.FullName;
            var infraConfigFullPath = Path.Combine(configDirPath, "InfraShared", "InfraConfig.json");
            await RuntimeConfiguration.InitializeRuntimeConfigs(infraConfigFullPath);

            var clientHandle = args.Length > 1 ? args[1] : null;


            var pipeStream = clientHandle != null
                ? new AnonymousPipeClientStream(PipeDirection.In, clientHandle)
                : null;
            var supervisor = await WorkerSupervisor.Create(pipeStream, workerConfigFullPath);
            await supervisor.RunWorkerLoop(pipeStream, workerConfigFullPath);
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
