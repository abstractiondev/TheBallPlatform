using System.Threading.Tasks;
using TheBall.Core;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class FinalizeConnectionAfterGroupAuthorizationImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }

        public static async Task<Connection> GetTarget_ConnectionAsync(string connectionId)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<Connection>(Owner, connectionId);

        }

        public static ConnectionCommunicationData GetTarget_ConnectionCommunicationData(Connection connection)
        {
            return new ConnectionCommunicationData
                {
                    ActiveSideConnectionID = connection.ID,
                    ProcessRequest = "FINALIZECONNECTION"
                };
        }

        public static async Task ExecuteMethod_CallDeviceServiceForFinalizingAsync(Connection connection, ConnectionCommunicationData connectionCommunicationData)
        {
            connectionCommunicationData.ProcessParametersString = connection.Description;
            var result = await DeviceSupport
                .ExecuteRemoteOperation<ConnectionCommunicationData>(
                    connection.DeviceID,
                    "TheBall.Interface.ExecuteRemoteCalledConnectionOperation", connectionCommunicationData);
            connectionCommunicationData.ReceivingSideConnectionID = result.ReceivingSideConnectionID;

        }

        public static void ExecuteMethod_UpdateConnectionWithCommunicationData(Connection connection, ConnectionCommunicationData connectionCommunicationData)
        {
            connection.OtherSideConnectionID = connectionCommunicationData.ReceivingSideConnectionID;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Connection connection)
        {
            await connection.StoreInformationAsync();
        }

        public static CreateConnectionStructuresParameters CallCreateConnectionStructures_GetParameters(Connection connection)
        {
            return new CreateConnectionStructuresParameters {ConnectionID = connection.ID};
        }
    }
}