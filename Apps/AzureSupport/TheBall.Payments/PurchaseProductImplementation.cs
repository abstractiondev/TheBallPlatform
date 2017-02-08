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

        public static string GetTarget_ProductName(ProductPurchaseInfo paymentToken)
        {
            return paymentToken.currentproduct;
        }

        public static double GetTarget_ProductPrice(ProductPurchaseInfo paymentToken)
        {
            return paymentToken.expectedprice;
        }

        public static async Task ExecuteMethod_ValidateStripeProductAndPriceAsync(string productName, double productPrice, bool isTestMode)
        {
            var stripeProducts = await StripeSupport.GetProducts(isTestMode);
            var matchingProductSkus = stripeProducts.data.SelectMany(product => product.skus.data)
                .Where(skuItem => skuItem.id == productName).ToArray();
            if (matchingProductSkus.Length > 1)
                throw new InvalidDataException($"Multiple product SKUs found for product '{productName}'");
            var sku = matchingProductSkus.FirstOrDefault();
            if (sku == null)
                throw new InvalidDataException($"Not matching product SKU found for product '{productName}'");
            int stripeFormedPrice = (int) (productPrice * 100);
            if(sku.price != stripeFormedPrice)
                throw new InvalidDataException($"Product '{productName}' expected price '{productPrice}' not matching to Stripe price {stripeFormedPrice / 100.0}");
        }

        public static async Task ExecuteMethod_ProcessPaymentAsync(ProductPurchaseInfo purchaseInfo, string stripeCustomerId, bool isTestMode, string productName, double productPrice, string currency)
        {
            var customerService = new StripeCustomerService(StripeSupport.GetStripeApiKey(isTestMode));
            var customer = await customerService.GetAsync(stripeCustomerId);
            var cardId = customer.DefaultSourceId;
            if(cardId == null)
                throw new InvalidOperationException("No default payment method set");
            var chargeService = new StripeChargeService(StripeSupport.GetStripeApiKey(isTestMode));
            var charge = await chargeService.CreateAsync(new StripeChargeCreateOptions
            {
                Amount = (int) (productPrice * 100),
                Currency = StripeSupport.GetStripeCurrency(currency),
                CustomerId = stripeCustomerId,
                Description = $"{productName} with currency {currency}"
            });
        }

        public static bool GetTarget_IsTestMode(CustomerAccount customerAccount)
        {
            return customerAccount.IsTestAccount;
        }

        public static string GetTarget_Currency(ProductPurchaseInfo purchaseInfo)
        {
            return purchaseInfo.currency;
        }
    }
}