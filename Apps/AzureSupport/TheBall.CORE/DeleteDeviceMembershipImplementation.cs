namespace TheBall.CORE
{
    public class DeleteDeviceMembershipImplementation
    {
        public static DeviceMembership GetTarget_DeviceMembership(IContainerOwner owner, string deviceMembershipId)
        {
            return ObjectStorage.RetrieveFromDefaultLocation<DeviceMembership>(deviceMembershipId, owner);
        }

        public static void ExecuteMethod_DeleteDeviceMembership(DeviceMembership deviceMembership)
        {
            deviceMembership.DeleteInformationObject();
        }
    }
}