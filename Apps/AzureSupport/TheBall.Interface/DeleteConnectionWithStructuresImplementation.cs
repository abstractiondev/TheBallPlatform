using System;
using System.Threading.Tasks;
using TheBall.CORE;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class DeleteConnectionWithStructuresImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }
        public static async Task<Connection> GetTarget_ConnectionAsync(string connectionId)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<Connection>(Owner, connectionId);
        }

        public static async Task ExecuteMethod_CallDeleteOnOtherEndAndDeleteOtherEndDeviceAsync(bool isLaunchedByRemoteDelete, Connection connection)
        {
            if (isLaunchedByRemoteDelete == false)
            {
                try
                {
                    var result = await DeviceSupport
                        .ExecuteRemoteOperation<ConnectionCommunicationData>(
                            connection.DeviceID,
                            "TheBall.Interface.ExecuteRemoteCalledConnectionOperation", new ConnectionCommunicationData
                                {
                                    ActiveSideConnectionID = connection.ID,
                                    ReceivingSideConnectionID = connection.OtherSideConnectionID,
                                    ProcessRequest = "DELETEREMOTECONNECTION"
                                });
                    bool success = result.ReceivingSideConnectionID == null;
                    await DeleteAuthenticatedAsActiveDevice.ExecuteAsync(new DeleteAuthenticatedAsActiveDeviceParameters
                        {
                            Owner = InformationContext.CurrentOwner,
                            AuthenticatedAsActiveDeviceID = connection.DeviceID
                        });
                }
                catch(Exception ex)
                {

                }
            }
        }

        public static async Task ExecuteMethod_DeleteConnectionIntermediateContentAsync(Connection connection)
        {
            string targetLocation;
            if (connection.IsActiveParty)
            {
                targetLocation = "TheBall.Interface/Connection/" + connection.ID;
            }
            else
            {
                if (connection.DeviceID != null)
                    targetLocation = "TheBall.CORE/DeviceMembership/" + connection.DeviceID + "_Input/";
                else
                    targetLocation = null;
            }
            if (targetLocation != null)
                await StorageSupport.DeleteBlobsFromOwnerTargetA(InformationContext.CurrentOwner, targetLocation);
        }

        public static async Task ExecuteMethod_DeleteConnectionProcessesAsync(Connection connection)
        {
            await DeleteProcess.ExecuteAsync(new DeleteProcessParameters
                {
                    ProcessID = connection.ProcessIDToUpdateThisSideCategories
                });
            await DeleteProcess.ExecuteAsync(new DeleteProcessParameters
                {
                    ProcessID = connection.ProcessIDToListPackageContents
                });
            await DeleteProcess.ExecuteAsync(new DeleteProcessParameters
                {
                    ProcessID = connection.ProcessIDToProcessReceived
                });
        }

        public static async Task ExecuteMethod_DeleteConnectionObjectAsync(Connection connection)
        {
            await connection.DeleteInformationObjectAsync(Owner);
        }
    }
}