using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Management.Compute;
using Microsoft.WindowsAzure.Management.Models;
using Microsoft.WindowsAzure.Management.Storage;
using Microsoft.WindowsAzure.Management.Storage.Models;

namespace AzMTool
{
    public static class StorageAccountCreator
    {
        public async static Task<string> CreateStorageAccount(
  SubscriptionCloudCredentials credentials)
        {
            using (var storageClient = new StorageManagementClient(credentials))
            {
                string storageName = ConfigurationManager.AppSettings["storageName"];

                await storageClient.StorageAccounts.CreateAsync(
                  new StorageAccountCreateParameters
                  {
                      Label = "Sample Storage Account",
                      Location = LocationNames.EastUS,
                      Name = storageName,
                      AccountType = "Standard_LRS"
                  });
            }
            return "Successfully created account.";
        }

        public async static Task<string>
  DeleteStorageAccount(SubscriptionCloudCredentials credentials)
        {
            using (var computeClient = new ComputeManagementClient(credentials))
            {
                var diskCount = 1;
                while (diskCount > 0)
                {
                    var diskListResult =
                      await computeClient.VirtualMachineDisks.ListDisksAsync();
                    diskCount = diskListResult.Disks.Count;
                }
            }

            using (var storageClient = new StorageManagementClient(credentials))
            {
                await storageClient.StorageAccounts.DeleteAsync(
                  ConfigurationManager.AppSettings["storageName"]);
            }

            return "Successfully deleted account.";
        }
    }
}