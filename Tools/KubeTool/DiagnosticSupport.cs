using System.Linq;
using System.Threading.Tasks;

namespace KubeTool
{
    public class DiagnosticSupport
    {
        public static async Task RunBaseDiagnostics()
        {
            string workerImageName = @"abstractiondev/theballworker";
            var latestWorkerTag = await DockerHubSupport.GetLatestTag(workerImageName);
            var workerDeployment = await KubeSupport.GetDeployment("tbwrk-deployment");
            var containers = workerDeployment?.Spec.Template.Spec.Containers;
            var isRunningLatest = containers.Any(item => item.Image.StartsWith($"{workerImageName}:{latestWorkerTag}"));
        }
    }
}