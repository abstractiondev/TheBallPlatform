using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TheBall.Infra.AzureRoleSupport
{
    public enum RoleAppType
    {
        WorkerConsole,
        WebConsole
    }

    public class RoleAppInfo
    {
        public string RoleSpecificManagerArgs;
        public string AppConfigPath;
        public string AppRootFolder;
        public string ComponentName;
        public RoleAppType AppType;

        public string TargetConsolePath => Path.Combine(AppRootFolder, ComponentName + ".exe");
    }


    public abstract class AcceleratorRole : RoleEntryPoint
    {
        //private const string PathTo7Zip = @"d:\bin\7z.exe";
        public AppRoleManager AppRoleManager { get; } = new AppRoleManager(CloudConfigurationManager.GetSetting("InfraToolsRootFolder"));

        protected abstract RoleAppInfo[] RoleApplications { get; }

        public override void Run()
        {
            AppRoleManager.Run();
        }


        public override bool OnStart()
        {
            var roleApplications = RoleApplications;
            AppRoleManager.OnStart(roleApplications);
            bool result = base.OnStart();
            Trace.TraceInformation("TheBallRole has been started");
            return result;
        }

        public override void OnStop()
        {
            AppRoleManager.OnStop();
            base.OnStop();
            Trace.TraceInformation("TheBallRole has stopped");
        }
    }
}
