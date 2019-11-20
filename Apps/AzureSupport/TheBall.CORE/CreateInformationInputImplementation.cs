using System.Threading.Tasks;

namespace TheBall.Core
{
    public class CreateInformationInputImplementation
    {
        public static InformationInput GetTarget_CreatedInformationInput(IContainerOwner owner, string inputDescription, string locationUrl, string localContentName, string authenticatedDeviceId)
        {
            InformationInput informationInput = new InformationInput();
            informationInput.SetLocationAsOwnerContent(owner, informationInput.ID);
            informationInput.InputDescription = inputDescription;
            informationInput.LocationURL = locationUrl;
            informationInput.LocalContentName = localContentName;
            informationInput.AuthenticatedDeviceID = authenticatedDeviceId;
            return informationInput;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(InformationInput createdInformationInput)
        {
            await createdInformationInput.StoreInformationAsync();
        }

        public static CreateInformationInputReturnValue Get_ReturnValue(InformationInput createdInformationInput)
        {
            return new CreateInformationInputReturnValue {InformationInput = createdInformationInput};
            
        }
    }
}