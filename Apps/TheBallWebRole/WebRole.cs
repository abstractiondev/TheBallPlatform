using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using TheBall.Infra.WebServerManager;

namespace TheBallWebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.


            SyncWebsitesFromStorage();
            
            return base.OnStart();
        }

        private void SyncWebsitesFromStorage()
        {
            string websiteFolder;
            string hostAndSiteName;

            websiteFolder = @"d:\temp\iistest1";
            hostAndSiteName = "testlocal.theball.me";
            try
            {
                IISSupport.CreateCCSWebSite(websiteFolder, hostAndSiteName);

            }
            catch (Exception)
            {
            }
        }
    }
}
