using System;
using System.IO;
using System.Threading.Tasks;
using AzureSupport;

namespace TheBall.Interface
{
    public class ExecuteInterfaceOperationImplementation
    {
        public static string GetTarget_OperationDataLocation(InterfaceOperation operation)
        {
            return operation.RelativeLocation + ".data";
        }

        public static async Task<InterfaceOperation> GetTarget_OperationAsync(string operationID)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<InterfaceOperation>(InformationContext.CurrentOwner, operationID);
        }

        public static async Task ExecuteMethod_ExecuteOperationAsync(InterfaceOperation operation, string operationDataLocation)
        {
            byte[] operationData = await StorageSupport.DownloadBlobByteArrayAsync(operationDataLocation);
            HttpOperationData reqData = null;
            using (var memStream = new MemoryStream(operationData))
            {
                reqData = memStream.DeserializeProtobuf<HttpOperationData>();
            }
            try
            {
                operation.OperationName = reqData.OperationName;
                operation.Started = DateTime.UtcNow;
                await operation.StoreInformationAsync();
                await OperationSupport.ExecuteHttpOperationAsync(reqData);
                await operation.DeleteInformationObjectAsync();
                await StorageSupport.DeleteBlobAsync(operationDataLocation);
            }
            catch (Exception ex)
            {
                operation.ErrorCode = ex.HResult.ToString();
                operation.ErrorMessage = ex.Message;
                operation.Finished = DateTime.UtcNow;
                await operation.StoreInformationAsync();
            }
        }
    }
}