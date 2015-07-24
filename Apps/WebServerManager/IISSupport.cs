using System;
using System.Collections.Generic;
using System.Linq;
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
                //iisManager.CommitChanges();
            }
            sites = iisManager.Sites;
            string bindingInformation = String.Format("*:443:{0}", hostAndSiteName);
            existingSite = sites.Add(hostAndSiteName, "https", bindingInformation, websiteFolder);
            existingSite.ApplicationDefaults.ApplicationPoolName = appPoolName;
            var binding = existingSite.Bindings[0];
            binding.SslFlags = SslFlags.CentralCertStore | SslFlags.Sni;
            iisManager.CommitChanges();
            return sites[hostAndSiteName];
        }

        public static void UpdateSite(string fullLivePath, string hostAndSiteName, Action action)
        {
            var site = CreateOrRetrieveCCSWebSite(fullLivePath, hostAndSiteName);
            //site.Stop();
            //while(site.State != ObjectState.Stopped)
            //    Thread.Sleep(200);
            action();
            //site.Start();
            //while(site.State != ObjectState.Started)
            //    Thread.Sleep(200);
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
