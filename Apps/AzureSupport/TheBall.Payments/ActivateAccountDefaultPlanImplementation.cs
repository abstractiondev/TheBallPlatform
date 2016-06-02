using System.Threading.Tasks;
using Stripe;
using TheBall.Payments.INT;

namespace TheBall.Payments
{
    public class ActivateAccountDefaultPlanImplementation
    {
        public static string GetTarget_AccountID()
        {
            throw new System.NotImplementedException();
        }

        public static Task<CustomerAccount> GetTarget_CustomerAccountAsync(string accountId)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_UpdateStripeCustomerData(PaymentToken paymentToken, CustomerAccount customerAccount)
        {
            throw new System.NotImplementedException();
        }

        public static string GetTarget_StripeCustomerID(CustomerAccount customerAccount)
        {
            throw new System.NotImplementedException();
        }

        public static string GetTarget_PlanName(PaymentToken paymentToken)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_ValidateStripePlanName(string planName)
        {
            throw new System.NotImplementedException();
        }

        public static Task<StripeSubscription[]> GetTarget_CustomersActiveSubscriptionsAsync(string stripeCustomerId)
        {
            throw new System.NotImplementedException();
        }

        public static string[] GetTarget_CustomersActivePlanNames(StripeSubscription[] customersActiveSubscriptions)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_SyncCurrentCustomerActivePlans(CustomerAccount customerAccount, string[] customersActivePlanNames)
        {
            throw new System.NotImplementedException();
        }

        public static Task ExecuteMethod_ProcessPaymentAsync(PaymentToken paymentToken, string stripeCustomerId, string planName, string[] customersActivePlanNames)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_AddPlanAsActiveToCustomer(CustomerAccount customerAccount, string planName)
        {
            throw new System.NotImplementedException();
        }

        public static Task ExecuteMethod_StoreObjectsAsync(CustomerAccount customerAccount)
        {
            throw new System.NotImplementedException();
        }

        public static SyncEffectivePlanAccessesToAccountParameters GrantAccessToPaidPlan_GetParameters(string accountId)
        {
            throw new System.NotImplementedException();
        }
    }
}