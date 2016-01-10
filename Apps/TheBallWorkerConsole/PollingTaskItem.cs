using System.Threading;
using System.Threading.Tasks;

namespace TheBall.Infra.TheBallWorkerConsole
{
    internal class PollingTaskItem
    {
        public readonly Task Task;
        public readonly InstancePollItem InstancePollItem;
        public readonly CancellationTokenSource CancellationTokenSource;
        public readonly SemaphoreSlim ThrottlingSemaphore;

        public PollingTaskItem(InstancePollItem instancePollItem, Task pollingTask,
            CancellationTokenSource cancellationTokenSource, SemaphoreSlim throttlingSemaphore)
        {
            InstancePollItem = instancePollItem;
            Task = pollingTask;
            CancellationTokenSource = cancellationTokenSource;
            ThrottlingSemaphore = throttlingSemaphore;
        }

    }
}