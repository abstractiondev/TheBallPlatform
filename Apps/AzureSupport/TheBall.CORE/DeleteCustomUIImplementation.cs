using System;
using System.Linq;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.CORE.Storage;

namespace TheBall.CORE
{
    public class DeleteCustomUIImplementation
    {
        public static async Task<GroupContainer> GetTarget_GroupContainerAsync(IContainerOwner owner)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<GroupContainer>(owner, "default");
        }

        public static string GetTarget_CustomUIFolder(IContainerOwner owner, string customUiName)
        {
            return StorageSupport.GetOwnerRootAddress(owner) + "customui_" + customUiName + "/";
        }

        public static void ExecuteMethod_RemoveCustomUIName(string customUiName, GroupContainer groupContainer)
        {
            var profile = groupContainer.GroupProfile;
            if (profile.CustomUICollection == null)
                profile.CustomUICollection = new ShortTextCollection();
            var customUICollection = groupContainer.GroupProfile.CustomUICollection;
            customUICollection.CollectionContent.RemoveAll(customUIEntry => customUIEntry.Content == customUiName);

        }

        public static async Task ExecuteMethod_RemoveCustomUIContents(string customUiFolder)
        {
            var blobListing = await BlobStorage.GetBlobItemsA(null, customUiFolder);
            var tasks = blobListing.Select(item => BlobStorage.DeleteBlobA(item.Name)).ToArray();
            await Task.WhenAll(tasks);
        }

        public static async Task ExecuteMethod_StoreObject(GroupContainer groupContainer)
        {
            await groupContainer.StoreInformationAsync();
        }
    }
}