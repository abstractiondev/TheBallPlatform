using System.Threading.Tasks;

namespace TheBall.Core
{
    public class SetInformationOutputValidationAndActiveStatusImplementation
    {
        public static async Task<InformationOutput> GetTarget_InformationOutputAsync(IContainerOwner owner, string informationOutputId)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<InformationOutput>(owner, informationOutputId);
        }

        public static void ExecuteMethod_SetInputValidAndActiveValue(bool isValidAndActive, InformationOutput informationOutput)
        {
            informationOutput.IsValidatedAndActive = isValidAndActive;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(InformationOutput informationOutput)
        {
            await informationOutput.StoreInformationAsync();
        }
    }
}