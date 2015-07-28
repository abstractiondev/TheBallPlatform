namespace TheBall.CORE
{
    public class DeleteInformationInputImplementation
    {
        public static InformationInput GetTarget_InformationInput(IContainerOwner owner, string informationInputId)
        {
            return ObjectStorage.RetrieveFromDefaultLocation<InformationInput>(informationInputId, owner);
        }

        public static void ExecuteMethod_DeleteInformationInput(InformationInput informationInput)
        {
            informationInput.DeleteInformationObject();
        }
    }
}