using System.Threading.Tasks;

namespace TheBall.Core
{
    public class DeleteProcessImplementation
    {
        public static async Task<Process> GetTarget_ProcessAsync(string processId)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<Process>(InformationContext.CurrentOwner, processId);
        }

        public static async Task<ProcessContainer> GetTarget_OwnerProcessContainerAsync()
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<ProcessContainer>(InformationContext.CurrentOwner, "default");
        }

        public static async Task ExecuteMethod_ObtainLockRemoveFromContainerAndDeleteProcessAsync(string processID, Process process, ProcessContainer ownerProcessContainer)
        {
            if (process == null)
            {
                if (ownerProcessContainer?.ProcessIDs != null)
                {
                    ownerProcessContainer.ProcessIDs.Remove(processID);
                    await ownerProcessContainer.StoreInformationAsync();
                }
            }
            else
            {
                string lockEtag = await process.ObtainLockOnObject();
                if (lockEtag == null)
                    return;
                try
                {
                    if (ownerProcessContainer != null)
                    {
                        ownerProcessContainer.ProcessIDs.Remove(process.ID);
                        await ownerProcessContainer.StoreInformationAsync();
                    }
                    await process.DeleteInformationObjectAsync();
                }
                finally
                {
                    await process.ReleaseLockOnObjectAsync(lockEtag);
                }
            }

        }
    }
}