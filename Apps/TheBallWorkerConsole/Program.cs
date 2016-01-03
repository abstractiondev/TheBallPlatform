using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure;
using Nito.AsyncEx;

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
            var configFullPath = args.Length > 0 ? args[0] : null;
            if (configFullPath == null)
                throw new ArgumentNullException(nameof(args), "Config full path cannot be null (first  argument)");

            var clientHandle = args.Length > 1 ? args[1] : null;

            var workerConfig = await WorkerConsoleConfig.GetConfig(configFullPath);

            var pipeStream = clientHandle != null
                ? new AnonymousPipeClientStream(PipeDirection.In, clientHandle)
                : null;
            var reader = pipeStream != null ? new StreamReader(pipeStream) : null;
            try
            {
                const int ConcurrentTasks = 3;
                Task[] activeTasks = new Task[ConcurrentTasks];
                int nextFreeIX = 0;

                var instanceNames = workerConfig.InstancePollItems.Select(item => item.InstanceHostName).ToArray();

                string startupLogPath = Path.Combine(AssemblyDirectory, "ConsoleStartupLog.txt");
                var startupMessage = "Starting up process (UTC): " + DateTime.UtcNow.ToString() + " for instances: " + String.Join(", ", instanceNames);
                File.WriteAllText(startupLogPath, startupMessage);

                var pipeMessageAwaitable = reader != null ? reader.ReadToEndAsync() : null;

                List<Task> awaitList = new List<Task>();
                if(pipeMessageAwaitable != null)
                    awaitList.Add(pipeMessageAwaitable);

                while (true)
                {

                    await Task.WhenAny(awaitList);
                    if (pipeMessageAwaitable != null && pipeMessageAwaitable.IsCompleted)
                    {
                        var pipeMessage = pipeMessageAwaitable.Result;
                        var shutdownLogPath = Path.Combine(AssemblyDirectory, "ConsoleShutdownLog.txt");
                        File.WriteAllText(shutdownLogPath,
                            "Quitting for message (UTC): " + pipeMessage + " " + DateTime.UtcNow.ToString());
                        break;
                    }
                    //await Task.WhenAny(activeTasks);
                }
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
