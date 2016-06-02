using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Stripe;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;
using TheBall.Payments.INT;

namespace TheBall.Payments
{
    public class ActivateAccountDefaultPlanImplementation
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
            {
                customerAccount = new CustomerAccount();
                customerAccount.ID = accountId;
                customerAccount.SetLocationAsOwnerContent(owner, customerAccount.ID);
                StripeCustomerService stripeCustomerService = new StripeCustomerService();
                var stripeCustomer = stripeCustomerService.Create(new StripeCustomerCreateOptions
                {
                    Email = accountEmail
                });
                customerAccount.StripeID = stripeCustomer.Id;
                await customerAccount.StoreInformationAsync();
            }
            return customerAccount;
        }

        public static async Task ExecuteMethod_UpdateStripeCustomerDataAsync(PaymentToken paymentToken, CustomerAccount customerAccount)
        {
            StripeCustomerService customerService = new StripeCustomerService();
            var customer = await customerService.GetAsync(customerAccount.StripeID);
            if (!customer.Metadata.ContainsKey("business_type"))
                customer.Metadata.Add("business_type", "B2C");
            var cardInfo = paymentToken.card;
            await customerService.UpdateAsync(customer.Id, new StripeCustomerUpdateOptions
            {
                Metadata = customer.Metadata,
                Description = paymentToken.card.name
            });
        }

        public static string GetTarget_StripeCustomerID(CustomerAccount customerAccount)
        {
            return customerAccount.StripeID;
        }

        public static string GetTarget_PlanName(PaymentToken paymentToken)
        {
            return paymentToken.currentproduct;
        }

        public static async Task ExecuteMethod_ValidateStripePlanNameAsync(string planName)
        {
            var planService = new StripePlanService();
            var stripePlan = await planService.GetAsync(planName);
            if (stripePlan == null)
                throw new InvalidDataException("Stripe plan not found: " + planName);
        }

        public static async Task<StripeSubscription[]> GetTarget_CustomersActiveSubscriptionsAsync(string stripeCustomerID)
        {
            StripeSubscriptionService subscriptionService = new StripeSubscriptionService();
            var stripeList = await subscriptionService.ListAsync(stripeCustomerID);
            return stripeList.ToArray();
        }

        public static string[] GetTarget_CustomersActivePlanNames(StripeSubscription[] customersActiveSubscriptions)
        {
            return customersActiveSubscriptions.Select(sub => sub.StripePlan.Id).ToArray();
        }

        public static void ExecuteMethod_SyncCurrentCustomerActivePlans(CustomerAccount customerAccount, string[] customersActivePlanNames)
        {
            customerAccount.ActivePlans = customersActivePlanNames.ToList();
        }

        public static async Task ExecuteMethod_ProcessPaymentAsync(PaymentToken paymentToken, string stripeCustomerId, string planName, string[] customersActivePlanNames)
        {
            bool customerHasPlanAlready = customersActivePlanNames.Contains(planName);
            if (!customerHasPlanAlready)
            {
                var customerID = stripeCustomerId;
                var subscriptionService = new StripeSubscriptionService();
                var cardInfo = paymentToken.card;
                var subscription = await subscriptionService.CreateAsync(customerID, planName, new StripeSubscriptionCreateOptions
                {
                    Card = new StripeCreditCardOptions
                    {
                        AddressCity = cardInfo.address_city,
                        AddressCountry = cardInfo.address_country,
                        AddressLine1 = cardInfo.address_line1,
                        Name = cardInfo.name,
                        AddressZip = cardInfo.address_zip,
                        Cvc = cardInfo.cvc_check,
                        ExpirationMonth = cardInfo.exp_month,
                        ExpirationYear = cardInfo.exp_year,
                        TokenId = paymentToken.id
                    },
                    /*
                    CardName = cardInfo.name,
                    CardAddressCity = cardInfo.address_city,
                    CardAddressCountry = cardInfo.address_country,
                    CardAddressLine1 = cardInfo.address_line1,
                    CardAddressZip = cardInfo.address_zip,
                    CardExpirationMonth = cardInfo.exp_month,
                    CardExpirationYear = cardInfo.exp_year,
                    TokenId = paymentToken.id,*/
                });
            }
        }

        public static void ExecuteMethod_AddPlanAsActiveToCustomer(CustomerAccount customerAccount, string planName)
        {
            if (!customerAccount.ActivePlans.Contains(planName))
                customerAccount.ActivePlans.Add(planName);
        }

        public static async Task ExecuteMethod_StoreObjectsAsync(CustomerAccount customerAccount)
        {
            await customerAccount.StoreInformationAsync();
        }

        public static SyncEffectivePlanAccessesToAccountParameters GrantAccessToPaidPlan_GetParameters(string accountID)
        {
            return new SyncEffectivePlanAccessesToAccountParameters { AccountID = accountID };
        }

        public static void ExecuteMethod_ValidateMatchingEmail(PaymentToken paymentToken)
        {
            throw new System.NotImplementedException();
        }

    }
}