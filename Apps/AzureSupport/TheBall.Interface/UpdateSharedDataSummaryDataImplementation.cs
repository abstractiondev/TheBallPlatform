using System.Threading.Tasks;
using TheBall.CORE;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class UpdateSharedDataSummaryDataImplementation
    {
        public static bool GetTarget_IsCompleteUpdate(CollaborationPartner partner)
        {
            throw new System.NotImplementedException();
        }

        public static Task<IContainerOwner[]> GetTarget_CollaborationPartnersAsync(CollaborationPartner partner, bool isCompleteUpdate)
        {
            throw new System.NotImplementedException();
        }

        public static Task ExecuteMethod_UpdatePartnerSummariesAsync(IContainerOwner[] collaborationPartners)
        {
            throw new System.NotImplementedException();
        }

        public static Task ExecuteMethod_UpdateCompleteShareSummaryAsync(IContainerOwner[] collaborationPartners, bool isCompleteUpdate)
        {
            throw new System.NotImplementedException();
        }
    }
}