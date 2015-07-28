using System;
using System.IO;
using AzureSupport;

namespace TheBall.Interface
{
    public class ExecuteInterfaceOperationImplementation
    {
        public static InterfaceOperation GetTarget_Operation(string operationID)
        {
            return ObjectStorage.RetrieveFromOwnerContent<InterfaceOperation>(InformationContext.CurrentOwner, operationID);
        }

        public static void ExecuteMethod_ExecuteOperation(InterfaceOperation operation, string operationDataLocation)
        {
            byte[] operationData = StorageSupport.CurrActiveContainer.DownloadBlobBinary(operationDataLocation);
            HttpOperationData reqData = null;
            using (var memStream = new MemoryStream(operationData))
            {
                reqData = memStream.DeserializeProtobuf<HttpOperationData>();
            }
            try
            {
                operation.OperationName = reqData.OperationName;
                operation.Started = DateTime.UtcNow;
                operation.StoreInformation();
                OperationSupport.ExecuteHttpOperation(reqData);

                //throw new NotImplementedException("TODO progress event introduction above and store object status; including support for StatusUpdates");
                // Finished cleanup
                operation.DeleteInformationObject();
                StorageSupport.DeleteWithoutFiringSubscriptions(operationDataLocation);
            }
            catch (Exception ex)
            {
                operation.ErrorCode = ex.HResult.ToString();
                operation.ErrorMessage = ex.Message;
                operation.Finished = DateTime.UtcNow;
                operation.StoreInformation();
            }
        }

        public static string GetTarget_OperationDataLocation(InterfaceOperation operation)
        {
            return operation.RelativeLocation + ".data";
        }
    }
}