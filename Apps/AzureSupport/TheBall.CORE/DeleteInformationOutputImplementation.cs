using System.Threading.Tasks;

namespace TheBall.Core
{
    public class DeleteInformationOutputImplementation
    {
        public static async Task<InformationOutput> GetTarget_InformationOutputAsync(IContainerOwner owner, string informationOutputId)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<InformationOutput>(owner, informationOutputId);
        }

        public static async Task ExecuteMethod_DeleteInformationOutputAsync(InformationOutput informationOutput)
        {
            await informationOutput.DeleteInformationObjectAsync();
        }
    }
}