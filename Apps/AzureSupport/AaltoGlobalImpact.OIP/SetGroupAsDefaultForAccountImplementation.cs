using System;
using System.Threading.Tasks;
using TheBall;
using TheBall.Core;
using TheBall.Core.StorageCore;

namespace AaltoGlobalImpact.OIP
{
    public class SetGroupAsDefaultForAccountImplementation
    {
        public static async Task<AccountContainer> GetTarget_AccountContainerAsync()
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<AccountContainer>(InformationContext.CurrentOwner, "default");
        }

        public static string GetTarget_RedirectFromFolderBlobName()
        {
            return StorageSupport.GetOwnerRootAddress(InformationContext.CurrentOwner) + "RedirectFromFolder.red";
        }

        public static void ExecuteMethod_SetDefaultGroupValue(string groupId, AccountContainer accountContainer)
        {
            accountContainer.AccountModule.Profile.IsSimplifiedAccount = true;
            accountContainer.AccountModule.Profile.SimplifiedAccountGroupID = groupId;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(AccountContainer accountContainer)
        {
            await accountContainer.StoreInformationAsync(InformationContext.CurrentOwner);
        }

        public static async Task ExecuteMethod_SetAccountRedirectFileToGroupAsync(string groupId, string redirectFromFolderBlobName)
        {
            var storageService = CoreServices.GetCurrent<IStorageService>();
            var redirectContents = $"/auth/grp/{groupId}/";
            await storageService.UploadBlobTextA(InformationContext.CurrentOwner, redirectFromFolderBlobName,
                redirectContents);
        }
    }
}