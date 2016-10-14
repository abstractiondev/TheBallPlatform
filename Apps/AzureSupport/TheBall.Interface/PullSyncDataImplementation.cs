using TheBall.CORE;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class PullSyncDataImplementation
    {
        public static IContainerOwner GetTarget_CollaborationSource(CollaborationPartner partner)
        {
            return new VirtualOwner(partner.PartnerType, partner.PartnerID, true);
        }
    }
}