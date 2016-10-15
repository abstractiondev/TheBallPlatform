using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TheBall.CORE.Storage
{
    public class BlobStorage
    {
        const string InterfaceDataPrefixFolder = "TheBall.Interface/InterfaceData";
        const string ShareDataPrefixFolder = "TheBall.Interface/ShareInfo";

        public static async Task<BlobStorageItem> GetBlobStorageItemA(string sourceFullPath)
        {
            BlobStorageItem blob = await StorageSupport.GetBlobStorageItem(sourceFullPath);
            return blob;
        }


        public static string GetOwnerInterfaceDataFullPath(string fileName)
        {
            var interfaceDataName = Path.Combine(InterfaceDataPrefixFolder, fileName).Replace("\\", "/");
            if (!interfaceDataName.StartsWith(InterfaceDataPrefixFolder + "/"))
                throw new ArgumentException("Relative filename not allowed: " + fileName);
            var ownerPrefixed = GetOwnerContentLocation(InformationContext.CurrentOwner, interfaceDataName);
            return ownerPrefixed;
        }

        public static string GetCollaborationOwnerShareFullPath(IContainerOwner collaborationTarget, string shareFileName, bool isMetadataFile)
        {
            string storedFileName;
            if (isMetadataFile)
            {
                var injectIndex = shareFileName.LastIndexOf('/') + 1;
                var beforeInject = shareFileName.Substring(0, injectIndex);
                var afterInject = shareFileName.Substring(injectIndex);
                storedFileName = beforeInject + "_" + afterInject + ".json";
            }
            else
                storedFileName = shareFileName;
            var interfaceDataName = Path.Combine(ShareDataPrefixFolder, collaborationTarget.ContainerName,
                collaborationTarget.LocationPrefix, storedFileName).Replace("\\", "/");
            if (!interfaceDataName.StartsWith(ShareDataPrefixFolder + "/"))
                throw new ArgumentException("Relative filename not allowed: " + shareFileName);
            var ownerPrefixed = GetOwnerContentLocation(InformationContext.CurrentOwner, interfaceDataName);
            return ownerPrefixed;
        }

        public static string GetOwnerContentLocation(IContainerOwner owner, string blobAddress)
        {
            string ownerPrefix = owner.ContainerName + "/" + owner.LocationPrefix + "/";
            if (blobAddress.StartsWith("grp/") || blobAddress.StartsWith("acc/") || blobAddress.StartsWith("sys/"))
            {
                if (blobAddress.StartsWith(ownerPrefix))
                    return blobAddress;
                throw new SecurityException("Invalid reference to blob: " + blobAddress + " by owner prefix: " + owner.LocationPrefix);
            }
            return ownerPrefix + blobAddress;
        }

        public static async Task UploadCurrentOwnerBlobTextAsync(string metadataFullPath, string jsonData)
        {
            var owner = InformationContext.CurrentOwner;
            await StorageSupport.UploadOwnerBlobTextAsync(owner, metadataFullPath, jsonData);
        }
    }
}
