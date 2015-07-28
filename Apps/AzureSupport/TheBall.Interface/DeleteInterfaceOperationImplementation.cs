using System;

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

        public static void ExecuteMethod_DeleteOperationWithData(string operationBlobLocation, string operationDataBlobLocation)
        {
            StorageSupport.DeleteWithoutFiringSubscriptions(operationBlobLocation);
            StorageSupport.DeleteWithoutFiringSubscriptions(operationDataBlobLocation);
        }
    }
}