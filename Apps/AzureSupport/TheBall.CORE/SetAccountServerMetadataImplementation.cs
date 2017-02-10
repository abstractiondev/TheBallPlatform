using System.Threading.Tasks;
using TheBall.CORE.INT;
using TheBall.CORE.Storage;

namespace TheBall.CORE
{
    public class SetAccountServerMetadataImplementation
    {
        public static async Task<Account> GetTarget_AccountAsync(AccountMetadata metadataInfo)
        {
            var account = await ObjectStorage.RetrieveFromSystemOwner<Account>(metadataInfo.AccountID);
            return account;
        }

        public static string GetTarget_MetadataAsJSONString(AccountMetadata metadataInfo)
        {
            var dataString = JSONSupport.SerializeToJSONString(metadataInfo.Data);
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