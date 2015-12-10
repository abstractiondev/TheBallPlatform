using TheBall.CORE;

namespace TheBall.Interface
{
    public class PutInterfaceOperationToQueueImplementation
    {
        public static IContainerOwner GetTarget_QueueOwner()
        {
            throw new System.NotImplementedException();
        }

        public static string GetTarget_QueueLocation()
        {
            throw new System.NotImplementedException();
        }

        public static IContainerOwner GetTarget_OperationOwner()
        {
            throw new System.NotImplementedException();
        }

        public static string GetTarget_QueueItemFileNameFormat()
        {
            throw new System.NotImplementedException();
        }

        public static string GetTarget_QueueItemFullPath(string operationID, string queueItemFileNameFormat, IContainerOwner queueOwner, string queueLocation, IContainerOwner operationOwner)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_CreateQueueEntry(string operationID, string queueItemFullPath)
        {
            throw new System.NotImplementedException();
        }
    }
}