using System.Threading.Tasks;
using TheBall;
using TheBall.CORE.Storage;

namespace AaltoGlobalImpact.OIP
{
    public class ClearDefaultGroupFromAccountImplementation
    {
        public static async Task<AccountContainer> GetTarget_AccountContainerAsync()
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<AccountContainer>(InformationContext.CurrentOwner, "default");
        }

        public static string GetTarget_RedirectFromFolderBlobName()
        {
            return StorageSupport.GetOwnerRootAddress(InformationContext.CurrentOwner) + "RedirectFromFolder.red";
        }

        public static void ExecuteMethod_ClearDefaultGroupValue(AccountContainer accountContainer)
        {
            accountContainer.AccountModule.Profile.SimplifiedAccountGroupID = null;
            accountContainer.AccountModule.Profile.IsSimplifiedAccount = false;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(AccountContainer accountContainer)
        {
            await accountContainer.StoreInformationAsync(InformationContext.CurrentOwner);
        }

        public static async Task ExecuteMethod_RemoveAccountRedirectFileAsync(string redirectFromFolderBlobName)
        {
            var redirectBlob = StorageSupport.GetOwnerBlobReference(redirectFromFolderBlobName);
            await BlobStorage.DeleteBlobA(redirectBlob.Name);
        }
    }
}