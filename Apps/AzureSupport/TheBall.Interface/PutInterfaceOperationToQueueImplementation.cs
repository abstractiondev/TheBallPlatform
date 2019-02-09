using System;
using System.IO;
using System.Threading.Tasks;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class PutInterfaceOperationToQueueImplementation
    {
        public static IContainerOwner GetTarget_QueueOwner()
        {
            return SystemSupport.SystemOwner;
        }

        public static string GetTarget_QueueLocation()
        {
            return OperationSupport.OperationQueueLocationName;
        }

        public static IContainerOwner GetTarget_OperationOwner()
        {
            return InformationContext.CurrentOwner;
        }

        public static string GetTarget_QueueItemFileNameFormat()
        {
            return OperationSupport.QueueFileNameFormat;
        }

        public static string GetTarget_QueueItemFullPath(string operationID, string queueItemFileNameFormat, IContainerOwner queueOwner, string queueLocation, IContainerOwner operationOwner)
        {
            DateTime timestamp = DateTime.UtcNow;
            var queueItemFileName = String.Format(queueItemFileNameFormat, timestamp, operationOwner.ContainerName, operationOwner.LocationPrefix, operationID);
            var fullPath = Path.Combine(queueOwner.GetOwnerPrefix(), queueLocation, queueItemFileName)
                .Replace(@"\", "/");
            return fullPath;
        }

        public static IAccountInfo GetTarget_InvokerAccount()
        {
            return InformationContext.CurrentAccount;
        }

        public static async Task ExecuteMethod_CreateQueueEntryAsync(string operationID, string queueItemFullPath, IAccountInfo invokerAccount)
        {
            string content = String.Join(Environment.NewLine,
                new string[]
                {operationID, invokerAccount?.AccountID, invokerAccount?.AccountEmail, invokerAccount?.AccountName});
            await StorageSupport.CurrActiveContainer.UploadBlobTextAsync(queueItemFullPath, content);
        }
    }
}