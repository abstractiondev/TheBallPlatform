using System;
using System.Security;
using System.Linq;
using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class DeleteSpecifiedInformationObjectImplementation
    {
        public static async Task<IInformationObject> GetTarget_ObjectToDeleteAsync(IContainerOwner owner, string objectDomainName, string objectName, string objectId)
        {
            IInformationObject objectToDelete =
                await ObjectStorage.RetrieveFromDefaultLocationA(objectDomainName, objectName, objectId, owner);
            return objectToDelete;
        }

        public static async Task ExecuteMethod_DeleteObjectAsync(IInformationObject objectToDelete)
        {
            if(objectToDelete != null)
                await objectToDelete.DeleteInformationObjectAsync();
        }

        public static void ExecuteMethod_CatchInvalidDomains(string objectDomainName)
        {
            if (SystemSupport.ReservedDomainNames.Contains(objectDomainName))
                throw new SecurityException("Creation of system namespace objects is not permitted");
        }
    }
}