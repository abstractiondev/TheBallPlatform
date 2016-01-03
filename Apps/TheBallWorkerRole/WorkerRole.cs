using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
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
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("TheBallWorkerRole is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("TheBallWorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("TheBallWorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("TheBallWorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // Start worker role console
            await StartWorkerConsole();
            Trace.TraceInformation("Working");
            await cancellationToken.AsAwaitable();

            // Clean up worker role console
            await ShutdownWorkerConsole();
        }

        private async Task StartWorkerConsole()
        {
            throw new NotImplementedException();
        }
        private async Task ShutdownWorkerConsole()
        {
            throw new NotImplementedException();
        }

    }
}
