using TheBall.CORE;

namespace TheBall.Payments
{
    public class AssociatePaymentToGroupImplementation
    {
        public static IContainerOwner GetTarget_GroupAsOwner(string groupId)
        {
            return new VirtualOwner("grp", groupId);
        }
    }
}