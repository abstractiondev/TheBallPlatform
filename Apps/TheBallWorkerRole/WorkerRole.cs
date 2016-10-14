using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Nito.AsyncEx;
using TheBall.Infra.AzureRoleSupport;

namespace TheBallWorkerRole
{
    public class WorkerRole : AcceleratorRole
    {
        const string TheBallWorkerConsoleName = "TheBallWorkerConsole.exe";
        protected override string AppPackageContainerName => "tb-instanceworkers";
        protected override string AppRootFolder => RoleEnvironment.GetLocalResource("WorkerFolder").RootPath;
        protected override AppTypeInfo[] ValidAppTypes => new[]
        {
            new AppTypeInfo("Dev", "DevConsole.zip", Path.Combine(AppRootFolder, "Dev", TheBallWorkerConsoleName), Path.Combine(AppRootFolder, "Dev.config")),
            new AppTypeInfo("Test", "TestConsole.zip", Path.Combine(AppRootFolder, "Test", TheBallWorkerConsoleName), Path.Combine(AppRootFolder, "Test.config")),
            new AppTypeInfo("Prod", "ProdConsole.zip", Path.Combine(AppRootFolder, "Prod", TheBallWorkerConsoleName), Path.Combine(AppRootFolder, "Prod.config")),
        };

    }
}
