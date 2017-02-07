using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using Stripe;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;
using TheBall.Payments.INT;

namespace TheBall.Payments
{
    public class ProcessStripeWebhookImplementation
    {
        public static string GetTarget_EventID(StripeWebhookData jsonObject)
        {
            //return jsonObject.livemode ? jsonObject.id : null;
            return jsonObject.id;
        }

        public static StripeEvent GetTarget_EventData(string eventId, bool isTestMode)
        {
            try
            {
                if (eventId == null)
                    return null;
                StripeEventService service = new StripeEventService(StripeSupport.GetStripeApiKey(isTestMode));
                return service.Get(eventId);
            }
            catch
            {
                return null;
            }
        }

        public static async Task ExecuteMethod_ProcessStripeEventAsync(StripeEvent eventData, bool testMode)
        {
            if (eventData == null)
                return;
            try
            {

                string eventType = eventData.Type;
                if (eventType.StartsWith("customer.subscription."))
                {
                    dynamic subscription = eventData.Data.Object;
                    string customerId = subscription.customer;
                    bool isTestMode = !eventData.LiveMode;
                    var output =
                        GetAccountFromStripeCustomer.Execute(new GetAccountFromStripeCustomerParameters
                        {
                            StripeCustomerID = customerId,
                            IsTestAccount = isTestMode
                        });
                    var account = output.ResultAccount;
                    if (account != null)
                    {
                        string accountId = output.ResultAccount.ID;
                        var accountRoot = await ObjectStorage.RetrieveFromDefaultLocationA<TBRAccountRoot>(accountId);
                        var currAccount = accountRoot.Account;
                        string accountEmail = currAccount.Emails.CollectionContent.Select(tbEm => tbEm.EmailAddress).FirstOrDefault();
                        if (accountEmail == null)
                            accountEmail = "";
                        var accountName = accountEmail;
                        accountName = accountName.Trim();
                        InformationContext.Current.Account = new CoreAccountData(accountId,
                            accountName, accountEmail);
                        await SyncEffectivePlanAccessesToAccount.ExecuteAsync(new SyncEffectivePlanAccessesToAccountParameters
                        {
                            AccountID = accountId
                        });
                    }
                }
            }
            catch (Exception exception) // Ignore errors
            {
                ErrorSupport.ReportException(exception);
            }

        }

        public static bool GetTarget_IsTestMode(StripeWebhookData parametersJsonObject)
        {
            return !parametersJsonObject.livemode;
        }
    }
}