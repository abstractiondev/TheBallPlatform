using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace TheBall.Infra.AzureRoleSupport
{
    public class AppManager
    {
        //[DllImport("kernel32.dll")]
        //static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        private readonly string _appConsolePath;
        public readonly string _appConfigPath;

        public AppManager(string fullPathToAppConsole, string fullPathToAppConfig)
        {
            _appConsolePath = fullPathToAppConsole;
            _appConfigPath = fullPathToAppConfig;
        }

        private Process ClientProcess;
        private AnonymousPipeServerStream PipeServer = null;

        public async Task StartAppConsole(bool isTestMode = false)
        {
            PipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
            var clientPipeHandler = PipeServer.GetClientHandleAsString();
            string appConfigPart = isTestMode ? "-test" : _appConfigPath;
            string args = $"{appConfigPart} {clientPipeHandler}";
            try
            {
                var startInfo = new ProcessStartInfo(_appConsolePath, args)
                {
                    UseShellExecute = false
                };
                ClientProcess = new Process {StartInfo = startInfo};
                ClientProcess.Start();
                var clientWnd = ClientProcess.MainWindowHandle;
                ShowWindow(clientWnd, SW_SHOW);

            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error starting app console from path: {_appConsolePath}", ex);                
            }
        }

        public async Task<int> ShutdownAppConsole(bool throwOnFail = false)
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
                var exitCode = ClientProcess.ExitCode;
                ClientProcess.Close();
                ClientProcess = null;

                PipeServer.Dispose();
                PipeServer = null;
                return exitCode;
            }
            catch (Exception ex)
            {
                if (throwOnFail)
                    throw;
                return -1;
            }

        }
    }
}