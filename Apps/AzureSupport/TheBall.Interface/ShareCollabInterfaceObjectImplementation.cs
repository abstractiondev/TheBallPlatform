using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using AzureSupport;
using TheBall.CORE;
using TheBall.CORE.Storage;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class ShareCollabInterfaceObjectImplementation
    {
        public static string GetTarget_SourceFullPath(string fileName)
        {
            return BlobStorage.GetOwnerInterfaceDataFullPath(fileName);
        }

        public static string GetTarget_MetadataFullPath(string fileName, IContainerOwner collaborationTarget)
        {
            var metadataFullPath = BlobStorage.GetCollaborationOwnerShareFullPath(collaborationTarget, fileName, true);
            return metadataFullPath;
        }
        public static async Task<ShareInfo> GetTarget_MetadataObjectAsync(string fileName, string sourceFullPath)
        {
            var blob = await BlobStorage.GetBlobStorageItemA(sourceFullPath);
            if (blob == null)
                return null;
            var result = new ShareInfo
            {
                ItemName = fileName,
                ContentMD5 = blob.ContentMD5,
                Length = blob.Length,
                Modified = blob.LastModified
            };
            return result;
        }

        public static async Task ExecuteMethod_StoreShareMetadataAsync(string metadataFullPath, ShareInfo metadataObject)
        {
            if (metadataObject != null)
            {
                var jsonData = JSONSupport.SerializeToJSONString(metadataObject);
                await BlobStorage.UploadCurrentOwnerBlobTextAsync(metadataFullPath, jsonData);
            }
        }

        public static IContainerOwner GetTarget_CollaborationTarget(ShareCollabParams collabParams)
        {
            var partner = collabParams.Partner;
            VirtualOwner owner = new VirtualOwner(partner.PartnerType, partner.PartnerID, true);
            return owner;
        }

        public static string GetTarget_FileName(ShareCollabParams collabParams)
        {
            return collabParams.FileName;
        }

        public static PushSyncNotificationParameters NotifyPartner_GetParameters(ShareCollabParams collabParams)
        {
            return new PushSyncNotificationParameters
            {
                Partner = collabParams.Partner
            };
        }

        public static void ExecuteMethod_ValidateFileName(string fileName)
        {
            bool isMetadataAlike = fileName.StartsWith("_") && fileName.EndsWith(".json");
            if(isMetadataAlike)
                throw new SecurityException("Invalid Metadata alike filename: " + fileName);
        }
    }
}