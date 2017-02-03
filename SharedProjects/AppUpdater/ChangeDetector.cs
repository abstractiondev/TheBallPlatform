using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.File;

namespace TheBall.Infra.AppUpdater
{
    public class ChangeMonitoredItem
    {
        public ChangeMonitoredItem(string filePath, CloudFileShare cloudShare = null)
        {
            FilePath = filePath;
            CloudShare = cloudShare;
        }

        public readonly string FilePath;
        public readonly CloudFileShare CloudShare;
        public DateTime LastChangedUtc;
    }

    public class ChangeDetector
    {
        private CancellationTokenSource _cancellationSource;
        public ChangeMonitoredItem[] ItemsToMonitor { get; private set; }
        public static ChangeDetector Create(ChangeMonitoredItem[] itemsToMonitor)
        {
            var detector = new ChangeDetector {ItemsToMonitor = itemsToMonitor};
            return detector;
        }

        private async Task<ChangeMonitoredItem> pollAndUpdateItem(ChangeMonitoredItem item)
        {
            DateTime lastModifiedUtcTime;
            var cloudShare = item.CloudShare;
            if (cloudShare != null)
            {
                var cloudFile = cloudShare.GetRootDirectoryReference().GetFileReference(item.FilePath);
                await cloudFile.FetchAttributesAsync();
                lastModifiedUtcTime = cloudFile.Properties.LastModified.GetValueOrDefault().UtcDateTime;
            }
            else // Filesystem poll
            {
                lastModifiedUtcTime = File.GetLastWriteTimeUtc(item.FilePath);
            }
            if (lastModifiedUtcTime != item.LastChangedUtc)
            {
                item.LastChangedUtc = lastModifiedUtcTime;
                return item;
            }
            return null;
        }

        private async Task<ChangeMonitoredItem[]> pollAndUpdateItems()
        {
            var updateTasks = ItemsToMonitor.Select(pollAndUpdateItem).ToArray();
            await Task.WhenAll(updateTasks);
            var updatedItems = updateTasks.Where(task => task.Result != null).Select(task => task.Result).ToArray();
            return updatedItems;
        }

        public void StopMonitoring()
        {
            if(_cancellationSource == null)
                throw new InvalidOperationException("MonitorItemsAsync not running");
            _cancellationSource.Cancel();
            _cancellationSource = null;
        }
        public async Task<ChangeMonitoredItem[]> MonitorItemsAsync(int pollingIntervalMs)
        {
            if(_cancellationSource != null)
                throw new InvalidOperationException("MonitorItemsAsync already running");
            _cancellationSource = new CancellationTokenSource();
            var cancellationToken = _cancellationSource.Token;
            var initialChanged = await pollAndUpdateItems();
            while (!cancellationToken.IsCancellationRequested)
            {
                var changedItems = await pollAndUpdateItems();
                if (changedItems.Any())
                    return changedItems;
                try
                {
                    await Task.Delay(pollingIntervalMs, cancellationToken);
                }
                catch(TaskCanceledException)
                {
                    
                }
            }
            return null;
        }
    }
}