using System.Security;
using System.Threading.Tasks;

namespace TheBall.Core
{
    public class ProcessFetchedInputsImplementation
    {
        public static async Task<InformationInput> GetTarget_InformationInputAsync(IContainerOwner owner, string informationInputId)
        {
            return await ObjectStorage.RetrieveFromDefaultLocationA<InformationInput>(informationInputId, owner);
        }

        public static void ExecuteMethod_VerifyValidInput(InformationInput informationInput)
        {
            if (informationInput.IsValidatedAndActive == false)
                throw new SecurityException("InformationInput is not active");
        }

        public static string GetTarget_InputFetchLocation(InformationInput informationInput)
        {
            return informationInput.RelativeLocation + "_Input";
        }

        public static ProcessFetchedInputs.ProcessInputFromStorageReturnValue ExecuteMethod_ProcessInputFromStorage(string processingOperationName, InformationInput informationInput, string inputFetchLocation)
        {
            var result = new ProcessFetchedInputs.ProcessInputFromStorageReturnValue();
            // TODO: Processing
            return result;
        }

        public static async Task ExecuteMethod_StoreObjectsAsync(IInformationObject[] processingResultsToStore)
        {
            if (processingResultsToStore == null)
                return;
            foreach (var iObj in processingResultsToStore)
                await iObj.StoreInformationAsync();
        }

        public static async Task ExecuteMethod_DeleteObjectsAsync(IInformationObject[] processingResultsToDelete)
        {
            if (processingResultsToDelete == null)
                return;
            foreach(var iObj in processingResultsToDelete)
                await iObj.DeleteInformationObjectAsync();
        }
    }
}