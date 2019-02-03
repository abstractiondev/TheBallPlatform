using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Web.Administration;

namespace TheBall.Infra.WebServerManager
{
    public enum Protocol
    {
        Undefined = 0,
        Http,
        Https
    }

    public class BindingSetting
    {
        public string AppName { get; set; }
        public string HostName { get; set; }
        public Protocol Protocol { get; set; }
    }

    internal class OptionFixerItem<T>
    {
        public Func<T, bool> NeedFix;
        public Action<T> FixAction;
    }

    public static class IISSupport
    {
        private static OptionFixerItem<ApplicationPool>[] AppPoolFixers = new OptionFixerItem<ApplicationPool>[]
        {
            new OptionFixerItem<ApplicationPool>()
            {
                NeedFix = appPool => appPool.AutoStart == false,
                FixAction = appPool => appPool.AutoStart = true,
            },
            new OptionFixerItem<ApplicationPool>()
            {
                NeedFix = appPool => appPool.StartMode != StartMode.AlwaysRunning,
                FixAction = appPool => appPool.StartMode = StartMode.AlwaysRunning,
            },
            new OptionFixerItem<ApplicationPool>()
            {
                NeedFix = appPool => appPool.ProcessModel.IdleTimeout != TimeSpan.Zero,
                FixAction = appPool => appPool.ProcessModel.IdleTimeout = TimeSpan.Zero,
            },

        };

        public static void CreateIISApplicationSiteIfMissing(string appSiteName, string appSiteFolder)
        {
            if (appSiteFolder == null)
                throw new ArgumentNullException(nameof(appSiteFolder));
            ServerManager iisManager = new ServerManager();
            var sites = iisManager.Sites;
            var existingSite = sites[appSiteName];
            if (existingSite != null)
            {
                if (existingSite.State == ObjectState.Stopped)
                    existingSite.Start();
                return;
            }
            var appPools = iisManager.ApplicationPools;
            string appPoolName = appSiteName;
            var appPool = appPools[appPoolName];
            if (appPool == null)
            {
                appPool = appPools.Add(appPoolName);
                appPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                appPool.ManagedRuntimeVersion = "v4.0";
                appPool.ProcessModel.IdentityType = ProcessModelIdentityType.NetworkService;
                appPool.AutoStart = true;
                appPool.StartMode = StartMode.AlwaysRunning;
                //iisManager.CommitChanges();
            }
            sites = iisManager.Sites;
            string bindingInformation = $"127.0.0.1:80:{appSiteName}";
            var newSite = sites.Add(appSiteName, "http", bindingInformation, appSiteFolder);
            newSite.ApplicationDefaults.ApplicationPoolName = appPoolName;
            iisManager.CommitChanges();
        }

        public static void SetAppBindings(string[] appNames, BindingSetting[] bindingSettings)
        {
            var appBindingsDict = appNames.ToDictionary(appName => appName,
                appName => bindingSettings.Where(item => item.AppName == appName).ToArray());
            bool anyChanges = false;
            using (var iisManager = new ServerManager())
            {
                foreach (var appName in appNames)
                {
                    var site = iisManager.Sites[appName];
                    bool requiredChanging = SetSiteBindings(site, appBindingsDict[appName]);
                    anyChanges |= requiredChanging;
                }
                if (anyChanges)
                    iisManager.CommitChanges();
            }
            var httpsAppGroups = bindingSettings
                .Where(setting => setting.Protocol == Protocol.Https)
                .GroupBy(setting => setting.AppName);
            foreach (var appGroup in httpsAppGroups)
            {
                var instanceHostNames = appGroup.Select(item => item.HostName).ToArray();
                var appSiteName = appGroup.Key;
                SetInstanceCertBindings(appSiteName, instanceHostNames);
            }
        }



        private static bool SetSiteBindings(Site site, BindingSetting[] newBindings)
        {
            var currBindings = site.Bindings.Cast<Binding>().ToArray();
            var bindingsToRemove = currBindings.Where(binding =>
            {
                bool isLocal = binding.Host.Contains(".") == false;
                bool hasMatchingSetting =
                    newBindings.Any(newBinding => 
                    newBinding.HostName == binding.Host && 
                    newBinding.Protocol.ToString().ToLower() == binding.Protocol.ToLower());
                return !hasMatchingSetting && !isLocal;
            }).ToArray();
            var bindingsToAdd = newBindings.Where(binding =>
            {
                bool hasMatchingSetting =
                    currBindings.Any(currBinding => 
                    currBinding.Host == binding.HostName &&
                    currBinding.Protocol.ToLower() == binding.Protocol.ToString().ToLower());
                return !hasMatchingSetting;
            }).ToArray();
            bool anyChanges = bindingsToRemove.Any() || bindingsToAdd.Any();

            // Remove old obsolete bindings
            Array.ForEach(bindingsToRemove, binding => site.Bindings.Remove(binding));

            var siteBindings = site.Bindings;

            Array.ForEach(bindingsToAdd, binding =>
            {
                var port = binding.Protocol == Protocol.Https ? 443 : 80;
                var hostName = binding.HostName;
                string bindingInformation = $"*:{port}:{hostName}";
                string protocol = binding.Protocol.ToString().ToLower();
                if (binding.Protocol == Protocol.Http)
                {
                    var newBinding = siteBindings.Add(bindingInformation, protocol);
                }
                else
                {
                    var domainName = getDomainName(hostName);
                    string certDomainName = $"*.{domainName}";
                    byte[] certificateHash = GetCertHash(certDomainName, true);
                    var newBinding = siteBindings.Add(bindingInformation, certificateHash, StoreName.My.ToString(),
                        SslFlags.Sni);
                }
            });
            return anyChanges;
        }

        public static void SetInstanceCertBindings(string appSiteName, string[] instanceHostNames)
        {
            var iisManager = new ServerManager();
            var site = iisManager.Sites[appSiteName];
            if (site == null)
                return;
            bool anyChanges = false;
            foreach (var instanceHostName in instanceHostNames)
            {
                bool requiredChange = ensureInstanceCertBinding(site, instanceHostName, false);
                anyChanges = anyChanges || requiredChange;
            }
            if(anyChanges)
                iisManager.CommitChanges();
        }

        [Obsolete("Old Format")]
        public static void UpdateInstanceBindings(string bindingData, params string[] siteNames)
        {
            bindingData = new StringReader(bindingData).ReadLine();
            var bindingComponents = bindingData.Split(';');
            foreach (var siteName in siteNames.Select(item => item.Trim()))
            {
                string sitePrefix = siteName + ":";
                var siteBindings = bindingComponents.FirstOrDefault(item => item.StartsWith(sitePrefix))?.Replace(sitePrefix, "").Split(',');
                if(siteBindings == null)
                    throw new InvalidDataException("Bindings not found for site: " + siteName);
                SetInstanceCertBindings(siteName, siteBindings);
            }
        }


        [Obsolete("Separate site & its bindings", true)]
        public static Site CreateOrRetrieveCCSWebSite(string websiteFolder, string hostAndSiteName)
        {
            if (websiteFolder == null)
                throw new ArgumentNullException(nameof(websiteFolder));
            ServerManager iisManager = new ServerManager();
            var sites = iisManager.Sites;
            var existingSite = sites[hostAndSiteName];
            if (existingSite != null)
                return existingSite;
            var appPools = iisManager.ApplicationPools;
            string appPoolName = hostAndSiteName;
            var appPool = appPools[appPoolName];
            if (appPool == null)
            {
                appPool = appPools.Add(appPoolName);
                appPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                appPool.ManagedRuntimeVersion = "v4.0";
                appPool.ProcessModel.IdentityType = ProcessModelIdentityType.NetworkService;
                appPool.AutoStart = true;
                appPool.StartMode = StartMode.AlwaysRunning;
                //iisManager.CommitChanges();
            }
            sites = iisManager.Sites;
            string bindingInformation = $"*:443:{hostAndSiteName}";
            var newSite = sites.Add(hostAndSiteName, "https", bindingInformation, websiteFolder);
            newSite.ApplicationDefaults.ApplicationPoolName = appPoolName;
            var binding = newSite.Bindings[0];
            //binding.SslFlags = SslFlags.CentralCertStore | SslFlags.Sni;
            binding.SslFlags = SslFlags.Sni;
            iisManager.CommitChanges();
            return sites[hostAndSiteName];
        }

        private static bool ensureInstanceCertBinding(Site site, string instanceHostName, bool throwOnCertMissingError = true)
        {
            if(site == null)
                throw new ArgumentNullException(nameof(site));
            var domainName = getDomainName(instanceHostName);
            string certDomainName = $"*.{domainName}";
            byte[] certificateHash = GetCertHash(certDomainName);
            if (certificateHash == null && throwOnCertMissingError)
                throw new InvalidDataException("Certificate not found for domain name: " + certDomainName);
            var bindings = site.Bindings;
            if(bindings == null)
                throw new InvalidDataException($"Bindings are null for site: {site.Name}");
            var existingBinding = bindings.FirstOrDefault(binding => binding.Host == instanceHostName);
            bool isChanged = false;
            if (existingBinding == null)
            {
                string bindingInformation = $"*:443:{instanceHostName}";
                existingBinding = bindings.Add(bindingInformation, "https");
                existingBinding.SslFlags = SslFlags.Sni;
                isChanged = true;
            }
            if (certificateHash != null && (existingBinding.CertificateHash == null || (certificateHash.SequenceEqual(existingBinding.CertificateHash) == false)))
            {
                existingBinding.Delete();
                //existingBinding.CertificateHash = certificateHash;
                //existingBinding.CertificateStoreName = StoreName.My.ToString();
                isChanged = true;
            }
            return isChanged;
        }

        private static string getDomainName(string instanceHostName)
        {
            var instanceNameSplit = instanceHostName.Split('.');
            if (instanceNameSplit.Length < 3)
                throw new InvalidDataException("Invalid instance host name for binding: " + instanceHostName);
            var secondLastIX = instanceNameSplit.Length - 2;
            var lastIX = instanceNameSplit.Length - 1;
            string domainName = instanceNameSplit[secondLastIX] + "." + instanceNameSplit[lastIX];
            return domainName;
        }

        public static byte[] GetCertHash(string certDomainName, bool fallbackToFirstIfNoMatch = false)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                var allCerts = store.Certificates.Cast<X509Certificate2>().ToArray();
                var foundCert = allCerts
                    .Where(cert => cert?.SubjectName?.Name?.Contains(certDomainName) == true)
                    .Where(cert => cert.Verify())
                    .OrderByDescending(cert => cert.NotAfter)
                    .FirstOrDefault();

                if (fallbackToFirstIfNoMatch && foundCert == null)
                    foundCert = allCerts.FirstOrDefault();

                return foundCert?.GetCertHash();
            }
            finally
            {
                store.Close();
            }
        }

        public static void EnsureHttpHostHeaders(string siteName, string[] hostHeaders)
        {
            UpdateExistingSite(siteName, site =>
            {
                var bindings = site.Bindings;
                foreach (var hostHeader in hostHeaders)
                {
                    bool existingBinding = bindings.Any(binding => String.Compare(binding.Host, hostHeader, StringComparison.OrdinalIgnoreCase) == 0);
                    if (!existingBinding)
                    {
                        string bindingInformation = string.Format("*:80:{0}", hostHeader);
                        string bindingProtocol = "http";
                        var binding = site.Bindings.Add(bindingInformation, bindingProtocol);
                    }
                }
            });
        }


        public static void UpdateExistingSite(string siteName, Action<Site> action)
        {
            ServerManager iisManager = new ServerManager();
            var sites = iisManager.Sites;
            var site = sites[siteName];
            action(site);
            iisManager.CommitChanges();
        }

        public static void DeployAppSiteContent(string tempSitePath, string appLiveFolder)
        {
            throw new NotImplementedException();
            /*
            using (var depManager = DeploymentManager.CreateObject(DeploymentWellKnownProvider.DirPath, tempSitePath))
            {
                depManager.SyncTo(DeploymentWellKnownProvider.DirPath, appLiveFolder, new DeploymentBaseOptions(),
                    new DeploymentSyncOptions());
            }
            */
        }

        public static void DeployAppPackageContent(string appPackageZip, string appLiveFolder, string appName)
        {
            throw new NotImplementedException();
            /*
            using (var depObject = DeploymentManager.CreateObject(DeploymentWellKnownProvider.Package, appPackageZip))
            {
                depObject.SyncParameters.Single(item => item.Name == "IIS Web Application Name").Value = appName;
                depObject.SyncTo(DeploymentWellKnownProvider.Auto, appLiveFolder, new DeploymentBaseOptions(),
                    new DeploymentSyncOptions());
            }*/
        }

        public static void SetImmediateFirstResponseOptions(string appName)
        {
            using (var iisManager = new ServerManager())
            {
                var appPool = iisManager.ApplicationPools[appName];
                bool anyChanges = false;
                anyChanges |= fixAppPool(appPool);
                if(anyChanges)
                    iisManager.CommitChanges();
            }
        }


        static bool fixAppPool(ApplicationPool appPool)
        {
            var fixesRequired = AppPoolFixers.Where(fixer => fixer.NeedFix(appPool)).ToArray();
            Array.ForEach(fixesRequired, fixer => fixer.FixAction(appPool));
            return fixesRequired.Length > 0;
        }
    }
}
