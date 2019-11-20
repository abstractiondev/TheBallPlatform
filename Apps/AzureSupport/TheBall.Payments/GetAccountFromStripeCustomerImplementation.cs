using System.Linq;
using System.Threading.Tasks;
using TheBall.Core;

namespace TheBall.Payments
{
    public class GetAccountFromStripeCustomerImplementation
    {
        public static async Task<CustomerAccount[]> GetTarget_AllCustomerAccountsAsync()
        {
            var masterCollection =
                await ObjectStorage.RetrieveFromOwnerContentA<CustomerAccountCollection>(InformationContext.CurrentOwner, "MasterCollection");
            return masterCollection.CollectionContent.ToArray();
        }

        public static CustomerAccount GetTarget_Account(string stripeCustomerID, bool isTestAccount, CustomerAccount[] allCustomerAccounts)
        {
            return allCustomerAccounts.FirstOrDefault(acc => acc.StripeID == stripeCustomerID && acc.IsTestAccount == isTestAccount);
        }

        public static GetAccountFromStripeCustomerReturnValue Get_ReturnValue(CustomerAccount account)
        {
            return new GetAccountFromStripeCustomerReturnValue {ResultAccount = account};
        }
    }
}