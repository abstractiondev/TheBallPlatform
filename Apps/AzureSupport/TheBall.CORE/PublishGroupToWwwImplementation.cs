using System.IO;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TheBall.Core
{
    public class PublishGroupToWwwImplementation
    {
        public static async Task<GroupContainer> GetTarget_GroupContainerAsync(IContainerOwner owner)
        {
            var groupContainer = await ObjectStorage.RetrieveFromOwnerContentA<GroupContainer>(owner, "default");
            return groupContainer;
        }

        public static string GetTarget_TargetContainerName(GroupContainer groupContainer)
        {
            return groupContainer.GroupProfile.WwwSiteToPublishTo.Replace(".", "-");
        }

        public static async Task<string> GetTarget_TargetContainerOwnerStringAsync(string targetContainerName)
        {
            CloudBlockBlob blob = StorageSupport.GetBlob(targetContainerName, RenderWebSupport.CurrentToServeFileName);
            string contents = await blob.DownloadTextAsync();
            string[] contentArr = contents.Split(':');
            if (contentArr.Length < 2)
                return null;
            return contentArr[1];
        }

        public static void ExecuteMethod_ValidatePublishParameters(IContainerOwner owner, string targetContainerOwnerString)
        {
            string ownerString = owner.ContainerName + "/" + owner.LocationPrefix;
            if (ownerString != targetContainerOwnerString)
                throw new InvalidDataException("Mismatch in validation of Owner equaling to targetcontainer owner ID");

        }

        public static void ExecuteMethod_PublishWithWorker(IContainerOwner owner, string targetContainerName, string targetContainerOwnerString)
        {
            string sourceContainerName = StorageSupport.CurrActiveContainer.Name;
            string sourceOwner = owner.ContainerName + "/" + owner.LocationPrefix;
            string sourceRoot = "wwwsite";
            WorkerSupport.ProcessPublishWebContent(sourceContainerName, sourceOwner, sourceRoot, targetContainerName);
        }
    }
}