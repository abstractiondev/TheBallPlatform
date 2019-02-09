using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using Ionic.Zip;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class PackageAndPushCollaborationContentImplementation
    {
        public static async Task<Connection> GetTarget_ConnectionAsync(string connectionId)
        {
            Connection connection = await ObjectStorage.RetrieveFromOwnerContentA<Connection>(InformationContext.CurrentOwner,
                                                                        connectionId);
            return connection;
        }

        public static string GetTarget_PackageContentListingOperationName(Connection connection)
        {
            return connection.OperationNameToListPackageContents;
        }

        public static string[] ExecuteMethod_DynamicPackageListingOperation(string connectionId, string packageContentListingOperationName)
        {
            // TODO: Fix reflection call for operation
            string operationTypeName = packageContentListingOperationName;
            string parameterTypeName = operationTypeName + "Parameters";
            string returnValueTypeName = operationTypeName + "ReturnValue";
            Type operationType = Type.GetType(operationTypeName);
            Type parameterType = Type.GetType(parameterTypeName);
            Type returnValueType = Type.GetType(returnValueTypeName);
            if(operationType == null)
                throw new InvalidDataException("Invalid operation type (not found): " + operationTypeName);
            if(parameterType == null)
                throw new InvalidDataException("Invalid parameter type (not found):" + parameterTypeName);
            if(returnValueType == null)
                throw new InvalidDataException("Invalid return value type (not found): " + returnValueTypeName);
            dynamic parameterObject = Activator.CreateInstance(parameterType);
            parameterObject.ConnectionID = connectionId;
            dynamic returnValueObject = operationType.GetMethod("Execute").Invoke(null, new object[] {parameterObject});
            return returnValueObject.ContentLocations;

        }

        public static TransferPackage GetTarget_TransferPackage(string connectionId)
        {
            TransferPackage transferPackage = new TransferPackage();
            transferPackage.ConnectionID = connectionId;
            transferPackage.SetLocationAsOwnerContent(InformationContext.CurrentOwner, transferPackage.ID);
            return transferPackage;
        }

        public static void ExecuteMethod_AddTransferPackageToConnection(Connection connection, TransferPackage transferPackage)
        {
            connection.OutgoingPackages.Add(transferPackage);
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Connection connection)
        {
            await connection.StoreInformationAsync();
        }

        public static string[] ExecuteMethod_PackageTransferPackageContent(TransferPackage transferPackage, string[] dynamicPackageListingOperationOutput)
        {
            string[] blobAddresses = new string[0];
            transferPackage.PackageContentBlobs.AddRange(blobAddresses);
            List<string> zipPackageNames = new List<string>();
            return zipPackageNames.ToArray();
        }

        public static async Task ExecuteMethod_SendTransferPackageContentAsync(Connection connection, TransferPackage transferPackage, string[] packageTransferPackageContentOutput)
        {
            string informationOutputID = connection.OutputInformationID;
            var fileNames = packageTransferPackageContentOutput;
            foreach (string fileName in fileNames)
            {
                await CORE.PushToInformationOutput.ExecuteAsync(
                    new PushToInformationOutputParameters
                        {
                            InformationOutputID = informationOutputID,
                            LocalContentName = fileName,
                            Owner = InformationContext.CurrentOwner,
                            SpecificDestinationContentName = fileName
                        });
            }
        }

        public static void ExecuteMethod_SetTransferPackageAsProcessed(TransferPackage transferPackage)
        {
            transferPackage.IsProcessed = true;
        }

        public static async Task ExecuteMethod_StoreObjectCompleteAsync(Connection connection, TransferPackage transferPackage)
        {
            int retryCount = 10;
            while (true)
            {
                try
                {
                    await connection.StoreInformationAsync();
                    break;
                }
                catch
                {
                    if (retryCount-- <= 0)
                        throw;
                    connection = await ObjectStorage.RetrieveFromOwnerContentA<Connection>(InformationContext.CurrentOwner, connection.ID);
                    connection.OutgoingPackages.Add(transferPackage);
                }
            }
        }
    }
}