using System.Threading.Tasks;
using TheBall.Core.INT;
using TheBall.Core.Storage;

namespace TheBall.Core
{
    public class SetAccountClientMetadataImplementation
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

        public static void ExecuteMethod_SetAccountClientMetadata(Account account, string metadataAsJsonString)
        {
            account.ClientMetadataJSON = metadataAsJsonString;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Account account)
        {
            await account.StoreInformationAsync();
        }
    }
}