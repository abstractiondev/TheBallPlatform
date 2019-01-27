using System;
using System.Linq;
using System.Threading.Tasks;
using k8s;

namespace KubeTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(config);
            var podResult = await client.ListPodForAllNamespacesWithHttpMessagesAsync();
            var containers = podResult.Body.Items.SelectMany(item => item.Spec.Containers).ToArray();
            var containerNames = containers.Select(item => item.Image).OrderBy(item => item).ToArray();
        }
    }
}
