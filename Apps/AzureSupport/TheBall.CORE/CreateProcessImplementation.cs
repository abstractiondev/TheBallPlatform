using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class CreateProcessImplementation
    {
        public static Process GetTarget_Process(string processDescription, string executingOperationName, SemanticInformationItem[] initialArguments)
        {
            Process process = new Process
                {
                    ProcessDescription = processDescription,
                    ExecutingOperation = new SemanticInformationItem(executingOperationName, null),
                };
            process.InitialArguments.AddRange(initialArguments);
            process.SetLocationAsOwnerContent(InformationContext.CurrentOwner, process.ID);
            return process;
        }

        public static async Task<ProcessContainer> GetTarget_OwnerProcessContainerAsync()
        {
            var processContainer = await ObjectStorage.RetrieveFromOwnerContentA<ProcessContainer>(InformationContext.CurrentOwner, "default");
            if (processContainer == null)
            {
                processContainer = new ProcessContainer();
                processContainer.SetLocationAsOwnerContent(InformationContext.CurrentOwner, "default");
            }
            return processContainer;
        }

        public static async Task ExecuteMethod_AddProcessObjectToContainerAndStoreBothAsync(ProcessContainer ownerProcessContainer, Process process)
        {
            if(ownerProcessContainer.ProcessIDs == null)
                ownerProcessContainer.ProcessIDs = new List<string>();
            ownerProcessContainer.ProcessIDs.Add(process.ID);
            await ownerProcessContainer.StoreInformationAsync();
            await process.StoreInformationAsync();
        }

        public static CreateProcessReturnValue Get_ReturnValue(Process process)
        {
            return new CreateProcessReturnValue {CreatedProcess = process};
        }
    }
}