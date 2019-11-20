using System.Linq;
using System.Threading.Tasks;
using TheBall;
using TheBall.Core;

namespace ProBroz.OnlineTraining
{
    public class GetOrInitiateDefaultGymImplementation
    {
        public static async Task<TenantGymCollection> GetTarget_AllGymsAsync()
        {
            var masterCollection =
                await ObjectStorage.RetrieveFromOwnerContentA<TenantGymCollection>("MasterCollection");
            return masterCollection;
        }

        public static async Task<TenantGym> ExecuteMethod_GetOrCreateDefaultGymIfMissingAsync(TenantGymCollection allGyms)
        {
            var existingGym = allGyms?.CollectionContent.FirstOrDefault();
            if (existingGym == null)
            {
                existingGym = new TenantGym()
                {
                    Address = "Default gym address",
                    Address2 = "Default gym address 2",
                    Country = "Default gym country",
                    Email = "default.gym@email.com",
                    GymName = "Default gym name",
                    PhoneNumber = "Default gym phonenumber",
                    PostOffice = "Default gym postoffice",
                    ZipCode = "Default gym zipcode"
                };
                existingGym.FixCurrentOwnerLocation();
                await existingGym.StoreInformationAsync(InformationContext.CurrentOwner);
            }
            return existingGym;
        }

        public static GetOrInitiateDefaultGymReturnValue Get_ReturnValue(TenantGym getOrCreateDefaultGymIfMissingOutput)
        {
            return new GetOrInitiateDefaultGymReturnValue {DefaultGym = getOrCreateDefaultGymIfMissingOutput};
        }
    }
}