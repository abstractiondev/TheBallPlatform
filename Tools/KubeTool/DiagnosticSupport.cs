using System.Linq;
using System.Threading.Tasks;

namespace KubeTool
{
    public class DiagnosticSupport
    {
        public static async Task RunBaseDiagnostics()
        {
            return;
            string workerBaseName = @"abstractiondev/theballworker";
            var workerDeploymentName = "tbwrk-deployment";
            await UpdateDeployment(workerBaseName, workerDeploymentName);

            string webBaseName = @"abstractiondev/theballweb";
            var webDeploymentName = "tbweb-deployment";
            //await UpdateDeployment(webBaseName, webDeploymentName, "20180802.1631_dev_126cbdbf4dc44315dc1578d15ef7a7726a7e26c9");
            await UpdateDeployment(webBaseName, webDeploymentName);
        }

        private static async Task UpdateDeployment(string imageBaseName, string deploymentName, string specificTag = null)
        {
            var latestImageTag = await DockerHubSupport.GetLatestTag(imageBaseName);
            var updateTag = specificTag ?? latestImageTag;
            var deployment = await KubeSupport.GetDeployment(deploymentName);
            var containers = deployment?.Spec.Template.Spec.Containers;
            var imageName = $"{imageBaseName}:{updateTag}";
            var isRunningExpected = containers.Any(item => item.Image.StartsWith(imageName));
            if (!isRunningExpected)
            {
                await KubeSupport.UpdateDeployment(deploymentName, imageName);
            }
        }
    }
}