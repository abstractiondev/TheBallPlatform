using System.IO;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TheBall.CORE
{
    public class PublishGroupToWwwImplementation
    {
        public static GroupContainer GetTarget_GroupContainer(IContainerOwner owner)
        {
            var groupContainer = ObjectStorage.RetrieveFromOwnerContent<GroupContainer>(owner, "default");
            return groupContainer;
        }

        public static string GetTarget_TargetContainerName(GroupContainer groupContainer)
        {
            return groupContainer.GroupProfile.WwwSiteToPublishTo.Replace(".", "-");
        }

        public static string GetTarget_TargetContainerOwnerString(string targetContainerName)
        {
            CloudBlockBlob blob = StorageSupport.GetBlob(targetContainerName, RenderWebSupport.CurrentToServeFileName);
            string contents = blob.DownloadText();
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