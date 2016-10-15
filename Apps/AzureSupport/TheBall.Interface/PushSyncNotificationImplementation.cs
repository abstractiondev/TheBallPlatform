using System.Threading.Tasks;
using AzureSupport;
using TheBall.CORE;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class PushSyncNotificationImplementation
    {
        public static IContainerOwner GetTarget_CollaborationTarget(CollaborationPartner partner)
        {
            return new VirtualOwner(partner.PartnerType, partner.PartnerID, true);
        }

        public static HttpOperationData GetTarget_SyncOperationData(IContainerOwner collaborationTarget)
        {
            var currentOwner = InformationContext.CurrentOwner;
            var operationData = OperationSupport.GetOperationDataFromParameters(collaborationTarget,
                "TheBall.Interface.PullSyncData",
                new INT.CollaborationPartner
                {
                    PartnerType = currentOwner.ContainerName,
                    PartnerID = currentOwner.LocationPrefix
                });
            return operationData;
        }

        public static async Task ExecuteMethod_QueueSyncOperationToTargetAsync(IContainerOwner collaborationTarget, HttpOperationData syncOperationData)
        {
            await InformationContext.ExecuteAsOwnerAsync(collaborationTarget, async () =>
            {
                await OperationSupport.QueueHttpOperationAsync(syncOperationData);
            });
        }
    }
}