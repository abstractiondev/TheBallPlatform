using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;
using TheBall.Interface;

namespace TheBall.Infra.TheBallWorkerConsole
{
    public class WorkerSupervisor
    {
        private readonly string ConfigRootFolder;
        private readonly Stream HostPollingStream;
        private readonly WorkerConsoleConfig WorkerConfig;

        private WorkerSupervisor(Stream hostPollingStream, WorkerConsoleConfig workerConfig, string configRootFolder)
        {
            HostPollingStream = hostPollingStream;
            WorkerConfig = workerConfig;
            ConfigRootFolder = configRootFolder;
        }

        internal static async Task<WorkerSupervisor> Create(Stream hostPollingStream, string workerConfigFullPath)
        {
            var workerConfig = await WorkerConsoleConfig.GetConfig(workerConfigFullPath);
            string configRootFolder = Path.GetDirectoryName(workerConfigFullPath);

            var supervisor = new WorkerSupervisor(hostPollingStream, workerConfig, configRootFolder);
            await supervisor.InitializeRuntime();
            return supervisor;
        }

        private async Task InitializeRuntime()
        {
            var infraConfigFullPath = Path.Combine(ConfigRootFolder, "InfraShared", "InfraConfig.json");
            await RuntimeConfiguration.InitializeRuntimeConfigs(infraConfigFullPath);
        }

        internal async Task RunWorkerLoop()
        {
            var pipeStream = HostPollingStream;
            var reader = pipeStream != null ? new StreamReader(pipeStream) : null;
            const int MaxParallelExecutingTasks = 3;
            try
            {
                var instances = WorkerConfig.InstancePollItems;

                var instanceNames = WorkerConfig.InstancePollItems.Select(item => item.InstanceHostName).ToArray();

                string startupLogPath = Path.Combine(Program.AssemblyDirectory, "ConsoleStartupLog.txt");
                var startupMessage = "Starting up process (UTC): " + DateTime.UtcNow.ToString() + " for instances: " +
                                     String.Join(", ", instanceNames);
                File.WriteAllText(startupLogPath, startupMessage);

                var pipeMessageAwaitable = reader?.ReadToEndAsync();

                bool keepWorkerRunning = true;
                SemaphoreSlim throttlingSemaphore = new SemaphoreSlim(MaxParallelExecutingTasks);
                var pollingTaskItems = instances.Select(instance => getPollingTaskItem(instance, throttlingSemaphore)).ToArray();
                List<Task> executingOperations = new List<Task>();

                while (keepWorkerRunning)
                {
                    List<Task> awaitList = new List<Task>();
                    if (pipeMessageAwaitable != null)
                        awaitList.Add(pipeMessageAwaitable);
                    awaitList.AddRange(pollingTaskItems.Select(item => item.Task));
                    awaitList.AddRange(executingOperations);
                    await Task.WhenAny(awaitList);

                    bool isCanceling = pipeMessageAwaitable != null && pipeMessageAwaitable.IsCompleted;
                    if (!isCanceling)
                    {
                        var completedPollingTaskItems = pollingTaskItems
                            .Where(item => item.Task.IsCompleted).ToArray();
                        executingOperations.RemoveAll(item => item.IsCompleted);
                        Task[] executionTasks =
                            completedPollingTaskItems.Select(item => getOperationExecutionTask(item, throttlingSemaphore)).ToArray();
                        executingOperations.AddRange(executionTasks);
                        pollingTaskItems = refreshPollingTasks(pollingTaskItems, completedPollingTaskItems.Select(item => item.Task).ToArray());
                    }
                    else
                    {
                        foreach (var taskItem in pollingTaskItems)
                            taskItem.CancellationTokenSource.Cancel();

                        await Task.WhenAll(awaitList);
                        var completedPollingTaskItems =
                            pollingTaskItems.Where(item => !item.Task.IsCanceled).ToArray();

                        await releaseLocks(completedPollingTaskItems);

                        // Cancel & allow processing of all lock-obtained and thus completed
                        var pipeMessage = pipeMessageAwaitable.Result;
                        var shutdownLogPath = Path.Combine(Program.AssemblyDirectory, "ConsoleShutdownLog.txt");
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

        private static LockInterfaceOperationsByOwnerReturnValue getLockItemFromTask(Task task)
        {
            var typedTask = (Task<LockInterfaceOperationsByOwnerReturnValue>) task;
            return typedTask.Result;
        }

        private async Task releaseLocks(PollingTaskItem[] pollingTaskItems)
        {
            var releaseTasks = pollingTaskItems
                    .Select(item =>
                    {
                        var typedTask = (Task<LockInterfaceOperationsByOwnerReturnValue>) item.Task;
                        var lockItem = typedTask.Result;
                        var instanceName = item.InstancePollItem.InstanceHostName;
                        InformationContext.InitializeToLogicalContext(SystemOwner.CurrentSystem, instanceName);
                        var blobPath = lockItem.LockBlobFullPath;
                        var blob = StorageSupport.GetOwnerBlobReference(SystemOwner.CurrentSystem, blobPath);
                        return blob.DeleteIfExistsAsync();
                    }).ToArray();
            await Task.WhenAll(releaseTasks);
        }

        private static PollingTaskItem[] refreshPollingTasks(PollingTaskItem[] currentTasks, Task[] completedPollingTasks)
        {
            var toReplace = currentTasks.Where(item => completedPollingTasks.Contains(item.Task)).ToArray();
            var toKeep = currentTasks.Where(item => toReplace.Contains(item) == false).ToArray();
            var replacements = toReplace.Select(item => getPollingTaskItem(item.InstancePollItem, item.ThrottlingSemaphore)).ToArray();
            return toKeep.Concat(replacements).ToArray();
        }

        private static PollingTaskItem getPollingTaskItem(InstancePollItem instancePollItem, SemaphoreSlim throttlingSemaphore)
        {
            var pollingTaskTuple = getPollingTask(instancePollItem, throttlingSemaphore);
            return new PollingTaskItem(instancePollItem, pollingTaskTuple.Item1,
                pollingTaskTuple.Item2, throttlingSemaphore);
        }

        private static Tuple<Task, CancellationTokenSource> getPollingTask(InstancePollItem instancePollItem, SemaphoreSlim throttlingSemaphore)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            Task result = Task.Run(async () =>
            {
                InformationContext.InitializeToLogicalContext(SystemOwner.CurrentSystem, instancePollItem.InstanceHostName);
                try
                {
                    bool keepRunning = !cancellationToken.IsCancellationRequested;
                    while (keepRunning)
                    {
                        Console.WriteLine("Polling..." + DateTime.Now.ToLongTimeString());
                        await throttlingSemaphore.WaitAsync(cancellationToken);
                        keepRunning = !cancellationToken.IsCancellationRequested;
                        if (keepRunning)
                        {
                            var obtainedLock = await LockInterfaceOperationsByOwner.ExecuteAsync();
                            if (obtainedLock != null)
                            {
                                Console.WriteLine("Found: " + obtainedLock.OperationIDs.Length + " operations...");
                                return obtainedLock;
                                    // Keeps throttlingSemaphore handle locked => will release after execution completes
                            }
                        }
                        throttlingSemaphore.Release();
                        await Task.Delay(1000, cancellationToken);
                        keepRunning = !cancellationToken.IsCancellationRequested;
                    }
                    return null;
                }
                finally
                {
                    InformationContext.ProcessAndClearCurrentIfAvailable();
                }
            }, cancellationTokenSource.Token);
            return new Tuple<Task, CancellationTokenSource>(result, cancellationTokenSource);
        }

        private Task getOperationExecutionTask(PollingTaskItem pollingTaskItem, SemaphoreSlim throttlingSemaphore)
        {
            var typedTask = (Task<LockInterfaceOperationsByOwnerReturnValue>) pollingTaskItem.Task;
            var lockItem = typedTask.Result;

            string instanceName = pollingTaskItem.InstancePollItem.InstanceHostName;
            var lockedOwnerPrefix = lockItem.LockedOwnerPrefix;
            var lockedOwnerID = lockItem.LockedOwnerID;
            var lockBlobFullPath = lockItem.LockBlobFullPath;
            var operationIDs = lockItem.OperationIDs;
            //var operationOwner = new VirtualOwner(lockedOwnerPrefix, lockedOwnerID);
            Task result = Task.Run(async () =>
            {
                try
                {
                    //InformationContext.InitializeToLogicalContext(operationOwner, instanceName);
                    await
                        ExecuteInterfaceOperationsByOwnerAndReleaseLock.ExecuteAsync(new ExecuteInterfaceOperationsByOwnerAndReleaseLockParameters
                        {
                            InstanceName = instanceName,
                            LockBlobFullPath = lockBlobFullPath,
                            LockedOwnerPrefix = lockedOwnerPrefix,
                            LockedOwnerID = lockedOwnerID,
                            OperationIDs = operationIDs
                        });
                    //InformationContext.ProcessAndClearCurrentIfAvailable();
                }
                finally
                {
                    throttlingSemaphore.Release();
                }
            });
            return result;
        }

    }
}