using System.Linq;
using Amazon.S3.Model;
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

        public static CustomerAccountCollection GetTarget_CurrentCustomers(IContainerOwner owner)
        {
            return CustomerAccountCollection.RetrieveFromOwnerContent(owner, "MasterCollection");
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

        public static void ExecuteMethod_StoreObjects(IContainerOwner owner, CustomerAccount[] newCustomersToCreate)
        {
            foreach (var customer in newCustomersToCreate)
                customer.StoreInformation(owner);
        }
    }
}