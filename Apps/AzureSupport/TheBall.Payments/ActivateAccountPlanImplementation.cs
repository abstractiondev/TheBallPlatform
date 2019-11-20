using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Stripe;
using TheBall.Core;
using TheBall.Core.InstanceSupport;
using TheBall.Payments.INT;

namespace TheBall.Payments
{
    public class ActivateAccountPlanImplementation
    {
        public static string GetTarget_AccountID()
        {
            string accountID = InformationContext.CurrentAccount.AccountID;
            return accountID;
        }

        public static async Task<CustomerAccount> GetTarget_CustomerAccountAsync(string accountId, bool isTestMode)
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
                customerAccount = new CustomerAccount {ID = accountId};
                customerAccount.SetLocationAsOwnerContent(owner, customerAccount.ID);
                customerAccount.IsTestAccount = isTestMode;
                StripeCustomerService stripeCustomerService = new StripeCustomerService(StripeSupport.GetStripeApiKey(isTestMode));
                var stripeCustomer = stripeCustomerService.Create(new StripeCustomerCreateOptions
                {
                    Email = accountEmail
                });
                customerAccount.StripeID = stripeCustomer.Id;
                await customerAccount.StoreInformationAsync();
            }
            return customerAccount;
        }

        public static async Task ExecuteMethod_UpdateStripeCustomerDataAsync(PaymentToken paymentToken, CustomerAccount customerAccount, bool isTestMode)
        {
            StripeCustomerService customerService = new StripeCustomerService(StripeSupport.GetStripeApiKey(isTestMode));
            var customer = await customerService.GetAsync(customerAccount.StripeID);
            var metadata = customer.Metadata ?? new Dictionary<string, string>();
            if (!metadata.ContainsKey("business_type"))
                metadata.Add("business_type", "B2C");
            var cardInfo = paymentToken.card;
            var tokenId = paymentToken.id;
            await customerService.UpdateAsync(customer.Id, new StripeCustomerUpdateOptions
            {
                Metadata = metadata,
                Description = paymentToken.card.name,
                SourceToken = tokenId,
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

        public static async Task ExecuteMethod_ValidateStripePlanNameAsync(string planName, bool isTestMode)
        {
            var planService = new StripePlanService(StripeSupport.GetStripeApiKey(isTestMode));
            var stripePlan = await planService.GetAsync(planName);
            if (stripePlan == null)
                throw new InvalidDataException("Stripe plan not found: " + planName);
        }

        public static async Task<StripeSubscription[]> GetTarget_CustomersActiveSubscriptionsAsync(string stripeCustomerID, bool isTestMode)
        {
            StripeSubscriptionService subscriptionService = new StripeSubscriptionService(StripeSupport.GetStripeApiKey(isTestMode));
            var stripeList = await subscriptionService.ListAsync(stripeCustomerID);
            return stripeList.ToArray();
        }

        public static async Task ExecuteMethod_ProcessPaymentAsync(PaymentToken paymentToken, string stripeCustomerId, bool isTestMode, string planName, StripeSubscription[] customersActiveSubscriptions)
        {
            var existingSubscription = customersActiveSubscriptions.FirstOrDefault(sub => sub.StripePlan.Id == planName);
            bool noExistingSubscription = existingSubscription == null || existingSubscription.Status == "canceled";
            var couponId = paymentToken.couponId;
            var hasNewCoupon = couponId != null;
            if (noExistingSubscription)
            {
                var customerID = stripeCustomerId;
                var subscriptionService = new StripeSubscriptionService(StripeSupport.GetStripeApiKey(isTestMode));
                var cardInfo = paymentToken.card;
                var subscription = await subscriptionService.CreateAsync(customerID, planName, new StripeSubscriptionCreateOptions()
                {
                    CouponId = couponId,
                });
            }
            else
            {
                bool isCancelAtPeriodOrDifferentCoupon = existingSubscription.CancelAtPeriodEnd ||
                                                         existingSubscription?.StripeDiscount?.StripeCoupon?.Id !=
                                                         couponId;
                var hasExistingCoupon = existingSubscription?.StripeDiscount?.StripeCoupon != null;
                if (isCancelAtPeriodOrDifferentCoupon)
                {
                    var subService = new StripeSubscriptionService(StripeSupport.GetStripeApiKey(isTestMode));
                    await
                        subService.UpdateAsync(existingSubscription.Id,
                            new StripeSubscriptionUpdateOptions
                            {
                                PlanId = existingSubscription.StripePlan.Id,
                                CouponId = couponId,
                            });
                    if (hasExistingCoupon && !hasNewCoupon)
                    {
                        var discountService = new StripeDiscountService(StripeSupport.GetStripeApiKey(isTestMode));
                        await discountService.DeleteSubscriptionDiscountAsync(existingSubscription.Id);
                    }
                }
            }
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
            //if (paymentToken.email.ToLower() != InformationContext.CurrentAccount.AccountEmail.ToLower())
            //    throw new SecurityException("Account email and payment email mismatch");
        }

        public static async Task<bool> GetTarget_IsTestAccountAsync(string accountId)
        {
            var account = await ObjectStorage.RetrieveFromSystemOwner<Account>(accountId);
            var anyEmailInTestList = account.Emails.Any(emailId =>
            {
                var emailAddress = Email.GetEmailAddressFromID(emailId);
                return InstanceConfig.Current.PaymentTestEmails.Contains(emailAddress);
            });
            bool isTestAccount = false;
            var paymentTestMetaAttrName = InstanceConfig.Current.PaymentTestClientMetaAttribute;
            if (!String.IsNullOrEmpty(paymentTestMetaAttrName))
            {
                var metadataExpando = account.GetClientMetadataObject();
                if (metadataExpando != null)
                {
                    var metaDict = (IDictionary<string, object>)metadataExpando;
                    object resultObj;
                    if(metaDict.TryGetValue(paymentTestMetaAttrName, out resultObj))
                    {
                        isTestAccount = (bool) resultObj;
                    }
                }

            }
            return anyEmailInTestList || isTestAccount;
        }

        public static bool GetTarget_IsTokenTestMode(PaymentToken paymentToken)
        {
            return paymentToken.isTestMode;
        }

        public static bool GetTarget_IsTestMode(bool isTokenTestMode, bool isTestAccount)
        {
            bool valuesMatch = isTokenTestMode == isTestAccount;
            if (!valuesMatch)
            {
                var tokenMode = isTokenTestMode ? "test" : "live";
                var accountMode = isTestAccount ? "test" : "live";
                throw new InvalidOperationException($"Payment token mode ({tokenMode}) does not match account mode ({accountMode})");
            }
            return isTokenTestMode;
        }
    }
}