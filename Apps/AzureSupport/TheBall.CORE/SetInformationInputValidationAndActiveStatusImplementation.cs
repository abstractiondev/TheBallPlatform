using System.Threading.Tasks;

namespace TheBall.Core
{
    public class SetInformationInputValidationAndActiveStatusImplementation
    {
        public static async Task<InformationInput> GetTarget_InformationInputAsync(IContainerOwner owner, string informationInputId)
        {
            return await ObjectStorage.RetrieveFromDefaultLocationA<InformationInput>(informationInputId, owner);
        }

        public static void ExecuteMethod_SetInputValidAndActiveValue(bool isValidAndActive, InformationInput informationInput)
        {
            informationInput.IsValidatedAndActive = isValidAndActive;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(InformationInput informationInput)
        {
            await informationInput.StoreInformationAsync();
        }
    }
}