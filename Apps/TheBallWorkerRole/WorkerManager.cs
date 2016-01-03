using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace TheBallWorkerRole
{
    internal class WorkerManager
    {
        private readonly string WorkerConsolePath;
        public WorkerManager(string fullPathToWorkerConsole)
        {
            WorkerConsolePath = fullPathToWorkerConsole;
        }

        private Process ClientProcess;
        AnonymousPipeServerStream PipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);

        internal async Task StartWorkerConsole()
        {
            var clientPipeHandler = PipeServer.GetClientHandleAsString();
            PipeServer.DisposeLocalCopyOfClientHandle();
            var startInfo = new ProcessStartInfo(WorkerConsolePath, clientPipeHandler)
            {
                UseShellExecute = false
            };
            ClientProcess = new Process {StartInfo = startInfo};
            ClientProcess.Start();
        }

        internal async Task ShutdownWorkerConsole()
        {
            using (StreamWriter writer = new StreamWriter(PipeServer))
            {
                writer.AutoFlush = true;
                await writer.WriteAsync("QUIT");
            }
            ClientProcess.WaitForExit();
            ClientProcess.Close();
            ClientProcess = null;

            PipeServer.Dispose();
            PipeServer = null;
        }
    }
}