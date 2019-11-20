using System.Threading.Tasks;
using TheBall.Core.INT;

namespace TheBall.Core
{
    public class DeleteAuthenticatedAsActiveDeviceImplementation
    {
        public static async Task<AuthenticatedAsActiveDevice> GetTarget_AuthenticatedAsActiveDeviceAsync(IContainerOwner owner, string authenticatedAsActiveDeviceId)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<AuthenticatedAsActiveDevice>(owner, authenticatedAsActiveDeviceId);
        }

        public static async Task ExecuteMethod_DeleteAuthenticatedAsActiveDeviceAsync(AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            await authenticatedAsActiveDevice.DeleteInformationObjectAsync();
        }

        public static void ExecuteMethod_CallDeleteDeviceOnRemoteSide(AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            try
            {
                var result = DeviceSupport.ExecuteRemoteOperation<DeviceOperationData>(authenticatedAsActiveDevice.ID,
                                                                                       "TheBall.Core.RemoteDeviceCoreOperation", new DeviceOperationData {OperationRequestString = "DELETEREMOTEDEVICE"});
            }
            catch
            {
                
            }
        }
    }
}