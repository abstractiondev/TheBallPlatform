using System.Threading.Tasks;

namespace TheBall.Core
{
    public class DeleteInformationInputImplementation
    {
        public static async Task<InformationInput> GetTarget_InformationInputAsync(IContainerOwner owner, string informationInputId)
        {
            return await ObjectStorage.RetrieveFromDefaultLocationA<InformationInput>(informationInputId, owner);
        }

        public static async Task ExecuteMethod_DeleteInformationInputAsync(InformationInput informationInput)
        {
            await informationInput.DeleteInformationObjectA();
        }
    }
}