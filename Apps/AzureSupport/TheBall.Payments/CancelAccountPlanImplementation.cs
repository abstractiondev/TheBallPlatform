using System;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Stripe;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;
using TheBall.Payments.INT;

namespace TheBall.Payments
{
    public class CancelAccountPlanImplementation
    {
        public static string GetTarget_AccountID()
        {
            string accountID = InformationContext.CurrentAccount.AccountID;
            return accountID;
        }

        public static async Task<CustomerAccount> GetTarget_CustomerAccountAsync(string accountId)
        {
            var owner = InformationContext.CurrentOwner;
            var ownerID = owner.GetIDFromLocationPrefix();
            if (ownerID != InstanceConfig.Current.PaymentsGroupID)
                throw new SecurityException("Not supported payment owner ID: " + ownerID);
            string accountEmail = InformationContext.CurrentAccount.AccountEmail;
            if (String.IsNullOrEmpty(accountEmail))
                throw new SecurityException("Cannot get customer account without valid email");
            CustomerAccount customerAccount = await ObjectStorage.RetrieveFromOwnerContentA<CustomerAccount>(owner, accountId);
            if (customerAccount == null)
                throw new InvalidOperationException("Cannot cancel plan for account, where no CustomerAccount is not found");
            return customerAccount;
        }

        public static string GetTarget_StripeCustomerID(CustomerAccount customerAccount)
        {
            return customerAccount.StripeID;
        }

        public static async Task ExecuteMethod_RemoveCustomerPaymentSourceAsync(string stripeCustomerID, bool isTestMode)
        {
            StripeCustomerService customerService = new StripeCustomerService(isTestMode
                ? SecureConfig.Current.StripeTestSecretKey
                : SecureConfig.Current.StripeLiveSecretKey);
            var customer = await customerService.GetAsync(stripeCustomerID);
            if(customer.DefaultSourceId != null)
            {
                StripeCardService cardService = new StripeCardService(isTestMode
                    ? SecureConfig.Current.StripeTestSecretKey
                    : SecureConfig.Current.StripeLiveSecretKey);
                await cardService.DeleteAsync(stripeCustomerID, customer.DefaultSourceId);
            }
        }

        public static string GetTarget_PlanName(CancelSubscriptionParams cancelParameters)
        {
            return cancelParameters.PlanName;
        }

        public static async Task<StripeSubscription[]> GetTarget_CustomersActiveSubscriptionsAsync(string stripeCustomerID, bool isTestMode)
        {
            StripeSubscriptionService subscriptionService = new StripeSubscriptionService(isTestMode
                ? SecureConfig.Current.StripeTestSecretKey
                : SecureConfig.Current.StripeLiveSecretKey);
            var stripeList = await subscriptionService.ListAsync(stripeCustomerID);
            return stripeList.ToArray();
        }

        public static async Task ExecuteMethod_CancelSubscriptionAtPeriodEndAsync(string stripeCustomerId, bool isTestMode, string planName, StripeSubscription[] customersActiveSubscriptions)
        {
            var existingSubscription = customersActiveSubscriptions.FirstOrDefault(sub => sub.StripePlan.Id == planName);
            bool hasExistingToCancel = existingSubscription != null && !existingSubscription.CancelAtPeriodEnd;
            if(hasExistingToCancel)
            {
                if (existingSubscription.CancelAtPeriodEnd)
                {
                    var subService = new StripeSubscriptionService(isTestMode
                        ? SecureConfig.Current.StripeTestSecretKey
                        : SecureConfig.Current.StripeLiveSecretKey);
                    await
                        subService.CancelAsync(stripeCustomerId, existingSubscription.Id, true);
                }
            }
        }

        public static async Task ExecuteMethod_StoreObjectsAsync(CustomerAccount customerAccount)
        {
            await customerAccount.StoreInformationAsync();
        }

        public static bool GetTarget_IsTestMode(CustomerAccount customerAccount)
        {
            return customerAccount.IsTestAccount;
        }
    }
}