using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;

namespace TheBall.Infra.WebServerManager
{
    public static class IISSupport
    {
        public static void CreateCCSWebSite(string websiteFolder, string hostAndSiteName)
        {
            ServerManager iisManager = new ServerManager();
            var sites = iisManager.Sites;
            var existingSite = sites[hostAndSiteName];
            if (existingSite != null)
                return;
            string bindingInformation = String.Format("*:443:{0}", hostAndSiteName);
            existingSite = sites.Add(hostAndSiteName, "https", bindingInformation, websiteFolder);
            var binding = existingSite.Bindings[0];
            binding.SslFlags = SslFlags.CentralCertStore | SslFlags.Sni;
            iisManager.CommitChanges();
        }

        public static void SetupIIS()
        {
            ServerManager iisManager = new ServerManager();
            var adminConfig = iisManager.GetAdministrationConfiguration();
        }
    }
}
