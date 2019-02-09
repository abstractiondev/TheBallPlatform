using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Nito.AsyncEx;

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
        public int? LatestExitCode { get; private set; }

        public event EventHandler OnConsoleExited
        {
            add { ClientProcess.Exited += value; }
            remove { ClientProcess.Exited -= value; }
        }

        private Task RunOnExitTask;

        public async Task StartAppConsole(bool isTestMode, bool autoUpdate, EventHandler onClientExitHandler = null, string additionalManagerArgs = null)
        {
            PipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
            var clientPipeHandler = PipeServer.GetClientHandleAsString();
            string appConfigPart = isTestMode ? "-test" : $"--ac \"{_appConfigPath}\"";
            string autoUpdatePart = autoUpdate ? " --au" : "";
            string additionalPart = !String.IsNullOrEmpty(additionalManagerArgs) ? " " + additionalManagerArgs : "";
            string args = $"{appConfigPart} --ch {clientPipeHandler}{autoUpdatePart}{additionalPart}";
            try
            {
                var startInfo = new ProcessStartInfo(_appConsolePath, args)
                {
                    UseShellExecute = false
                };
                ClientProcess = new Process {StartInfo = startInfo};
                LatestExitCode = null;
                RunOnExitTask = new Task(onExitCleanup);
                ClientProcess.EnableRaisingEvents = true;
                ClientProcess.Exited += ClientProcess_Exited;
                if(onClientExitHandler != null)
                    ClientProcess.Exited += onClientExitHandler;
                ClientProcess.Start();
                PipeServer.DisposeLocalCopyOfClientHandle();
                try
                {
                    var clientWnd = ClientProcess.MainWindowHandle;
                    ShowWindow(clientWnd, SW_SHOW);
                }
                catch
                {
                    
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error starting app console from path: {_appConsolePath}", ex);                
            }
        }

        private void onExitCleanup()
        {
            LatestExitCode = ClientProcess.ExitCode;
            ClientProcess.Close();
            ClientProcess = null;

            PipeServer.Dispose();
            PipeServer = null;
            RunOnExitTask = null;
        }

        private void ClientProcess_Exited(object sender, EventArgs e)
        {
            RunOnExitTask.Start();
        }

        public bool IsRunning => ClientProcess?.HasExited == false;

        public async Task ShutdownAppConsole(bool throwOnFail = false)
        {
            try
            {
                try
                {

                    using (StreamWriter writer = new StreamWriter(PipeServer))
                    {
                        writer.AutoFlush = true;
                        await writer.WriteAsync("QUIT");
                        PipeServer.WaitForPipeDrain();
                    }
                }
                catch
                {
                    
                }
                var exitTask = RunOnExitTask;
                if (exitTask != null)
                    await exitTask;
            }
            catch (Exception ex)
            {
                if (throwOnFail)
                    throw;
            }

        }
    }
}