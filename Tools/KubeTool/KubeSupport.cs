using System;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace KubeTool
{
    public class KubeSupport
    {
        [Obsolete("dont use", true)]
        public static async Task kubeTest()
        {
            var client = GetClient();
            var podResult = await client.ListPodForAllNamespacesWithHttpMessagesAsync();
            var containers = podResult.Body.Items.SelectMany(item => item.Spec.Containers).ToArray();
            var containerNames = containers.Select(item => item.Image).OrderBy(item => item).ToArray();
        }

        public static async Task<V1Deployment[]> GetDeployments(string namespaceName = null)
        {
            var client = GetClient();
            var allDeployments = await client.ListDeploymentForAllNamespacesWithHttpMessagesAsync();
            bool searchAllNamespaces = namespaceName == null;
            var deployments = allDeployments.Body.Items.Where(item => searchAllNamespaces || item.Metadata.NamespaceProperty == namespaceName)
                .ToArray();
            return deployments;
        }

        public static async Task<V1Deployment> GetDeployment(string deploymentName)
        {
            var allDeployments = await GetDeployments();
            var deployment = allDeployments.FirstOrDefault(item => item.Metadata.Name == deploymentName);
            return deployment;
        }

        public static Kubernetes GetClient()
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(config);
            return client;
        }
    }
}