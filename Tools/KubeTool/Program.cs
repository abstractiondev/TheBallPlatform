using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KubeTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //await kubeTest();
            await KubeSupport.UpdatePlatformToLatest();
        }
    }
}
