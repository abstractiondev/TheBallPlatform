using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using Nito.AsyncEx;
using TheBall.CORE;

namespace TheBallDevWorker
{

    class Program
    {
        private static int ExitCode = 0;

        const string ComponentName = "TheBallDevWorker";

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



        static int Main(string[] args)
        {
            try
            {
                //Debugger.Launch();
                string applicationConfigFullPath = null;
                string monitorFolder = null;
                var optionSet = new OptionSet()
                {
                    {
                        "ac|applicationConfig=", "Application config full path",
                        ac => applicationConfigFullPath = ac
                    },
                    {
                        "m|monitorFolder=", "Monitor folder full path",
                        m => monitorFolder = m
                    }

                };
                var options = optionSet.Parse(args);
                bool hasExtraOptions = options.Count > 0;
                bool isMissingMandatory = applicationConfigFullPath == null && monitorFolder == null;
                if (hasExtraOptions || isMissingMandatory)
                {
                    Console.WriteLine($"Usage: {ComponentName}.exe");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    optionSet.WriteOptionDescriptions(Console.Out);
                    return -1;
                }
                AsyncContext.Run(() => MainAsync(applicationConfigFullPath:applicationConfigFullPath, monitorFolder:monitorFolder));
            }
            catch (Exception exception)
            {
                var errorFile = Path.Combine(AssemblyDirectory, "ConsoleErrorLog.txt");
                File.WriteAllText(errorFile, exception.ToString());
                Console.WriteLine("Top Exception Handler: ");
                Console.WriteLine(exception.ToString());
                return -2;
            }
            return ExitCode;
        }

        static async Task MainAsync(string applicationConfigFullPath, string monitorFolder)
        {
            while (true)
            {
                await WhenFileAvailable(monitorFolder);
                var files = Directory.GetFiles(monitorFolder)
                    .Where(file => file.EndsWith(".data"))
                    .OrderBy(file => file)
                    .ToArray();
                try
                {
                    foreach (var file in files)
                    {
                        Console.WriteLine("Found: " + Path.GetFileName(file));
                        HttpOperationData operationData;
                        using (var fileStream = File.OpenRead(file))
                        {
                            operationData = fileStream.DeserializeProtobuf<HttpOperationData>();
                        }
                        Console.WriteLine($"Operation: {operationData.OperationName}");
                        var baseName = Path.GetFileNameWithoutExtension(file);
                        var folder = Path.GetDirectoryName(file);
                        File.Delete(file);
                        var jsonFile = Path.Combine(folder, baseName + ".json");
                        if (File.Exists(jsonFile))
                            File.Delete(jsonFile);
                    }
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    await Task.Delay(100);
                }
            }
        }


        public static Task WhenFileAvailable(string folderPath)
        {
            if(!Directory.Exists(folderPath))
                throw new ArgumentException($"Folderpath missing: {folderPath}", nameof(folderPath));

            if (Directory.GetFiles(folderPath).Length > 0)
                return Task.FromResult(true);

            var tcs = new TaskCompletionSource<bool>();
            FileSystemWatcher watcher = new FileSystemWatcher(folderPath);
            FileSystemEventHandler changeHandler = null;
            changeHandler = (s, e) =>
            {
                tcs.TrySetResult(true);
                watcher.Created -= changeHandler;
                watcher.Dispose();
            };

            watcher.Created += changeHandler;
            watcher.EnableRaisingEvents = true;

            if (Directory.GetFiles(folderPath).Length > 0)
                return Task.FromResult(true);

            return tcs.Task;
        }

    }
}
