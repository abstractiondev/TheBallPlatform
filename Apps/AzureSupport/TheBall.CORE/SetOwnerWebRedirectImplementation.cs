using Microsoft.WindowsAzure.StorageClient;
using TheBall.CORE.InstanceSupport;

namespace TheBall.CORE
{
    public class SetOwnerWebRedirectImplementation
    {
        public static void ExecuteMethod_SetRedirection(IContainerOwner owner, string redirectPath)
        {
            CloudBlob redirectBlob = StorageSupport.GetOwnerBlobReference(owner,
                                                                  InfraSharedConfig.Current.RedirectFromFolderFileName);
            if (string.IsNullOrEmpty(redirectPath))
                redirectBlob.DeleteIfExists();
            else
                redirectBlob.UploadText(redirectPath);
            
        }
    }
}