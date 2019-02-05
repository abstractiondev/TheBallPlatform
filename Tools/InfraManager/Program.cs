using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KubeTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await kubeTest();
            var updateResult = await KubeSupport.UpdatePlatformToLatest();
            var message = String.Join(Environment.NewLine, updateResult);
            Console.WriteLine(message);
        }
    }
}
