using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TheBall.CORE.InstanceSupport
{
    public class RuntimeConfiguration
    {
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        internal static ConcurrentDictionary<string, RuntimeConfiguration>  RuntimeConfigurationsDict = new ConcurrentDictionary<string, RuntimeConfiguration>();

        public static InfraSharedConfig InfraConfig;
        internal static string ConfigRootPath;
        public readonly SecureConfig SecureConfig;
        public readonly InstanceConfig InstanceConfig;

        public RuntimeConfiguration(SecureConfig secureConfig, InstanceConfig instanceConfig)
        {
            SecureConfig = secureConfig;
            InstanceConfig = instanceConfig;
        }

        public static async Task InitializeOrUpdateInfraConfig(string infraConfigFullPath)
        {
            await _semaphore.WaitAsync();
            try
            {
                var infraFullDirectory = new DirectoryInfo(Path.GetDirectoryName(infraConfigFullPath));
                ConfigRootPath = infraFullDirectory.Parent.FullName;
                var infraConfig = await deserialize<InfraSharedConfig>(infraConfigFullPath);
                InfraConfig = infraConfig;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public static async Task InitializeOrUpdateInstanceConfig(string instanceName)
        {
            var secureConfigFullPath = Path.Combine(ConfigRootPath, instanceName, "SecureConfig.json");
            var secureConfig = await deserialize<SecureConfig>(secureConfigFullPath);
            var instanceConfigFullPath = Path.Combine(ConfigRootPath, instanceName, "InstanceConfig.json");
            var instanceConfig = await deserialize<InstanceConfig>(instanceConfigFullPath);

            var upToDateConfig = new RuntimeConfiguration(secureConfig, instanceConfig);
            RuntimeConfigurationsDict.AddOrUpdate(instanceName, upToDateConfig,
                (s, configuration) => upToDateConfig);
        }

        private static async Task<T> deserialize<T>(string fullPathName)
        {
            using (var stream = File.OpenText(fullPathName))
            {
                var textContent = await stream.ReadToEndAsync();
                T result = JsonConvert.DeserializeObject<T>(textContent);
                return result;
            }
        }

        public static InfraSharedConfig GetInfraSharedConfig()
        {
            return InfraConfig;
        }

        public static RuntimeConfiguration GetConfiguration(string instanceName)
        {
            RuntimeConfiguration result;
            RuntimeConfigurationsDict.TryGetValue(instanceName, out result);
            return result;
        }
    }
}