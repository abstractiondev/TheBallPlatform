using System;
using System.Threading.Tasks;

namespace TheBall.Interface
{
    public class DeleteInterfaceOperationImplementation
    {
        public static string GetTarget_OperationBlobLocation(string operationID)
        {
            string operationLocation = ObjectStorage.GetRelativeLocationFromID<InterfaceOperation> (operationID);
            return StorageSupport.GetOwnerContentLocation(InformationContext.CurrentOwner, operationLocation);
        }

        public static string GetTarget_OperationDataBlobLocation(string operationBlobLocation)
        {
            return operationBlobLocation + ".data";
        }

        public static async Task ExecuteMethod_DeleteOperationWithDataAsync(string operationBlobLocation, string operationDataBlobLocation)
        {
            await StorageSupport.DeleteBlobAsync(operationBlobLocation);
            await StorageSupport.DeleteBlobAsync(operationDataBlobLocation);
        }
    }
}