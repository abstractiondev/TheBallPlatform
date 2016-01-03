using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace TheBallWorkerConsole
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
            var clientHandle = args.Length > 0 ? args[0] : null;
            if(clientHandle == null)
                throw new ArgumentNullException(nameof(args), "Client handle cannot be null (first argument)");

            using (var pipeStream = new AnonymousPipeClientStream(PipeDirection.In, clientHandle))
            using(var reader = new StreamReader(pipeStream))
            {
                const int ConcurrentTasks = 3;
                Task[] activeTasks = new Task[ConcurrentTasks];
                int nextFreeIX = 0;



                while (true)
                {
                    Console.WriteLine("Waiting to process: " + DateTime.Now.ToString());
                    await Task.Delay(2000);
                    var pipeMessage = await reader.ReadToEndAsync();
                    if (!String.IsNullOrEmpty(pipeMessage))
                    {
                        Console.WriteLine("Quitting for message: " + pipeMessage);
                        await Task.Delay(10000);
                        break;
                    }
                    //await Task.WhenAny(activeTasks);
                }
            }
        }
    }
}
