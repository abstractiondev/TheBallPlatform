namespace TheBall.Interface
{
    public class ExecuteInterfaceOperationImplementation
    {
        public static InterfaceOperation GetTarget_OperationData(string operationID)
        {
            return InterfaceOperation.RetrieveFromOwnerContent(InformationContext.CurrentOwner, operationID);
        }

        public static void ExecuteMethod_ExecuteOperation(InterfaceOperation operationData)
        {
            throw new System.NotImplementedException();
        }
    }
}