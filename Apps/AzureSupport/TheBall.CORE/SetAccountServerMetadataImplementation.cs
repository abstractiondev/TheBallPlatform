using System.Threading.Tasks;
using TheBall.CORE.INT;
using TheBall.CORE.Storage;

namespace TheBall.CORE
{
    public class SetAccountServerMetadataImplementation
    {
        public static async Task<Account> GetTarget_AccountAsync(string accountID)
        {
            var account = await ObjectStorage.RetrieveFromSystemOwner<Account>(accountID);
            return account;
        }

        public static string GetTarget_MetadataAsJSONString(JSONDataContainer dataContainer)
        {
            var dataString = JSONSupport.SerializeToJSONString(dataContainer.Data);
            return dataString;
        }

        public static void ExecuteMethod_SetAccountServerMetadata(Account account, string metadataAsJsonString)
        {
            account.ServerMetadataJSON = metadataAsJsonString;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Account account)
        {
            await account.StoreInformationAsync();
        }
    }
}