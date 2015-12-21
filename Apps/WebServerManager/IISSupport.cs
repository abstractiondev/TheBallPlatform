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
            binding.SslFlags = SslFlags.CentralCertStore | SslFlags.Sni;
            iisManager.CommitChanges();
            return sites[hostAndSiteName];
        }

        public static void SetHostHeaders(string siteName, string[] hostHeaders)
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
