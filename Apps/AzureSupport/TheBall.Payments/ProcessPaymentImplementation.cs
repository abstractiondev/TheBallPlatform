using System;
using System.Security;
using System.Web;
using AzureSupport;
using Stripe;
using TheBall.CORE;
using TheBall.Payments.INT;

namespace TheBall.Payments
{
    public class ProcessPaymentImplementation
    {
        public static PaymentToken GetTarget_PaymentToken()
        {
            return JSONSupport.GetObjectFromStream<PaymentToken>(HttpContext.Current.Request.GetBufferedInputStream());
        }

        public static void ExecuteMethod_ProcessPayment(PaymentToken paymentToken, CustomerAccount customerAccount)
        {
            //var tokenService = new StripeTokenService();
            //var stripeToken = tokenService.Get(paymentToken.id);
            var customerID = customerAccount.StripeID;
            //var customerService = new StripeCustomerService();
            //var customer = customerService.Get(customerID);

            /*
            var myCharge = new StripeChargeCreateOptions
            {
                Amount = chargeAmount,
                Currency = "eur",
                Description = description,
                TokenId = paymentToken.id,
                CustomerId = customerAccount.StripeID
            };

            var chargeService = new StripeChargeService();
            var charge = chargeService.Create(myCharge);
            if(string.IsNullOrEmpty(charge.FailureMessage) == false)
                HttpContext.Current.Response.Write(String.Format("{{ failure: \"{0}\"}}", charge.FailureMessage));
            else
                HttpContext.Current.Response.Write("{}");
             * */
            var subscriptionService = new StripeSubscriptionService();
            var subscription = subscriptionService.Create(customerID, paymentToken.currentproduct, new StripeSubscriptionCreateOptions
            {
                TokenId = paymentToken.id
            });
            customerAccount.ActivePlans.Add(paymentToken.currentproduct);
            HttpContext.Current.Response.Write("{}");
        }

        public static void ExecuteMethod_ValidateMatchingEmail(PaymentToken paymentToken)
        {
            if(paymentToken.email.ToLower() != InformationContext.CurrentAccount.AccountEmail.ToLower())
                throw new SecurityException("Account email and payment email mismatch");
        }

        public static CustomerAccount GetTarget_CustomerAccount()
        {
            string accountID = InformationContext.CurrentAccount.AccountID;
            string accountEmail = InformationContext.CurrentAccount.AccountEmail;
            if(String.IsNullOrEmpty(accountEmail))
                throw new SecurityException("Cannot get customer account without valid email");
            var owner = InformationContext.CurrentOwner;
            var ownerID = owner.GetIDFromLocationPrefix();
            if(ownerID != InstanceConfiguration.PaymentsGroupID)
                throw new SecurityException("Not supported payment owner ID: " + ownerID);
            CustomerAccount customerAccount = CustomerAccount.RetrieveFromOwnerContent(owner, accountID);
            if (customerAccount == null)
            {
                customerAccount = new CustomerAccount();
                customerAccount.ID = accountID;
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
    }
}