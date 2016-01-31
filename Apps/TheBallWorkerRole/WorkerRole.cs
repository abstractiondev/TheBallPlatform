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

namespace TheBallWorkerRole
{
    internal static class TaskExt
    {
        public static Task AsAwaitable(this CancellationToken token)
        {
            var ev = new AsyncManualResetEvent();
            token.Register(() => ev.Set());
            return ev.WaitAsync();
        }
    }

    public class WorkerRole : RoleEntryPoint
    {
    }
}
