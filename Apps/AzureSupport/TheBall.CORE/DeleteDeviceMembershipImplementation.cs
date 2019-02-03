using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class DeleteDeviceMembershipImplementation
    {
        public static async Task<DeviceMembership> GetTarget_DeviceMembershipAsync(IContainerOwner owner, string deviceMembershipId)
        {
            return await ObjectStorage.RetrieveFromDefaultLocationA<DeviceMembership>(deviceMembershipId, owner);
        }

        public static async Task ExecuteMethod_DeleteDeviceMembershipAsync(DeviceMembership deviceMembership)
        {
            await deviceMembership.DeleteInformationObjectAsync();
        }
    }
}