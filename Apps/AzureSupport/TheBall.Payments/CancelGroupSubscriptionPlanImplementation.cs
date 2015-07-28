using System.Linq;
using System.Web;
using AzureSupport;
using Stripe;
using TheBall.Payments.INT;

namespace TheBall.Payments
{
    public class CancelGroupSubscriptionPlanImplementation
    {
        public static CustomerAccount GetTarget_CustomerAccount(string accountId)
        {
            var owner = InformationContext.CurrentOwner;
            CustomerAccount customerAccount = ObjectStorage.RetrieveFromOwnerContent<CustomerAccount>(owner, accountId);
            return customerAccount;
        }

        public static void ExecuteMethod_CancelSubscriptionPlan(string planName, CustomerAccount customerAccount)
        {
            StripeCustomerService stripeCustomerService = new StripeCustomerService();
            var stripeCustomer = stripeCustomerService.Get(customerAccount.StripeID);
            var stripeSubscriptions = stripeCustomer.StripeSubscriptionList.StripeSubscriptions;
            var planSubscriptions =
                stripeSubscriptions.Where(subscription => subscription.StripePlan.Id == planName).ToArray();
            StripeSubscriptionService subscriptionService = new StripeSubscriptionService();
            var customerID = stripeCustomer.Id;
            foreach (var subscription in planSubscriptions)
            {
                subscriptionService.Cancel(customerID, subscription.Id);
            }
            HttpContext.Current.Response.Write("{}");
        }

        public static SyncEffectivePlanAccessesToAccountParameters RevokeAccessFromCanceledPlan_GetParameters(string accountID)
        {
            return new SyncEffectivePlanAccessesToAccountParameters { AccountID = accountID };
        }

        public static CancelSubscriptionParams GetTarget_CancelParams()
        {
            return JSONSupport.GetObjectFromStream<CancelSubscriptionParams>(HttpContext.Current.Request.GetBufferedInputStream());
        }

        public static string GetTarget_PlanName(CancelSubscriptionParams cancelParams)
        {
            return cancelParams.PlanName;
        }

        public static string GetTarget_AccountID()
        {
            return InformationContext.CurrentAccount.AccountID;
        }
    }
}