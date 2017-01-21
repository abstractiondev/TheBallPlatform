using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Nito.AsyncEx;
using TheBall.CORE.Storage;
using TheBall.Infra.TheBallWebConsole;

namespace TheBall.Infra.TheBallWebConsole
{
    class Program
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }


        static void Main(string[] args)
        {
            try
            {
                AsyncContext.Run(() => MainAsync(args));
            }
            catch (Exception exception)
            {
                var errorFile = Path.Combine(AssemblyDirectory, "ConsoleErrorLog.txt");
                File.WriteAllText(errorFile, exception.ToString());
                throw;
            }
        }

        static async void MainAsync(string[] args)
        {
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.DefaultConnectionLimit = 500;
            ServicePointManager.Expect100Continue = false;

            storageConceptTest();
            return;

            var workerConfigFullPath = args.Length > 0 ? args[0] : null;
            if (workerConfigFullPath == null)
                throw new ArgumentNullException(nameof(args), "Config full path cannot be null (first  argument)");
            
            var clientHandle = args.Length > 1 ? args[1] : null;

            var pipeStream = clientHandle != null
                ? new AnonymousPipeClientStream(PipeDirection.In, clientHandle)
                : null;
            var webManager = await WebManager.Create(pipeStream, workerConfigFullPath);
            await webManager.RunUpdateLoop();
        }

        private static void storageConceptTest()
        {
            
            StorageSupport.InitializeFileStorage("https://tbdevops.file.core.windows.net", "?sv=");
            var cloudFileClient = StorageSupport.CloudFileClient;
            var deployShare = cloudFileClient.ListShares("deploy").First();
            var rootDirectory = deployShare.GetRootDirectoryReference();
            var filesAndDirs =
                rootDirectory.ListFilesAndDirectories(new FileRequestOptions() {LocationMode = LocationMode.PrimaryOnly});
        }
    }
}
