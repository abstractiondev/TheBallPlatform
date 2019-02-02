using System;
using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class ObtainSystemProcessLockImplementation
    {
        private const string TimeStampConstant = "yyyyMMdd_HHmmss";

        public static string GetTarget_LockFileContent(DateTime latestEntryTime, int amountOfEntries)
        {
            string entryTimeText = latestEntryTime.ToString(TimeStampConstant) + ":" + amountOfEntries.ToString();
            return entryTimeText;
        }

        public static string GetTarget_OwnerLockFileName()
        {
            return "Processing.lock";
        }

        public static string GetTarget_SystemOwnerLockFileName(IContainerOwner owner, DateTime latestEntryTime)
        {
            string entryTimePrefix = latestEntryTime.ToString(TimeStampConstant);
            string fileName = String.Format("{0}_{1}_{2}.lock", entryTimePrefix, owner.ContainerName, owner.LocationPrefix);
            return fileName;
        }

        public static async Task<string> ExecuteMethod_ObtainOwnerLevelLockAsync(IContainerOwner owner, string ownerLockFileName, string lockFileContent)
        {
            string claimedLockID = await StorageSupport.TryClaimLockForOwnerAsync(owner, ownerLockFileName, lockFileContent);
            return claimedLockID;
        }

        public static async Task ExecuteMethod_ReportSystemLockToMatchOwnerLockAsync(string obtainOwnerLevelLockOutput, string systemOwnerLockFileName, string lockFileContent)
        {
            if (obtainOwnerLevelLockOutput == null)
                return;
            var systemOwner = SystemSupport.SystemOwner;
            await StorageSupport.ReplicateClaimedLockAsync(systemOwner, systemOwnerLockFileName, lockFileContent);
        }

        public static ObtainSystemProcessLockReturnValue Get_ReturnValue(string obtainOwnerLevelLockOutput)
        {
            return new ObtainSystemProcessLockReturnValue { ObtainedLockID = obtainOwnerLevelLockOutput};
        }
    }
}