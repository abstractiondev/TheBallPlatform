using System;

namespace TheBall.Platform.WorkerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            WorkerManager worker = new WorkerManager();
            string pipeHandleAsString = null;
            if (args.Length > 0)
                pipeHandleAsString = args[0];
            else
            {
                Console.WriteLine("Starting worker... press CTRL+C to gracefully shut down");
            }
            StopInvoker stopInvoker = new StopInvoker(worker, pipeHandleAsString);
            worker.OnStart();
            worker.RunUntilStopped();
            //worker.OnStop();
        }
    }
}
