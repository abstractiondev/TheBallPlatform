using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBall.Platform.WorkerConsole
{
    public class StopInvoker
    {
        private AnonymousPipeClientStream ClientPipeStream = null;
        private StreamReader PipeReader = null;
        private Task AsyncPipeReader = null;
        byte[] AsyncDummy = new byte[1024];

        public StopInvoker(WorkerManager workerManager, string pipeHandleAsString = null)
        {
            if (pipeHandleAsString == null)
                Console.CancelKeyPress += (sender, args) =>
                {
                    if (args.SpecialKey == ConsoleSpecialKey.ControlC)
                    {
                        Console.WriteLine("Setting Stopped status to true...");
                        workerManager.IsStopped = true;
                        args.Cancel = true;
                    }
                };
            else
            {
                ClientPipeStream = new AnonymousPipeClientStream(PipeDirection.In, pipeHandleAsString);
                ClientPipeStream.BeginRead(AsyncDummy, 0, 1024, ar =>
                {
                    workerManager.IsStopped = true;
                }, null);
            }
        }
    
    }
}
