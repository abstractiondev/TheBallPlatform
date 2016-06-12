using System;
using System.IO;
using System.Threading.Tasks;
using AzureSupport;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;

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
            catch (Exception exceptionToReport)
            {
                if (exceptionToReport.InnerException != null)
                    exceptionToReport = exceptionToReport.InnerException;
                exceptionToReport.ReportException();
                operation.ErrorCode = exceptionToReport.HResult.ToString();
                operation.ErrorMessage = exceptionToReport.Message;
                operation.Finished = DateTime.UtcNow;
                await operation.StoreInformationAsync();
            }
        }

        public static async Task ExecuteMethod_PreExecuteSyncSQLiteFromStorageAsync(bool useSqLiteDb, InterfaceOperation operation)
        {
            if (!useSqLiteDb)
                return;
            var ownerDomainResult = await GetOwnerSemanticDomains.ExecuteAsync(new GetOwnerSemanticDomainsParameters { SkipSystemDomains = true});
            var ownerDomains = ownerDomainResult.OwnerSemanticDomains;
            foreach (var ownerDomain in ownerDomains)
            {
                await UpdateOwnerDomainObjectsInSQLiteStorage.ExecuteAsync(new UpdateOwnerDomainObjectsInSQLiteStorageParameters
                {
                    Owner = InformationContext.CurrentOwner,
                    SemanticDomain = ownerDomain
                });
            }
        }

        public static async Task ExecuteMethod_PostExecuteSyncStorageFromSQLiteAsync(bool useSqLiteDb, InterfaceOperation operation)
        {
            if (!useSqLiteDb)
                return;
        }
    }
}