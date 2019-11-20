using System;
using System.Threading.Tasks;
using TheBall.Core;

namespace TheBall.Interface
{
    public class ExecuteConnectionProcessImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }
        public static async Task<Connection> GetTarget_ConnectionAsync(string connectionId)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<Connection>(Owner, connectionId);
        }

        public static async Task ExecuteMethod_PerformProcessExecutionAsync(string connectionProcessToExecute, Connection connection)
        {
            string processID;
            switch (connectionProcessToExecute)
            {
                case "UpdateConnectionThisSideCategories":
                    processID = connection.ProcessIDToUpdateThisSideCategories;
                    break;
                case "ProcessReceived":
                    processID = connection.ProcessIDToProcessReceived;
                    break;
                default:
                    throw new NotImplementedException("Connection process execution not implemented for: " + connectionProcessToExecute);
            }
            await ExecuteProcess.ExecuteAsync(new ExecuteProcessParameters
            {
                ProcessID = processID
            });
        }
    }
}