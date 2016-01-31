using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace TheBall.Infra.AzureRoleSupport
{
    internal class WorkerManager
    {
        private readonly string WorkerConsolePath;
        public WorkerManager(string fullPathToWorkerConsole)
        {
            WorkerConsolePath = fullPathToWorkerConsole;
        }

        private Process ClientProcess;
        private AnonymousPipeServerStream PipeServer = null;

        internal async Task StartWorkerConsole()
        {
            PipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
            var clientPipeHandler = PipeServer.GetClientHandleAsString();
            string args = $@"X:\Configs\WorkerConsole.json {clientPipeHandler}";
            var startInfo = new ProcessStartInfo(WorkerConsolePath, args)
            {
                UseShellExecute = false
            };
            ClientProcess = new Process {StartInfo = startInfo};
            ClientProcess.Start();
        }

        internal async Task ShutdownWorkerConsole()
        {
            PipeServer.DisposeLocalCopyOfClientHandle();
            try
            {
                using (StreamWriter writer = new StreamWriter(PipeServer))
                {
                    writer.AutoFlush = true;
                    await writer.WriteAsync("QUIT");
                    PipeServer.WaitForPipeDrain();
                }
                ClientProcess.WaitForExit();
                ClientProcess.Close();
                ClientProcess = null;

                PipeServer.Dispose();
                PipeServer = null;

            }
            catch (Exception)
            {
                
            }

        }
    }
}