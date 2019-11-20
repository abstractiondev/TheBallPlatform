using System.Diagnostics;
using System.Threading.Tasks;
using TheBall.Core;

namespace TheBall.Admin
{
    public class FixGroupMastersAndCollectionsImplementation
    {
        public static async Task ExecuteMethod_FixMastersAndCollectionsAsync(string groupID)
        {
            Debug.WriteLine("Fixing group: " + groupID);
            Group owner = await ObjectStorage.RetrieveFromSystemOwner<Group>(groupID);
            await owner.InitializeAndConnectMastersAndCollections();
        }
    }
}