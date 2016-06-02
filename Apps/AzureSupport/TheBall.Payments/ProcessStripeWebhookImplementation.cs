using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using Stripe;
using TheBall.CORE;
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

        public static StripeEvent GetTarget_EventData(string eventID)
        {
            try
            {
                if (eventID == null)
                    return null;
                StripeEventService service = new StripeEventService();
                return service.Get(eventID);
            }
            catch
            {
                return null;
            }
        }

        public static async Task ExecuteMethod_ProcessStripeEventAsync(StripeEvent eventData)
        {
            if (eventData == null)
                return;
            try
            {

                string eventType = eventData.Type;
                if (eventType.StartsWith("customer.subscription."))
                {
                    dynamic subscription = eventData.Data.Object;
                    string customerID = subscription.customer;
                    var output =
                        GetAccountFromStripeCustomer.Execute(new GetAccountFromStripeCustomerParameters
                        {
                            StripeCustomerID = customerID
                        });
                    var account = output.ResultAccount;
                    if (account != null)
                    {
                        string accountID = output.ResultAccount.ID;
                        var accountRoot = await ObjectStorage.RetrieveFromDefaultLocationA<TBRAccountRoot>(accountID);
                        var currAccount = accountRoot.Account;
                        string accountName;
                        string accountEmail = currAccount.Emails.CollectionContent.Select(tbEm => tbEm.EmailAddress).FirstOrDefault();
                        if (accountEmail == null)
                            accountEmail = "";
                        accountName = accountEmail;
                        accountName = accountName.Trim();
                        InformationContext.Current.Account = new CoreAccountData(accountID,
                            accountName, accountEmail);
                        await SyncEffectivePlanAccessesToAccount.ExecuteAsync(new SyncEffectivePlanAccessesToAccountParameters
                        {
                            AccountID = accountID
                        });
                    }
                }
            }
            catch (Exception exception) // Ignore errors
            {
                ErrorSupport.ReportException(exception);
            }

        }
    }
}