using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace TheBallWorkerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync(args));
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

                var awaitableQuitReader = reader.ReadToEndAsync();

                while (true)
                {
                    Console.WriteLine("Waiting to process: " + DateTime.Now.ToString());
                    await Task.WhenAny(Task.Delay(2000), awaitableQuitReader);
                    if (awaitableQuitReader.IsCompleted)
                    {
                        Console.WriteLine("Quitting...");
                        await Task.Delay(10000);
                        break;
                    }
                    //await Task.WhenAny(activeTasks);
                }
            }
        }
    }
}
