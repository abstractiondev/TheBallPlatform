using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace TheBall.Infra.AzureRoleSupport
{
    public class AppManager
    {
        private readonly string _appConsolePath;
        public readonly string _appConfigPath;

        public AppManager(string fullPathToAppConsole, string fullPathToAppConfig)
        {
            _appConsolePath = fullPathToAppConsole;
            _appConfigPath = fullPathToAppConfig;
        }

        private Process ClientProcess;
        private AnonymousPipeServerStream PipeServer = null;

        internal async Task StartAppConsole()
        {
            PipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
            var clientPipeHandler = PipeServer.GetClientHandleAsString();
            string args = $@"{_appConfigPath} {clientPipeHandler}";
            var startInfo = new ProcessStartInfo(_appConsolePath, args)
            {
                UseShellExecute = false
            };
            ClientProcess = new Process {StartInfo = startInfo};
            ClientProcess.Start();
        }

        internal async Task ShutdownAppConsole()
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