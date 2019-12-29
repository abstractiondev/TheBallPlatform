using System;
using System.Threading.Tasks;
using TheBall.Core;
using TheBall.Core.StorageCore;

namespace TheBall.Interface
{
    public class CreateInterfaceOperationForExecutionImplementation
    {
        public static InterfaceOperation GetTarget_Operation(string dataType)
        {
            if(dataType != OperationSupport.HttpOperationDataType)
                throw new ArgumentException("OperationDataType not supported: " + dataType, "dataType");

            var operation = new InterfaceOperation();
            operation.SetLocationAsOwnerContent(InformationContext.CurrentOwner, operation.ID);
            operation.OperationDataType = dataType;
            return operation;
        }

        public static CreateInterfaceOperationForExecutionReturnValue Get_ReturnValue(InterfaceOperation operation)
        {
            return new CreateInterfaceOperationForExecutionReturnValue {OperationID = operation.ID};
        }

        public static string GetTarget_OperationDataLocation(InterfaceOperation operation)
        {
            return operation.RelativeLocation + ".data";
        }

        public static async Task ExecuteMethod_StoreOperationWithDataAsync(byte[] operationData, InterfaceOperation operation, string operationDataLocation)
        {
            if (operation.OperationDataType != OperationSupport.HttpOperationDataType)
                throw new NotSupportedException("OperationDataType not supported: " + operation.OperationDataType);
            await operation.StoreInformationAsync();
            var storageService = CoreServices.GetCurrent<IStorageService>();
            await storageService.UploadBlobDataA(InformationContext.CurrentOwner, operationDataLocation, operationData);
        }
    }
}