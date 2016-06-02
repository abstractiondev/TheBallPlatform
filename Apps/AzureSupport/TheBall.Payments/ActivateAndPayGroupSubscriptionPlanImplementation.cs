using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Web;
using AzureSupport;
using Stripe;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;
using TheBall.Payments.INT;

namespace TheBall.Payments
{
    public class ActivateAndPayGroupSubscriptionPlanImplementation
    {
        public static PaymentToken GetTarget_PaymentToken()
        {
            return JSONSupport.GetObjectFromStream<PaymentToken>(HttpContext.Current.Request.GetBufferedInputStream());
        }

        public static void ExecuteMethod_ValidateMatchingEmail(PaymentToken paymentToken)
        {
            if (paymentToken.email.ToLower() != InformationContext.CurrentAccount.AccountEmail.ToLower())
                throw new SecurityException("Account email and payment email mismatch");
        }

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

        public static string GetTarget_PlanName(PaymentToken paymentToken)
        {
            return paymentToken.currentproduct;
        }


        public static ValidatePlanContainingGroupsParameters ValidatePlanGroups_GetParameters(string planName)
        {
            return new ValidatePlanContainingGroupsParameters { PlanName = planName };
        }

        public static void ExecuteMethod_ValidateStripePlanName(string planName)
        {
            var planService = new StripePlanService();
            var stripePlan = planService.Get(planName);
            if(stripePlan == null)
                throw new InvalidDataException("Stripe plan not found: " + planName);
        }

        public static StripeSubscription[] GetTarget_CustomersActiveSubscriptions(string stripeCustomerID)
        {
            StripeSubscriptionService subscriptionService = new StripeSubscriptionService();
            return subscriptionService.List(stripeCustomerID).ToArray();
        }

        /*
        public static void ExecuteMethod_ProcessPayment(CustomerAccount customerAccount)
        {
            var customerID = customerAccount.StripeID;
            var subscriptionService = new StripeSubscriptionService();
            var subscription = subscriptionService.Create(customerID, paymentToken.currentproduct, new StripeSubscriptionCreateOptions
            {
                TokenId = paymentToken.id
            });
            HttpContext.Current.Response.Write("{}");
        }
         * */

        public static string GetTarget_PaymentTokenID(PaymentToken paymentToken)
        {
            return paymentToken.id;
        }

        public static string GetTarget_StripeCustomerID(CustomerAccount customerAccount)
        {
            return customerAccount.StripeID;
        }

        public static void ExecuteMethod_AddPlanAsActiveToCustomer(CustomerAccount customerAccount, string planName)
        {
            if(!customerAccount.ActivePlans.Contains(planName))
                customerAccount.ActivePlans.Add(planName);
        }

        public static void ExecuteMethod_StoreObjects(CustomerAccount customerAccount)
        {
            customerAccount.StoreInformation();
        }

        public static string[] GetTarget_CustomersActivePlanNames(StripeSubscription[] customersActiveSubscriptions)
        {
            return customersActiveSubscriptions.Select(sub => sub.StripePlan.Id).ToArray();
        }

        public static void ExecuteMethod_SyncCurrentCustomerActivePlans(CustomerAccount customerAccount, string[] customersActivePlanNames)
        {
            customerAccount.ActivePlans = customersActivePlanNames.ToList();
        }

        public static void ExecuteMethod_ProcessPayment(string stripeCustomerId, string planName, string[] customersActivePlanNames, PaymentToken paymentToken)
        {
            bool customerHasPlanAlready = customersActivePlanNames.Contains(planName);
            if (!customerHasPlanAlready)
            {
                var customerID = stripeCustomerId;
                var subscriptionService = new StripeSubscriptionService();
                var cardInfo = paymentToken.card;
                var subscription = subscriptionService.Create(customerID, planName, new StripeSubscriptionCreateOptions
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
            HttpContext.Current.Response.Write("{}");
        }

        public static SyncEffectivePlanAccessesToAccountParameters GrantAccessToPaidPlan_GetParameters(string accountID)
        {
            return new SyncEffectivePlanAccessesToAccountParameters {AccountID = accountID};
        }

        public static void ExecuteMethod_UpdateStripeCustomerData(CustomerAccount customerAccount, PaymentToken paymentToken)
        {
            StripeCustomerService customerService = new StripeCustomerService();
            var customer = customerService.Get(customerAccount.StripeID);
            if(!customer.Metadata.ContainsKey("business_type"))
                customer.Metadata.Add("business_type", "B2C");
            var cardInfo = paymentToken.card;
            customerService.Update(customer.Id, new StripeCustomerUpdateOptions
            {
                Metadata = customer.Metadata,
                Description = paymentToken.card.name
            });
        }
    }
}