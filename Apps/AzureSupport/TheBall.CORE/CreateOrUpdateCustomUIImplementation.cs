using System;
using System.IO;
using System.IO.Compression;
using AaltoGlobalImpact.OIP;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.Core.Storage;

namespace TheBall.Core
{
    public class CreateOrUpdateCustomUIImplementation
    {
        public static async Task<GroupContainer> GetTarget_GroupContainerAsync(IContainerOwner owner)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<GroupContainer>(owner, "default");
        }

        public static string GetTarget_CustomUIFolder(IContainerOwner owner, string customUiName)
        {
            if(string.IsNullOrWhiteSpace(customUiName))
                throw new ArgumentException("Invalid custom UI name", customUiName);
            return StorageSupport.GetOwnerRootAddress(owner) + "customui_" + customUiName + "/";
        }

        public static void ExecuteMethod_SetCustomUIName(string customUiName, GroupContainer groupContainer)
        {
            var profile = groupContainer.GroupProfile;
            if (profile.CustomUICollection == null)
                profile.CustomUICollection = new ShortTextCollection();
            var customUICollection = groupContainer.GroupProfile.CustomUICollection;
            bool hasName = customUICollection.CollectionContent.Count(customUI => customUI.Content == customUiName) > 0;
            if (hasName)
                return;
            customUICollection.CollectionContent.Add(new ShortTextObject
                {
                    Content = customUiName
                });
            customUICollection.CollectionContent.Sort((x, y) => String.Compare(x.Content, y.Content));
        }

        public static async Task ExecuteMethod_CopyUIContentsFromZipArchiveAsync(Stream zipArchiveStream, string customUiFolder)
        {
            var blobListing = await BlobStorage.GetOwnerBlobsA(InformationContext.CurrentOwner, customUiFolder);
            foreach (var blob in blobListing)
            {
                await BlobStorage.DeleteBlobA(blob.Name);
            }
            ZipArchive zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Read);
            foreach (var zipEntry in zipArchive.Entries)
            {
                string blobFullName = customUiFolder + zipEntry.FullName;
                var entryStream = zipEntry.Open();
                await StorageSupport.CurrActiveContainer.UploadBlobStreamAsync(blobFullName, entryStream);
            }
        }

        public static async Task ExecuteMethod_StoreObjectAsync(GroupContainer groupContainer)
        {
            await groupContainer.StoreInformationAsync();
        }

        public static void ExecuteMethod_ValidateCustomUIName(string customUiName)
        {
            bool hasInvalidCharacters = customUiName.Any(ch => char.IsLetterOrDigit(ch) == false);
            if(hasInvalidCharacters)
                throw new InvalidDataException("Custom UI name is invalid (contains non-alphanumeric character(s)): " + customUiName);
        }
    }
}