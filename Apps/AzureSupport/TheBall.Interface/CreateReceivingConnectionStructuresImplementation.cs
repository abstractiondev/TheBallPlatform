using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TheBall.CORE;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class CreateReceivingConnectionStructuresImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }

        public static async Task<Connection> GetTarget_ThisSideConnectionAsync(ConnectionCommunicationData connectionCommunicationData)
        {
            if(string.IsNullOrEmpty(connectionCommunicationData.ReceivingSideConnectionID))
                throw new InvalidDataException("ReceivingSideConnectionID be initialized to retrieve the connection");
            return await ObjectStorage.RetrieveFromOwnerContentA<Connection>(Owner, connectionCommunicationData.ReceivingSideConnectionID);
        }


        public static CreateConnectionStructuresParameters CallCreateConnectionStructures_GetParameters(Connection thisSideConnection)
        {
            return new CreateConnectionStructuresParameters
                {
                    ConnectionID = thisSideConnection.ID
                };
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Connection thisSideConnection)
        {
            await thisSideConnection.StoreInformationAsync();
        }
    }
}