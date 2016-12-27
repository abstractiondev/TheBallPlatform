using System.IO;
using System.Threading.Tasks;
using TheBall.CORE.Storage;

namespace TheBall.Infra.TheBallWorkerConsole
{
    public class WorkerConsoleConfig
    {
        public InstancePollItem[] InstancePollItems { get; set; }

        public static async Task<WorkerConsoleConfig> GetConfig(string fullPathToConfig)
        {
            using (var fileStream = File.OpenRead(fullPathToConfig))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                var data = await reader.ReadToEndAsync();
                return JSONSupport.GetObjectFromString<WorkerConsoleConfig>(data);
            }
        }

    }

    public class InstancePollItem
    {
        public string InstanceHostName { get; set; }
        public string StorageAccountName { get; set; }
        public string StorageAccountKey { get; set; }
    }
}