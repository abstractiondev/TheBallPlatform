using System;
using System.Security;
using System.Threading.Tasks;
using Stripe;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;
using TheBall.Payments.INT;

namespace TheBall.Payments
{
    public class PurchaseProductImplementation
    {
        public static string GetTarget_AccountID()
        {
            return InformationContext.CurrentAccount.AccountID;
        }

        public static async Task<CustomerAccount> GetTarget_CustomerAccountAsync(string accountId)
        {
            var owner = InformationContext.CurrentOwner;
            var ownerID = owner.GetIDFromLocationPrefix();
            if (ownerID != InstanceConfig.Current.PaymentsGroupID)
                throw new SecurityException("Not supported payment owner ID: " + ownerID);
            CustomerAccount customerAccount = await ObjectStorage.RetrieveFromOwnerContentA<CustomerAccount>(owner, accountId);
            if(customerAccount == null)
                throw new InvalidOperationException("Customer does not exist for account: " + accountId);
            return customerAccount;
        }

        public static string GetTarget_StripeCustomerID(CustomerAccount customerAccount)
        {
            return customerAccount.StripeID;
        }

        public static string GetTarget_ProductName(PaymentToken paymentToken)
        {
            return paymentToken.currentproduct;
        }

        public static double GetTarget_ProductPrice(PaymentToken paymentToken)
        {
            return paymentToken.expectedprice;
        }

        public static async Task ExecuteMethod_ValidateStripeProductAndPriceAsync(string productName, double productPrice)
        {
            //Stripe  customerService = new StripeCustomerService(SecureConfig.Current.StripeSecretKey);

        }

        public static async Task ExecuteMethod_ProcessPaymentAsync(PaymentToken paymentToken, string stripeCustomerId, string productName)
        {
            string cardId = paymentToken.id;
            if (cardId == null)
            {
                var customerService = new StripeCustomerService(SecureConfig.Current.StripeSecretKey);
                var customer = await customerService.GetAsync(stripeCustomerId);
                cardId = customer.DefaultSourceId;
            }
            if(cardId == null)
                throw new InvalidOperationException("No default payment method set");
            var chargeService = new StripeChargeService(SecureConfig.Current.StripeSecretKey);
            var charge = await chargeService.CreateAsync(new StripeChargeCreateOptions
            {
                Amount = (int) (paymentToken.expectedprice * 100),
                CustomerId = stripeCustomerId,
                Description = paymentToken.currentproduct
            });
        }
    }
}