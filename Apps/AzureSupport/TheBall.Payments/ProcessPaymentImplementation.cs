using System;
using System.Security;
using System.Web;
using AzureSupport;
using Stripe;
using TheBall.Payments.INT;

namespace TheBall.Payments
{
    public class ProcessPaymentImplementation
    {
        public static PaymentToken GetTarget_PaymentToken()
        {
            return JSONSupport.GetObjectFromStream<PaymentToken>(HttpContext.Current.Request.GetBufferedInputStream());
        }

        public static void ExecuteMethod_ProcessPayment(PaymentToken paymentToken)
        {
            //var tokenService = new StripeTokenService();
            //var stripeToken = tokenService.Get(paymentToken.id);
            /*
            var customerID = InformationContext.CurrentAccount.AccountID;
            StripeCustomer customer = null;
            var customerService = new StripeCustomerService();
            try
            {
                customer = customerService.Get(customerID);
            }
            catch
            {
                customer = customerService.Create(new StripeCustomerCreateOptions
                {
                    
                });
            }
            */
            bool isAllInclusive = paymentToken.currentproduct == "ALLINCLUSIVE";
            int chargeAmount = isAllInclusive ? 7500 : 999;
            string description = isAllInclusive ? "All Inclusive Training by iZENZEi" : "Online Taekwondo by iZENZEi";
            var myCharge = new StripeChargeCreateOptions
            {
                Amount = chargeAmount,
                Currency = "eur",
                Description = description,
                TokenId = paymentToken.id,
                //CustomerId = customer != null ? customer.Id : null
            };

            var chargeService = new StripeChargeService();
            var charge = chargeService.Create(myCharge);
            if(string.IsNullOrEmpty(charge.FailureMessage) == false)
                HttpContext.Current.Response.Write(String.Format("{{ failure: \"{0}\"}}", charge.FailureMessage));
            else
                HttpContext.Current.Response.Write("{}");
            //var subscriptionService = new StripeSubscriptionService();
            //var subscription = subscriptionService.Create()
        }

        public static void ExecuteMethod_ValidateMatchingEmail(PaymentToken paymentToken)
        {
            if(paymentToken.email.ToLower() != InformationContext.CurrentAccount.AccountEmail.ToLower())
                throw new SecurityException("Account email and payment email mismatch");
        }
    }
}