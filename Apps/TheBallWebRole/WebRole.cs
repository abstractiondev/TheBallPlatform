using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.ApplicationServices;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.Infra.AzureRoleSupport;
using TheBall.Infra.WebServerManager;

namespace TheBallWebRole
{
    public class WebRole : AcceleratorRole
    {
        private static bool IsHybridWebRole { get; } = CloudConfigurationManager.GetSetting("IsHybridWebRole") ==
                                                bool.TrueString;

        protected override RoleAppInfo[] RoleApplications { get; } = IsHybridWebRole ?
            new []
            {
                GetWebConsoleAppInfo(),
                GetWorkerConsoleAppInfo()
            }
            : new[]
        {
            GetWebConsoleAppInfo(),
        };

        public override bool OnStart()
        {
            if (!RoleEnvironment.IsEmulated)
            {
                string hostsFileContents =
@"127.0.0.1 dev
127.0.0.1   test
127.0.0.1   beta
127.0.0.1   prod
127.0.0.1   websites
";
                var hostsFilePath = Path.Combine(Environment.SystemDirectory, "drivers", "etc", "hosts");
                File.WriteAllText(hostsFilePath, hostsFileContents);
            }
            return base.OnStart();
        }
    }
}
