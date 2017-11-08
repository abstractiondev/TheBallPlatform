using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TheBall.CORE.InstanceSupport
{
    public class RuntimeConfiguration
    {
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        internal static ConcurrentDictionary<string, RuntimeConfiguration>  RuntimeConfigurationsDict = new ConcurrentDictionary<string, RuntimeConfiguration>();

        public static InfraSharedConfig InfraConfig { get; private set; }
        internal static string ConfigRootPath;
        public readonly SecureConfig SecureConfig;
        public readonly InstanceConfig InstanceConfig;
        public readonly string FixedEnvironmentName;

        private RuntimeConfiguration(SecureConfig secureConfig, InstanceConfig instanceConfig,
            string fixedEnvironmentName = null)
        {
            SecureConfig = secureConfig;
            InstanceConfig = instanceConfig;
            FixedEnvironmentName = fixedEnvironmentName;
        }

        public static async Task InitializeRuntimeConfigs(string infraConfigFullPath)
        {
            await UpdateInfraConfig(infraConfigFullPath);
            foreach (var instanceName in InfraConfig.InstanceNames)
                await UpdateInstanceConfig(instanceName);
        }

        public static async Task UpdateInfraConfig(string infraConfigFullPath)
        {
            if(!File.Exists(infraConfigFullPath))
                throw new ArgumentException("InfraConfig does not exist: " + infraConfigFullPath, nameof(infraConfigFullPath));
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

        public static async Task UpdateInstanceConfig(string instanceName)
        {
            var secureConfigFullPath = Path.Combine(ConfigRootPath, instanceName, "SecureConfig.json");
            var secureConfig = await deserialize<SecureConfig>(secureConfigFullPath);
            var instanceConfigFullPath = Path.Combine(ConfigRootPath, instanceName, "InstanceConfig.json");
            var instanceConfig = await deserialize<InstanceConfig>(instanceConfigFullPath);
            instanceConfig.InstanceName = instanceName;
            var upToDateConfig = new RuntimeConfiguration(secureConfig, instanceConfig);
            RuntimeConfigurationsDict.AddOrUpdate(instanceName, upToDateConfig,
                (s, configuration) => upToDateConfig);
        }

        private static async Task<T> deserialize<T>(string fullPathName)
        {
            using (var stream = File.OpenText(fullPathName))
            {
                var textContent = await stream.ReadToEndAsync();
                var deserializeSettings = new JsonSerializerSettings()
                {
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                };
                T result = JsonConvert.DeserializeObject<T>(textContent, deserializeSettings);
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

        public static void InitializeForCustomTool(InfraSharedConfig infraSharedConfig, SecureConfig secureConfig, InstanceConfig instanceConfig, 
            string instanceName, string environmentName)
        {
            if (InfraConfig != null)
                throw new InvalidOperationException("InfraConfig already initialized");
            InfraConfig = infraSharedConfig;
            RuntimeConfiguration config = new RuntimeConfiguration(secureConfig, instanceConfig, environmentName);
            RuntimeConfigurationsDict.AddOrUpdate(instanceName, config, (s, configuration) => config);
        }
    }
}