using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureSupport;
using TheBall.Core;
using TheBall.Core.Storage;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class ExecuteRemoteCalledConnectionOperationImplementation
    {
        public static ConnectionCommunicationData GetTarget_ConnectionCommunicationData(Stream inputStream)
        {
            return JSONSupport.GetObjectFromStream<ConnectionCommunicationData>(inputStream);
        }

        public static async Task ExecuteMethod_PerformOperationAsync(ConnectionCommunicationData connectionCommunicationData)
        {
            switch (connectionCommunicationData.ProcessRequest)
            {
                case "PROCESSPUSHEDCONTENT":
                    {
                        await ExecuteConnectionProcess.ExecuteAsync(new ExecuteConnectionProcessParameters
                            {
                                ConnectionID = connectionCommunicationData.ReceivingSideConnectionID,
                                ConnectionProcessToExecute = "ProcessReceived"
                            });
                        break;
                    }
                case "SYNCCATEGORIES":
                    {
                        await ExecuteConnectionProcess.ExecuteAsync(new ExecuteConnectionProcessParameters
                            {
                                ConnectionID = connectionCommunicationData.ReceivingSideConnectionID,
                                ConnectionProcessToExecute = "UpdateConnectionThisSideCategories"
                            });
                        Connection thisSideConnection = await ObjectStorage.RetrieveFromOwnerContentA<Connection>(InformationContext.CurrentOwner,
                                                                                            connectionCommunicationData.ReceivingSideConnectionID);
                        thisSideConnection.OtherSideCategories.Clear();
                        thisSideConnection.OtherSideCategories.AddRange(connectionCommunicationData.CategoryCollectionData.Select(catInfo => catInfo.ToCategory()));
                        thisSideConnection.CategoryLinks.Clear();
                        thisSideConnection.CategoryLinks.AddRange(connectionCommunicationData.LinkItems.Select(catLinkItem => new CategoryLink
                            {
                                SourceCategoryID = catLinkItem.SourceCategoryID,
                                TargetCategoryID = catLinkItem.TargetCategoryID,
                                LinkingType = catLinkItem.LinkingType
                            }));
                        await thisSideConnection.StoreInformationAsync();
                        connectionCommunicationData.CategoryCollectionData = thisSideConnection.ThisSideCategories.Select(CategoryInfo.FromCategory).ToArray();
                        break;
                    }
                case "FINALIZECONNECTION":
                    var output = await CreateReceivingConnection.ExecuteAsync(new CreateReceivingConnectionParameters
                        {
                            Description = connectionCommunicationData.ProcessParametersString,
                            OtherSideConnectionID = connectionCommunicationData.ActiveSideConnectionID
                        });
                    connectionCommunicationData.ReceivingSideConnectionID = output.ConnectionID;
                    await CreateReceivingConnectionStructures.ExecuteAsync(new CreateReceivingConnectionStructuresParameters
                        {
                            ConnectionCommunicationData = connectionCommunicationData
                        });
                    break;
                case "DELETEREMOTECONNECTION":
                    await DeleteConnectionWithStructures.ExecuteAsync(new DeleteConnectionWithStructuresParameters
                        {
                            ConnectionID = connectionCommunicationData.ReceivingSideConnectionID,
                            IsLaunchedByRemoteDelete = true
                        });
                    connectionCommunicationData.ReceivingSideConnectionID = null;
                    break;
                default:
                    break;
            }
        }

        public static void ExecuteMethod_SerializeCommunicationDataToOutput(Stream outputStream, ConnectionCommunicationData connectionCommunicationData)
        {
            JSONSupport.SerializeToJSONStream(connectionCommunicationData, outputStream);
        }
    }
}