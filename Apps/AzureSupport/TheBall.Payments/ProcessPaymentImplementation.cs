using System;
using System.Security;
using System.Threading.Tasks;
using System.Web;
using AzureSupport;
using Microsoft.AspNetCore.Http;
using Stripe;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;
using TheBall.CORE.Storage;
using TheBall.Payments.INT;

namespace TheBall.Payments
{
    public class ProcessPaymentImplementation
    {
        public static PaymentToken GetTarget_PaymentToken()
        {
            HttpContext current = null;
            return JSONSupport.GetObjectFromStream<PaymentToken>(current.Request.Body);
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
                //TokenId = paymentToken.id
                Card = new StripeCreditCardOptions {  TokenId = paymentToken.id}
            });
            customerAccount.ActivePlans.Add(paymentToken.currentproduct);
            //HttpContext.Current.Response.Write("{}");
        }

        public static void ExecuteMethod_ValidateMatchingEmail(PaymentToken paymentToken)
        {
            if(paymentToken.email.ToLower() != InformationContext.CurrentAccount.AccountEmail.ToLower())
                throw new SecurityException("Account email and payment email mismatch");
        }

        public static async Task<CustomerAccount> GetTarget_CustomerAccountAsync()
        {
            string accountID = InformationContext.CurrentAccount.AccountID;
            string accountEmail = InformationContext.CurrentAccount.AccountEmail;
            if(String.IsNullOrEmpty(accountEmail))
                throw new SecurityException("Cannot get customer account without valid email");
            var owner = InformationContext.CurrentOwner;
            var ownerID = owner.GetIDFromLocationPrefix();
            if(ownerID != InstanceConfig.Current.PaymentsGroupID)
                throw new SecurityException("Not supported payment owner ID: " + ownerID);
            CustomerAccount customerAccount = await ObjectStorage.RetrieveFromOwnerContentA<CustomerAccount>(owner, accountID);
            if (customerAccount == null)
            {
                customerAccount = new CustomerAccount();
                customerAccount.ID = accountID;
                customerAccount.SetLocationAsOwnerContent(owner, customerAccount.ID);
                StripeCustomerService stripeCustomerService = new StripeCustomerService();
                var stripeCustomer = await stripeCustomerService.CreateAsync(new StripeCustomerCreateOptions
                {
                    Email = accountEmail
                });
                customerAccount.StripeID = stripeCustomer.Id;
                await customerAccount.StoreInformationAsync();
            }
            return customerAccount;
        }
    }
}