using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using Microsoft.Web.Deployment;

namespace TheBall.Infra.WebServerManager
{
    public static class IISSupport
    {
        public static void CreateIISApplicationSiteIfMissing(string appSiteName, string appSiteFolder)
        {
            if (appSiteFolder == null)
                throw new ArgumentNullException(nameof(appSiteFolder));
            ServerManager iisManager = new ServerManager();
            var sites = iisManager.Sites;
            var existingSite = sites[appSiteName];
            if (existingSite != null)
            {
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
            string bindingInformation = $"*:80:{appSiteName}";
            var newSite = sites.Add(appSiteName, "http", bindingInformation, appSiteFolder);
            newSite.ApplicationDefaults.ApplicationPoolName = appPoolName;
            iisManager.CommitChanges();
        }

        public static void SetInstanceCertBindings(string appSiteName, string[] instanceHostNames)
        {
            var iisManager = new ServerManager();
            var site = iisManager.Sites[appSiteName];
            bool anyChanges = false;
            foreach (var instanceHostName in instanceHostNames)
            {
                bool requiredChange = ensureInstanceCertBinding(site, instanceHostName);
                anyChanges = anyChanges || requiredChange;
            }
            if(anyChanges)
                iisManager.CommitChanges();
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
            string bindingInformation = String.Format("*:443:{0}", hostAndSiteName);
            var newSite = sites.Add(hostAndSiteName, "https", bindingInformation, websiteFolder);
            newSite.ApplicationDefaults.ApplicationPoolName = appPoolName;
            var binding = newSite.Bindings[0];
            //binding.SslFlags = SslFlags.CentralCertStore | SslFlags.Sni;
            binding.SslFlags = SslFlags.Sni;
            iisManager.CommitChanges();
            return sites[hostAndSiteName];
        }

        private static bool ensureInstanceCertBinding(Site site, string instanceHostName)
        {
            var instanceNameSplit = instanceHostName.Split('.');
            if (instanceNameSplit.Length < 3)
                return false;
            var secondLastIX = instanceNameSplit.Length - 2;
            var lastIX = instanceNameSplit.Length - 1;
            string domainName = instanceNameSplit[secondLastIX] + "." + instanceNameSplit[lastIX];
            string certCommonName = $"*.{domainName}";
            byte[] certificateHash = getCertHash(certCommonName);
            if (certificateHash == null)
                return false;
            var bindings = site.Bindings;
            var existingBinding = bindings.FirstOrDefault(binding => binding.Host == instanceHostName);
            if (existingBinding == null)
            {
                string bindingInformation = $"*:443:{instanceHostName}";
                existingBinding = bindings.Add(bindingInformation, "https");
            }
            if (certificateHash.SequenceEqual(existingBinding.CertificateHash) == false)
            {
                existingBinding.CertificateHash = certificateHash;
                existingBinding.CertificateStoreName = StoreName.My.ToString();
                return true;
            }
            return false;
        }

        private static byte[] getCertHash(string certCommonName)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            var matchingCerts = store.Certificates.Find(X509FindType.FindBySubjectName, certCommonName, true);
            var foundCert = matchingCerts.Count > 0 ? matchingCerts[0] : null;
            return foundCert?.GetCertHash();
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
            using (var depManager = DeploymentManager.CreateObject(DeploymentWellKnownProvider.DirPath, tempSitePath))
            {
                depManager.SyncTo(DeploymentWellKnownProvider.DirPath, appLiveFolder, new DeploymentBaseOptions(),
                    new DeploymentSyncOptions());
            }
        }

        [Obsolete("Separate site creation from its deployment", true)]
        public static void UpdateSiteWithDeploy(bool needsContentUpdating, string tempSitePath, string fullLivePath, string hostAndSiteName)
        {
            var site = CreateOrRetrieveCCSWebSite(fullLivePath, hostAndSiteName);
            if (needsContentUpdating)
            {
                using (var depObj = DeploymentManager.CreateObject(DeploymentWellKnownProvider.DirPath, tempSitePath))
                {
                    depObj.SyncTo(DeploymentWellKnownProvider.DirPath, fullLivePath, new DeploymentBaseOptions(),
                        new DeploymentSyncOptions());
                }
            }
        }

    }
}
