using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using AzureSupport;
using TheBall.CORE;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class ShareCollabInterfaceObjectImplementation
    {
        public static string GetTarget_SourceFullPath(string fileName)
        {
            return StorageSupport.GetOwnerInterfaceDataFullPath(fileName);
        }

        public static string GetTarget_MetadataFullPath(string fileName, IContainerOwner collaborationTarget)
        {
            var metadataFullPath = StorageSupport.GetCollaborationOwnerShareFullPath(collaborationTarget, fileName, true);
            return metadataFullPath;
        }
        public static async Task<ShareInfo> GetTarget_MetadataObjectAsync(string fileName, string sourceFullPath)
        {
            var fileInfo = await StorageSupport.GetFileInfoA(sourceFullPath);
            string contentMD5 = fileInfo.Item1;
            long length = fileInfo.Item2;
            DateTime modified = fileInfo.Item3;
            var result = new ShareInfo
            {
                ItemName = fileName,
                ContentMD5 = contentMD5,
                Length = length,
                Modified = modified
            };
            return result;
        }

        public static async Task ExecuteMethod_StoreShareMetadataAsync(string metadataFullPath, ShareInfo metadataObject)
        {
            var jsonData = JSONSupport.SerializeToJSONString(metadataObject);
            var owner = InformationContext.CurrentOwner;
            await StorageSupport.UploadOwnerBlobTextAsync(owner, metadataFullPath, jsonData);
        }

        public static IContainerOwner GetTarget_CollaborationTarget(string colTargetType, string colTargetId)
        {
            VirtualOwner owner = new VirtualOwner(colTargetType, colTargetId, true);
            return owner;
        }
    }
}