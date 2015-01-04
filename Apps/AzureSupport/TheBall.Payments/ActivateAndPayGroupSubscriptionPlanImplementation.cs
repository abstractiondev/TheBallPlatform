using TheBall.Payments.INT;

namespace TheBall.Payments
{
    public class ActivateAndPayGroupSubscriptionPlanImplementation
    {
        public static PaymentToken GetTarget_PaymentToken()
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_ValidateMatchingEmail(PaymentToken paymentToken)
        {
            throw new System.NotImplementedException();
        }

        public static string GetTarget_AccountID()
        {
            throw new System.NotImplementedException();
        }

        public static CustomerAccount GetTarget_CustomerAccount(string accountId)
        {
            throw new System.NotImplementedException();
        }

        public static string GetTarget_PlanName(PaymentToken paymentToken)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_ProcessPayment(PaymentToken paymentToken, CustomerAccount customerAccount)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_GrantAccessToAccount(string planName, string accountId)
        {
            throw new System.NotImplementedException();
        }
    }
}