using Stripe;
using TheBall.Payments.INT;

namespace TheBall.Payments
{
    public class ProcessStripeWebhookImplementation
    {
        public static string GetTarget_EventID(StripeWebhookData jsonObject)
        {
            return jsonObject.livemode ? jsonObject.id : null;
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

        public static void ExecuteMethod_ProcessStripeEvent(StripeEvent eventData)
        {
            if (eventData == null)
                return;
            string eventType = eventData.Type;
            if (eventType.StartsWith("customer.subscription."))
            {
                dynamic subscription = eventData.Data.Object;
                string customerID = subscription.customer;
                //TheBall.Payments.
            }
            
        }
    }
}