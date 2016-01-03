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
                var instances = workerConfig.InstancePollItems;

                var instanceNames = workerConfig.InstancePollItems.Select(item => item.InstanceHostName).ToArray();

                string startupLogPath = Path.Combine(AssemblyDirectory, "ConsoleStartupLog.txt");
                var startupMessage = "Starting up process (UTC): " + DateTime.UtcNow.ToString() + " for instances: " + String.Join(", ", instanceNames);
                File.WriteAllText(startupLogPath, startupMessage);

                var pipeMessageAwaitable = reader?.ReadToEndAsync();

                bool keepWorkerRunning = true;
                var currentTasks = instances.Select(getInstancePollingTask).ToArray();

                while (keepWorkerRunning)
                {
                    List<Task> awaitList = new List<Task>();
                    if (pipeMessageAwaitable != null)
                        awaitList.Add(pipeMessageAwaitable);
                    awaitList.AddRange(currentTasks.Select(item => item.Item2));

                    await Task.WhenAny(awaitList);

                    bool isCanceling = pipeMessageAwaitable != null && pipeMessageAwaitable.IsCompleted;
                    if (!isCanceling)
                    {
                        var completedPollingTasks = awaitList.Where(item => item != pipeMessageAwaitable && item.IsCompleted).ToArray();
                        currentTasks = replaceCompletedTasks(currentTasks, completedPollingTasks);
                    }
                    else
                    {
                        foreach (var taskItem in currentTasks)
                            taskItem.Item3.Cancel();

                        await Task.WhenAll(awaitList);
                        var completedPollingTasks = awaitList.Where(item => item != pipeMessageAwaitable && !item.IsCanceled).ToArray();

                        // Cancel & allow processing of all lock-obtained and thus completed
                        var pipeMessage = pipeMessageAwaitable.Result;
                        var shutdownLogPath = Path.Combine(AssemblyDirectory, "ConsoleShutdownLog.txt");
                        File.WriteAllText(shutdownLogPath,
                            "Quitting for message (UTC): " + pipeMessage + " " + DateTime.UtcNow.ToString());
                        keepWorkerRunning = false;
                    }
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

        private static Tuple<InstancePollItem, Task, CancellationTokenSource>[] replaceCompletedTasks(
            Tuple<InstancePollItem, Task, CancellationTokenSource>[] currentTasks, Task[] completedPollingTasks)
        {
            var toReplace = currentTasks.Where(item => completedPollingTasks.Contains(item.Item2)).ToArray();
            var toKeep = currentTasks.Where(item => toReplace.Contains(item) == false).ToArray();
            var replacements = toReplace.Select(item =>
            {
                var instancePollItem = item.Item1;
                return getInstancePollingTask(instancePollItem);
            }).ToArray();
            return toKeep.Concat(replacements).ToArray();
        }

        private static Tuple<InstancePollItem, Task, CancellationTokenSource> getInstancePollingTask(InstancePollItem instancePollItem)
        {
            var pollingTask = getPollingTask(instancePollItem);
            return new Tuple<InstancePollItem, Task, CancellationTokenSource>(instancePollItem, pollingTask.Item1, pollingTask.Item2);
        }

        private static Tuple<Task, CancellationTokenSource> getPollingTask(InstancePollItem instancePollItem)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Task result = Task.Run(async () =>
            {

            }, cancellationTokenSource.Token);
            return new Tuple<Task, CancellationTokenSource>(result, cancellationTokenSource);
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
