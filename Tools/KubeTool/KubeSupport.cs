using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Rest.Serialization;

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

        public static async Task UpdateDeployment(string deploymentName, string imageName)
        {
            var client = GetClient();
            var deployment = await GetDeployment(deploymentName);
            var currentImageName = deployment.Spec.Template.Spec.Containers.First().Image;
            var currentImageBaseName = currentImageName.Split(':').First();
            var updateImageBaseName = imageName.Split(':').First();
            if(currentImageBaseName != updateImageBaseName)
                throw new InvalidOperationException($"Not allowed to update image {currentImageName} with incompatible base {imageName}");
            var namespaceName = deployment.Metadata.NamespaceProperty;
            //deployment.Spec.Template.Spec.Containers.FirstOrDefault()
            IJsonPatchDocument jsonPatch = new JsonPatchDocument(new List<Operation>()
            {
                new Operation("replace", "/spec/template/spec/containers/0/image", null, imageName)
            }, new ReadOnlyJsonContractResolver());
            V1Patch patchBody = new V1Patch(jsonPatch);
            await client.PatchNamespacedDeploymentWithHttpMessagesAsync(patchBody, deploymentName, namespaceName);
        }
    }
}