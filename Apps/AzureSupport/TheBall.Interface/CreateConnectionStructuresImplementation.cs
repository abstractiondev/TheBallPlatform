using System.Threading.Tasks;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class CreateConnectionStructuresImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }

        public static async Task<Connection> GetTarget_ConnectionAsync(string connectionId)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<Connection>(Owner, connectionId);
        }

        public static async Task<Process> GetTarget_ProcessToListPackageContentsAsync(Connection connection)
        {
            CreateProcessParameters processParameters = new CreateProcessParameters
                {
                    ExecutingOperationName = "AaltoGlobalImpact.OIP.ListConnectionPackageContents",
                    InitialArguments = new SemanticInformationItem[] {new SemanticInformationItem("ConnectionID", connection.ID)},
                    ProcessDescription = "Process to list package contents"
                };
            var result = await CreateProcess.ExecuteAsync(processParameters);
            return result.CreatedProcess;
        }

        public static async Task<Process> GetTarget_ProcessToProcessReceivedDataAsync(Connection connection)
        {
            CreateProcessParameters processParameters = new CreateProcessParameters
            {
                ExecutingOperationName = "AaltoGlobalImpact.OIP.ProcessConnectionReceivedData",
                InitialArguments = new SemanticInformationItem[] { new SemanticInformationItem("ConnectionID", connection.ID) },
                ProcessDescription = "Process to list package contents"
            };
            var result = await CreateProcess.ExecuteAsync(processParameters);
            return result.CreatedProcess;
        }

        public static async Task<Process> GetTarget_ProcessToUpdateThisSideCategoriesAsync(Connection connection)
        {
            CreateProcessParameters processParameters = new CreateProcessParameters
            {
                ExecutingOperationName = "AaltoGlobalImpact.OIP.UpdateConnectionThisSideCategories",
                InitialArguments = new SemanticInformationItem[] { new SemanticInformationItem("ConnectionID", connection.ID) },
                ProcessDescription = "Process to list package contents"
            };
            var result = await CreateProcess.ExecuteAsync(processParameters);
            return result.CreatedProcess;
        }

        public static void ExecuteMethod_SetConnectionProcesses(Connection connection, Process processToListPackageContents, Process processToProcessReceivedData, Process processToUpdateThisSideCategories)
        {
            connection.ProcessIDToListPackageContents = processToListPackageContents.ID;
            connection.ProcessIDToProcessReceived = processToProcessReceivedData.ID;
            connection.ProcessIDToUpdateThisSideCategories = processToUpdateThisSideCategories.ID;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Connection connection)
        {
            await connection.StoreInformationAsync();
        }

        public static CreateConnectionStructuresReturnValue Get_ReturnValue(Connection connection)
        {
            return new CreateConnectionStructuresReturnValue {UpdatedConnection = connection};
        }
    }
}