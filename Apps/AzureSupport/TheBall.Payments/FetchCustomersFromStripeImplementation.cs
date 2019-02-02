using System.Linq;
using System.Threading.Tasks;
using Stripe;
using TheBall.CORE;

namespace TheBall.Payments
{
    public class FetchCustomersFromStripeImplementation
    {
        public static IContainerOwner GetTarget_Owner(string groupId)
        {
            throw new System.NotImplementedException();
        }

        public static StripeCustomer[] GetTarget_StripeCustomers()
        {
            var customerService = new StripeCustomerService();
            var customers = customerService.List().ToArray();
            return customers;
        }

        public static async Task<CustomerAccountCollection> GetTarget_CurrentCustomersAsync(IContainerOwner owner)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<CustomerAccountCollection>(owner, "MasterCollection");
        }

        public static CustomerAccount[] GetTarget_NewCustomersToCreate(IContainerOwner owner, StripeCustomer[] stripeCustomers, CustomerAccountCollection currentCustomers)
        {
            var newStripeCustomers =
                stripeCustomers.Where(
                    sCustomer =>
                        currentCustomers.CollectionContent.All(customer => customer.StripeID != sCustomer.Id.ToLower())).ToArray();
            var newCustomers = newStripeCustomers.Select(sCustomer =>
            {
                var customer = new CustomerAccount();
                customer.SetLocationAsOwnerContent(owner, customer.ID);
                customer.EmailAddress = sCustomer.Email;
                customer.Description = sCustomer.Description;
                return customer;
            }).ToArray();
            return newCustomers;
        }

        public static async Task ExecuteMethod_StoreObjectsAsync(IContainerOwner owner, CustomerAccount[] newCustomersToCreate)
        {
            foreach (var customer in newCustomersToCreate)
                await customer.StoreInformationAsync(owner);
        }
    }
}