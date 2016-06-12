using System.Threading;
using System.Threading.Tasks;
using TheBall.CORE;

namespace TheBall.Infra.TheBallWorkerConsole
{
    internal class PollingTaskItem
    {
        public readonly Task Task;
        public readonly InstancePollItem InstancePollItem;
        public readonly CancellationTokenSource CancellationTokenSource;
        public readonly SemaphoreSlim ThrottlingSemaphore;
        public readonly IContainerOwner DedicatedToOwner;

        public PollingTaskItem(InstancePollItem instancePollItem, Task pollingTask,
            CancellationTokenSource cancellationTokenSource, SemaphoreSlim throttlingSemaphore, IContainerOwner dedicatedToOwner)
        {
            InstancePollItem = instancePollItem;
            Task = pollingTask;
            CancellationTokenSource = cancellationTokenSource;
            ThrottlingSemaphore = throttlingSemaphore;
            DedicatedToOwner = dedicatedToOwner;
        }

    }
}