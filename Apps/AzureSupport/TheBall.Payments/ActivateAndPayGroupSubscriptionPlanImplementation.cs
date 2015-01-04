using System;
using System.Security;
using System.Web;
using AzureSupport;
using Stripe;
using TheBall.CORE;
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

        public static CustomerAccount GetTarget_CustomerAccount(string accountId)
        {
            var owner = InformationContext.CurrentOwner;
            var ownerID = owner.GetIDFromLocationPrefix();
            if (ownerID != InstanceConfiguration.PaymentsGroupID)
                throw new SecurityException("Not supported payment owner ID: " + ownerID);
            string accountEmail = InformationContext.CurrentAccount.AccountEmail;
            if (String.IsNullOrEmpty(accountEmail))
                throw new SecurityException("Cannot get customer account without valid email");
            CustomerAccount customerAccount = CustomerAccount.RetrieveFromOwnerContent(owner, accountId);
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
                customerAccount.StoreInformation();
            }
            return customerAccount;
        }

        public static string GetTarget_PlanName(PaymentToken paymentToken)
        {
            return paymentToken.currentproduct;
        }

        public static void ExecuteMethod_ProcessPayment(PaymentToken paymentToken, CustomerAccount customerAccount)
        {
            var customerID = customerAccount.StripeID;
            var subscriptionService = new StripeSubscriptionService();
            var subscription = subscriptionService.Create(customerID, paymentToken.currentproduct, new StripeSubscriptionCreateOptions
            {
                TokenId = paymentToken.id
            });
            HttpContext.Current.Response.Write("{}");
        }

        public static ValidatePlanContainingGroupsParameters ValidatePlan_GetParameters(string planName)
        {
            return new ValidatePlanContainingGroupsParameters {PlanName = planName};
        }

        public static GrantPlanAccessToAccountParameters GrantAccessToPaidPlan_GetParameters(string planName, string accountId)
        {
            return new GrantPlanAccessToAccountParameters {AccountID = accountId, PlanName = planName};
        }
    }
}