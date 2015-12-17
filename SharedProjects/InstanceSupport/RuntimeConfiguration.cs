using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TheBall.CORE.InstanceSupport
{
    public class RuntimeConfiguration
    {
        internal static ConcurrentDictionary<string, RuntimeConfiguration>  RuntimeConfigurationsDict = new ConcurrentDictionary<string, RuntimeConfiguration>();

        public readonly InfraSharedConfig InfraConfig;
        public readonly SecureConfig SecureConfig;
        public readonly InstanceConfig InstanceConfig;

        public RuntimeConfiguration(InfraSharedConfig infraConfig, SecureConfig secureConfig, InstanceConfig instanceConfig)
        {
            InfraConfig = infraConfig;
            SecureConfig = secureConfig;
            InstanceConfig = instanceConfig;
        }

        public static async Task InitializeOrUpdateConfig(string instanceName, string infraConfigFullPath, string secureConfigFullPath,
            string instanceConfigFullPath)
        {
            var infraConfig = await deserialize<InfraSharedConfig>(infraConfigFullPath);
            var secureConfig = await deserialize<SecureConfig>(secureConfigFullPath);
            var instanceConfig = await deserialize<InstanceConfig>(instanceConfigFullPath);

            var upToDateConfig = new RuntimeConfiguration(infraConfig, secureConfig, instanceConfig);
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

        public static RuntimeConfiguration GetConfiguration(string instanceName)
        {
            RuntimeConfiguration result;
            RuntimeConfigurationsDict.TryGetValue(instanceName, out result);
            return result;
        }
    }
}