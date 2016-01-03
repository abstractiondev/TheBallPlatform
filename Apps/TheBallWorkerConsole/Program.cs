using System;
using System.Collections.Generic;
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
            const int ConcurrentTasks = 3;
            Task[] activeTasks = new Task[ConcurrentTasks];
            int nextFreeIX = 0;

            while (true)
            {
                await Task.WhenAny(activeTasks);
            }
        }
    }
}
