using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Management.Compute;
using Microsoft.WindowsAzure.Management.Compute.Models;

namespace AzMTool
{
    public static class VirtualMachineCreator
    {
        public static async Task<string> CreateVirtualMachine(
            SubscriptionCloudCredentials credentials)
        {
            using (var computeClient = new ComputeManagementClient(credentials))
            {
                var operatingSystemImageListResult =
                    await computeClient.VirtualMachineOSImages.ListAsync();
                var imageName =
                    operatingSystemImageListResult.Images.FirstOrDefault(
                        x => x.Label.Contains(
                            ConfigurationManager.AppSettings["imageFilter"])).Name;
                var windowsConfigSet = new ConfigurationSet
                {
                    ConfigurationSetType =
                        ConfigurationSetTypes.WindowsProvisioningConfiguration,
                    AdminPassword =
                        ConfigurationManager.AppSettings["adminPassword"],
                    AdminUserName = ConfigurationManager.AppSettings["adminName"],
                    ComputerName = ConfigurationManager.AppSettings["vmName"],
                    HostName = string.Format("{0}.cloudapp.net",
                        ConfigurationManager.AppSettings["serviceName"])
                };

                var networkConfigSet = new ConfigurationSet
                {
                    ConfigurationSetType = "NetworkConfiguration",
                    InputEndpoints = new List<InputEndpoint>
                    {
                        new InputEndpoint
                        {
                            Name = "PowerShell",
                            LocalPort = 5986,
                            Protocol = "tcp",
                            Port = 5986,
                        },
                        new InputEndpoint
                        {
                            Name = "Remote Desktop",
                            LocalPort = 3389,
                            Protocol = "tcp",
                            Port = 3389,
                        }
                    }
                };
                var vhd = new OSVirtualHardDisk
                {
                    SourceImageName = imageName,
                    HostCaching = VirtualHardDiskHostCaching.ReadWrite,
                    MediaLink = new Uri(string.Format(
                        "https://{0}.blob.core.windows.net/vhds/{1}.vhd",
                        ConfigurationManager.AppSettings["storageName"], imageName))
                };

                var deploymentAttributes = new Role
                {
                    RoleName = ConfigurationManager.AppSettings["vmName"],
                    RoleSize = VirtualMachineRoleSize.Small,
                    RoleType = VirtualMachineRoleType.PersistentVMRole.ToString(),
                    OSVirtualHardDisk = vhd,
                    ConfigurationSets = new List<ConfigurationSet>
                    {
                        windowsConfigSet,
                        networkConfigSet
                    },
                    ProvisionGuestAgent = true
                };

                var createDeploymentParameters =
                    new VirtualMachineCreateDeploymentParameters
                    {
                        Name = ConfigurationManager.AppSettings["deployName"],
                        Label = ConfigurationManager.AppSettings["deployName"],
                        DeploymentSlot = DeploymentSlot.Production,
                        Roles = new List<Role> {deploymentAttributes}
                    };
                var deploymentResult =
                    await computeClient.VirtualMachines.CreateDeploymentAsync(
                        ConfigurationManager.AppSettings["serviceName"],
                        createDeploymentParameters);
            }
            return "Successfully created Virtual Machine";
        }

        public async static Task<string> DeleteVirtualMachine(
  SubscriptionCloudCredentials credentials)
        {
            using (var computeClient = new ComputeManagementClient(credentials))
            {
                string vmStatus = "Created";
                while (!vmStatus.Equals("Running"))
                {
                    var vmStatusResponse =
                      await computeClient.Deployments.GetBySlotAsync(
                        ConfigurationManager.AppSettings["serviceName"],
                        DeploymentSlot.Production);
                    vmStatus = vmStatusResponse.Status.ToString();
                }

                var deleteDeploymentResult =
                  await computeClient.Deployments.DeleteByNameAsync(
                    ConfigurationManager.AppSettings["serviceName"],
                    ConfigurationManager.AppSettings["deployName"],
                    true);
            }
            return "Successfully deleted Virtual Machine.";
        }
    }
}