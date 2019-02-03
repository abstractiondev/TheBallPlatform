using System.Threading.Tasks;
using Footvoter.Services.INT;
using TheBall;

namespace Footvoter.Services
{
    public class UpdateUserProfileImplementation
    {
        public static async Task<UserProfile> GetTarget_UserProfileAsync()
        {
            var userProfile = await ObjectStorage.GetInterfaceObject<UserProfile>();
            return userProfile ?? new UserProfile();
        }


        public static async Task ExecuteMethod_StoreObjectsAsync(UserProfile userProfile)
        {
            await ObjectStorage.StoreInterfaceObject(userProfile);
        }

        public static void ExecuteMethod_SetUserProfileFields(UserProfile parametersProfileData, UserProfile userProfile)
        {
            userProfile.firstName = parametersProfileData.firstName;
            userProfile.lastName = parametersProfileData.lastName;
            userProfile.dateOfBirth = parametersProfileData.dateOfBirth;
            userProfile.description = parametersProfileData.description;
        }
    }
}