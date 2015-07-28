using System.Linq;

namespace TheBall.Payments
{
    public class GetAccountFromStripeCustomerImplementation
    {
        public static CustomerAccount[] GetTarget_AllCustomerAccounts()
        {
            var masterCollection =
                ObjectStorage.RetrieveFromOwnerContent<CustomerAccountCollection>(InformationContext.CurrentOwner, "MasterCollection");
            return masterCollection.CollectionContent.ToArray();
        }

        public static CustomerAccount GetTarget_Account(string stripeCustomerID, CustomerAccount[] allCustomerAccounts)
        {
            return allCustomerAccounts.FirstOrDefault(acc => acc.StripeID == stripeCustomerID);
        }

        public static GetAccountFromStripeCustomerReturnValue Get_ReturnValue(CustomerAccount account)
        {
            return new GetAccountFromStripeCustomerReturnValue {ResultAccount = account};
        }
    }
}