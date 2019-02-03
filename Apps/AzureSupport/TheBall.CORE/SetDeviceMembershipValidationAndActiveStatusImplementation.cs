using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class SetDeviceMembershipValidationAndActiveStatusImplementation
    {
        public static async Task<DeviceMembership> GetTarget_DeviceMembershipAsync(IContainerOwner owner, string deviceMembershipId)
        {
            return await ObjectStorage.RetrieveFromDefaultLocationA<DeviceMembership>(deviceMembershipId, owner);
        }

        public static void ExecuteMethod_SetDeviceValidAndActiveValue(bool isValidAndActive, DeviceMembership deviceMembership)
        {
            deviceMembership.IsValidatedAndActive = isValidAndActive;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(DeviceMembership deviceMembership)
        {
            await deviceMembership.StoreInformationAsync();
        }
    }
}