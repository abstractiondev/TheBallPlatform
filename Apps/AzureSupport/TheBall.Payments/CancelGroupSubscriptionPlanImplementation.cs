using System.Linq;
using Stripe;

namespace TheBall.Payments
{
    public class CancelGroupSubscriptionPlanImplementation
    {
        public static CustomerAccount GetTarget_CustomerAccount(string accountId)
        {
            var owner = InformationContext.CurrentOwner;
            CustomerAccount customerAccount = CustomerAccount.RetrieveFromOwnerContent(owner, accountId);
            return customerAccount;
        }

        public static void ExecuteMethod_CancelSubscriptionPlan(string planName, CustomerAccount customerAccount)
        {
            StripeCustomerService stripeCustomerService = new StripeCustomerService();
            var stripeCustomer = stripeCustomerService.Get(customerAccount.StripeID);
            var stripeSubscriptions = stripeCustomer.StripeSubscriptionList.StripeSubscriptions;
            var planSubscriptions =
                stripeSubscriptions.Where(subscription => subscription.StripePlan.Name == planName).ToArray();
            StripeSubscriptionService subscriptionService = new StripeSubscriptionService();
            var customerID = stripeCustomer.Id;
            foreach (var subscription in planSubscriptions)
            {
                subscriptionService.Cancel(customerID, subscription.Id);
            }
        }

        public static SyncEffectivePlanAccessesToAccountParameters RevokeAccessFromCanceledPlan_GetParameters(string accountID)
        {
            return new SyncEffectivePlanAccessesToAccountParameters { AccountID = accountID };
        }
    }
}