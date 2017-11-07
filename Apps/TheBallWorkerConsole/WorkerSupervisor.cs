using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
//using Microsoft.VisualBasic.Devices;
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
        private bool IsTestMode = false;
        internal TelemetryClient AppInsightsClient { get; private set; }

        private WorkerSupervisor(Stream hostPollingStream, WorkerConsoleConfig workerConfig, string configRootFolder)
        {
            HostPollingStream = hostPollingStream;
            WorkerConfig = workerConfig;
            ConfigRootFolder = configRootFolder;
        }

        private WorkerSupervisor(Stream hostPollingStream)
        {
            HostPollingStream = hostPollingStream;
            IsTestMode = true;
        }

        internal static async Task<WorkerSupervisor> Create(Stream hostPollingStream, string workerConfigFullPath, bool isTestMode)
        {
            WorkerSupervisor supervisor;
            if (isTestMode)
            {
                supervisor = new WorkerSupervisor(hostPollingStream);
            }
            else
            {
                var workerConfig = await WorkerConsoleConfig.GetConfig(workerConfigFullPath);
                string configRootFolder = Path.GetDirectoryName(workerConfigFullPath);
                supervisor = new WorkerSupervisor(hostPollingStream, workerConfig, configRootFolder);
                await supervisor.InitializeRuntime();
            }
            return supervisor;
        }

        private async Task InitializeRuntime()
        {
            var infraConfigFullPath = Path.Combine(ConfigRootFolder, "InfraShared", "InfraConfig.json");
            await RuntimeConfiguration.InitializeRuntimeConfigs(infraConfigFullPath);
            TelemetryConfiguration.Active.InstrumentationKey =
                InfraSharedConfig.Current.AppInsightInstrumentationKey;
            AppInsightsClient = new TelemetryClient();
            RuntimeSupport.ExceptionReportHandler =
                (exception, properties) => AppInsightsClient.TrackException(exception, properties);
        }

        internal async Task WaitHandleExitCommand(int timeoutSeconds)
        {
            var pipeStream = HostPollingStream;
            var reader = pipeStream != null ? new StreamReader(pipeStream) : null;
            bool keepWorkerRunning = true;
            var pipeMessageAwaitable = reader?.ReadToEndAsync();
            while (keepWorkerRunning)
            {
                List<Task> awaitList = new List<Task>();
                awaitList.Add(pipeMessageAwaitable);
                awaitList.Add(Task.Delay(timeoutSeconds * 1000));
                await Task.WhenAny(awaitList);
                if (pipeMessageAwaitable.IsCompleted)
                    keepWorkerRunning = false;
                else
                    throw new InvalidOperationException($"Test cancel not happened within {timeoutSeconds} second timeout");
            }
        }

        internal async Task RunWorkerLoop(Task autoUpdateTask, string dedicatedToInstance, string dedicatedToOwnerPrefix)
        {
            var pipeStream = HostPollingStream;
            var reader = pipeStream != null ? new StreamReader(pipeStream) : null;
            int MaxParallelExecutingTasks = getAutoConfiguredWorkerCount();
            try
            {
                var instances = WorkerConfig.InstancePollItems;
                IContainerOwner dedicatedToOwner = null;
                if (dedicatedToInstance != null)
                {
                    instances = instances.Where(item => item.InstanceHostName == dedicatedToInstance).ToArray();
                    dedicatedToOwner = VirtualOwner.FigureOwner(dedicatedToOwnerPrefix);
                }

                var instanceNames = instances.Select(item => item.InstanceHostName).ToArray();

                string startupLogPath = Path.Combine(Program.AssemblyDirectory, "ConsoleStartupLog.txt");
                var startupMessage = "Starting up process (UTC): " + DateTime.UtcNow.ToString() + " for instances: " +
                                     String.Join(", ", instanceNames);
                File.WriteAllText(startupLogPath, startupMessage);
                AppInsightsClient.TrackEvent("WorkerStartup", new Dictionary<string, string>
                {
                    { "Instances", String.Join(", ", instanceNames) }
                });

                var pipeMessageAwaitable = reader?.ReadToEndAsync();

                bool keepWorkerRunning = true;
                SemaphoreSlim throttlingSemaphore = new SemaphoreSlim(MaxParallelExecutingTasks);
                var pollingTaskItems =
                    instances.Select(instance => getPollingTaskItem(instance, throttlingSemaphore, dedicatedToOwner))
                        .ToArray();
                List<Task> executingOperations = new List<Task>();

                while (keepWorkerRunning)
                {
                    List<Task> awaitList = new List<Task>();
                    if (pipeMessageAwaitable != null)
                        awaitList.Add(pipeMessageAwaitable);
                    if (autoUpdateTask != null)
                        awaitList.Add(autoUpdateTask);
                    awaitList.AddRange(pollingTaskItems.Select(item => item.Task));
                    awaitList.AddRange(executingOperations);
                    await Task.WhenAny(awaitList);

                    bool isCanceling = pipeMessageAwaitable != null && pipeMessageAwaitable.IsCompleted;
                    bool isUpdating = autoUpdateTask != null && autoUpdateTask.IsCompleted;
                    if (!isCanceling && !isUpdating)
                    {
                        var completedPollingTaskItems = pollingTaskItems
                            .Where(item => item.Task.IsCompleted).ToArray();
                        executingOperations.RemoveAll(item => item.IsCompleted);
                        Task[] executionTasks =
                            completedPollingTaskItems.Select(
                                item => getOperationExecutionTask(item, throttlingSemaphore)).ToArray();
                        executingOperations.AddRange(executionTasks);
                        pollingTaskItems = refreshPollingTasks(pollingTaskItems,
                            completedPollingTaskItems.Select(item => item.Task).ToArray());
                    }
                    else // isCanceling or isUpdating
                    {
                        //if(isUpdating)
                        //    Debugger.Launch();
                        // Cancel all polling
                        foreach (var taskItem in pollingTaskItems)
                            taskItem.CancellationTokenSource.Cancel();

                        // Remove uninteresting waits
                        awaitList.Remove(pipeMessageAwaitable);
                        awaitList.Remove(autoUpdateTask);

                        await Task.WhenAll(awaitList);
                        var lockObtainedPollingTaskItems =
                            pollingTaskItems.Where(item => getLockItemFromTask(item.Task) != null).ToArray();

                        await releaseLocks(lockObtainedPollingTaskItems);

                        // Cancel & allow processing of all lock-obtained and thus completed
                        var pipeMessage = isCanceling ? pipeMessageAwaitable?.Result : null;
                        var shutdownLogPath = Path.Combine(Program.AssemblyDirectory, "ConsoleShutdownLog.txt");
                        File.WriteAllText(shutdownLogPath,
                            "Quitting for message (UTC): " + pipeMessage ?? "Updating" + " " + DateTime.UtcNow.ToString());
                        keepWorkerRunning = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ReportException();
                throw;
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

        private int getAutoConfiguredWorkerCount()
        {
            //ComputerInfo computerInfo = new ComputerInfo();
            ulong gigaByte = 1024*1024 * 1024;
            var totalMemory = 2 * gigaByte;  //computerInfo.TotalPhysicalMemory;
            if (totalMemory < gigaByte) // Extra Small in Azure
                return 3;
            if (totalMemory < 2*gigaByte) // Small
                return 10;
            if (totalMemory < 4*gigaByte) // A2 or D1
                return 20;
            return 20; // Something bigger
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
                        InformationContext.InitializeToLogicalContext(null, SystemOwner.CurrentSystem, instanceName, null, true);
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
            var replacements = toReplace.Select(item => getPollingTaskItem(item.InstancePollItem, item.ThrottlingSemaphore, item.DedicatedToOwner)).ToArray();
            return toKeep.Concat(replacements).ToArray();
        }

        private static PollingTaskItem getPollingTaskItem(InstancePollItem instancePollItem, SemaphoreSlim throttlingSemaphore, IContainerOwner dedicatedToOwner)
        {
            var pollingTaskTuple = getPollingTask(instancePollItem, throttlingSemaphore, dedicatedToOwner);
            return new PollingTaskItem(instancePollItem, pollingTaskTuple.Item1,
                pollingTaskTuple.Item2, throttlingSemaphore, dedicatedToOwner);
        }

        private static Tuple<Task, CancellationTokenSource> getPollingTask(InstancePollItem instancePollItem, SemaphoreSlim throttlingSemaphore, IContainerOwner dedicatedToOwner)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            Task result = Task.Run(async () =>
            {
                InformationContext.InitializeToLogicalContext(null, SystemOwner.CurrentSystem, instancePollItem.InstanceHostName, null, true);
                try
                {
                    bool keepRunning = !cancellationToken.IsCancellationRequested;
                    while (keepRunning)
                    {
                        Console.WriteLine("Polling..." + DateTime.Now.ToLongTimeString());
                        try
                        {
                            await throttlingSemaphore.WaitAsync(cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                            
                        }
                        keepRunning = !cancellationToken.IsCancellationRequested;
                        if (keepRunning)
                        {
                            var obtainedLock = await LockInterfaceOperationsByOwner.ExecuteAsync(new LockInterfaceOperationsByOwnerParameters
                            {
                                DedicatedToOwner = dedicatedToOwner
                            });
                            if (obtainedLock != null)
                            {
                                Console.WriteLine("Found: " + obtainedLock.OperationIDs.Length + " operations...");
                                return obtainedLock;
                                    // Keeps throttlingSemaphore handle locked => will release after execution completes
                            }
                        }
                        throttlingSemaphore.Release();
                        try
                        {
                            await Task.Delay(1000, cancellationToken);
                        }
                        catch (TaskCanceledException)
                        {
                            
                        }
                        keepRunning = !cancellationToken.IsCancellationRequested;
                    }
                    return null;
                }
                finally
                {
                    await InformationContext.ProcessAndClearCurrentIfAvailableAsync();
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
            var operationQueueItems = lockItem.OperationQueueItems;
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
                            OperationQueueItems = operationQueueItems,
                            OperationIDs = operationIDs
                        });
                    //InformationContext.ProcessAndClearCurrentIfAvailable();
                }
                catch (Exception ex)
                {
                    var trackProperties = new Dictionary<string, string>
                    {
                        { "Instance", instanceName },
                        { "Owner", lockedOwnerPrefix },
                        { "OperationIDs", String.Join(", ", operationIDs) }
                    };
                    ex.ReportException(trackProperties);
                    throw;
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