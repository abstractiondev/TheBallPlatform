using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheBall.CORE.Storage;
using TheBall.Infra.AppUpdater;

namespace TheBall.Infra.TheBallWebConsole
{
    public class WebConsoleConfig
    {
        public int PollingIntervalSeconds { get; set; }

        public UpdateConfigItem[] PackageData { get; set; }

        public MaturityBindingItem[] InstanceBindings { get; set; }

        public string WwwSitesMaturityLevel { get; set; }
        public string[] WwwSiteHostNames { get; set; }

        public static async Task<WebConsoleConfig> GetConfig(string fullPathToConfig)
        {
            if(fullPathToConfig == null)
                throw new ArgumentNullException(nameof(fullPathToConfig));
            using (var fileStream = File.OpenRead(fullPathToConfig))
            {
                var data = new byte[fileStream.Length];
                await fileStream.ReadAsync(data, 0, data.Length);

                var config = JSONSupport.GetObjectFromData<WebConsoleConfig>(data);
                validateConfig(config);
                return config;
            }
        }

        private static void validateConfig(WebConsoleConfig config)
        {
            bool validatePackageData = config.PackageData?.GroupBy(item => item.MaturityLevel).All(grp => grp.Count() == 1) == true;
            if(!validatePackageData)
                throw new InvalidDataException("PackageData");
            bool validateBindings =
                config.InstanceBindings?.GroupBy(binding => binding.MaturityLevel).All(grp => grp.Count() == 1) == true;
            if(!validateBindings)
                throw new InvalidDataException("Bindings");
            bool validateWwwSitesMaturityLevel = String.IsNullOrEmpty(config.WwwSitesMaturityLevel) || 
                config.PackageData?.Any(package => package.MaturityLevel == config.WwwSitesMaturityLevel) == true;
            if(!validateWwwSitesMaturityLevel)
                throw new InvalidDataException("WwwSitesMaturityLevel");
        }
    }
}