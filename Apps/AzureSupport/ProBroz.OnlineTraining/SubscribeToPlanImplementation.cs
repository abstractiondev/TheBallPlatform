using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheBall;

namespace ProBroz.OnlineTraining
{
    public class SubscribeToPlanImplementation
    {
        public static async Task<Member> GetTarget_MemberAsync(string memberID)
        {
            var member =
                await ObjectStorage.RetrieveFromOwnerContentA<Member>(memberID);
            return member;
        }

        public static async Task<MembershipPlan> GetTarget_PlanAsync(string planID)
        {
            var plan = await ObjectStorage.RetrieveFromOwnerContentA<MembershipPlan>(planID);
            return plan;
        }

        public static async Task<PaymentOption> GetTarget_PaymentOptionAsync(string paymentOptionID, MembershipPlan plan)
        {
            var paymentOption = await ObjectStorage.RetrieveFromOwnerContentA<PaymentOption>(paymentOptionID);
            if (plan.PaymentOptions.All(item => item != paymentOptionID))
                throw new InvalidDataException($"Invalid payment ID for plan: {paymentOptionID}");
            return paymentOption;
        }
    }
}