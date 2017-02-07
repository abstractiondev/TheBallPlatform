using TheBall.CORE.InstanceSupport;

namespace TheBall.Payments
{
    public class StripeSupport
    {
        public static string GetStripeApiKey(bool isTestMode)
        {
            return isTestMode
                ? SecureConfig.Current.StripeTestSecretKey
                : SecureConfig.Current.StripeLiveSecretKey;
        }
    }
}