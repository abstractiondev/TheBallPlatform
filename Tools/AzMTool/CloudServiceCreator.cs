using System.Configuration;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Compute;
using Microsoft.WindowsAzure.Management.Compute.Models;
using Microsoft.WindowsAzure.Management.Models;

namespace AzMTool
{
    public static class CloudServiceCreator
    {
        public static async Task<string> CreateCloudService(
            SubscriptionCloudCredentials credentials)
        {
            using (var computeClient = new ComputeManagementClient(credentials))
            {
                await computeClient.HostedServices.CreateAsync(
                    new HostedServiceCreateParameters
                    {
                        Label = "NewCloudService",
                        Location = LocationNames.EastUS,
                        ServiceName = ConfigurationManager.AppSettings["serviceName"]
                    });
            }
            return "Successfully created service.";
        }

        public static async Task<string> DeleteCloudService(
            this SubscriptionCloudCredentials credentials)
        {
            using (var computeClient = new ComputeManagementClient(credentials))
            {
                await computeClient.HostedServices.DeleteAsync(
                    ConfigurationManager.AppSettings["serviceName"]);
            }
            return "Successfully deleted service.";
        }
    }
}