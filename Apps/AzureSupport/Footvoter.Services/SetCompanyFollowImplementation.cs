using System.Threading.Tasks;
using Footvoter.Services.INT;
using TheBall;

namespace Footvoter.Services
{
    public class SetCompanyFollowImplementation
    {
        public static async Task<CompanyFollowData> GetTarget_FollowDataAsync()
        {
            var companyFollowData = await ObjectStorage.GetInterfaceObject<CompanyFollowData>();
            return companyFollowData;
        }

        public static void ExecuteMethod_SetCompanyFollowData(CompanyFollowData parametersFollowDataInput, CompanyFollowData followData)
        {
            followData.FollowDataItems = parametersFollowDataInput.FollowDataItems;
        }

        public static async Task ExecuteMethod_StoreObjectsAsync(CompanyFollowData followData)
        {
            await ObjectStorage.StoreInterfaceObject(followData);
        }

    }
}