using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.CORE.InstanceSupport;

namespace TheBall.CORE
{
    public class SetOwnerWebRedirectImplementation
    {
        public static void ExecuteMethod_SetRedirection(IContainerOwner owner, string redirectPath)
        {
            CloudBlockBlob redirectBlob = StorageSupport.GetOwnerBlobReference(owner,
                                                                  InfraSharedConfig.Current.RedirectFromFolderFileName);
            if (string.IsNullOrEmpty(redirectPath))
                redirectBlob.DeleteIfExists();
            else
                redirectBlob.UploadText(redirectPath);
            
        }
    }
}