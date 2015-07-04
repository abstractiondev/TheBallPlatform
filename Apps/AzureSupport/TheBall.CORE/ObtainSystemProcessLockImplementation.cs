using System;

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

        public static string ExecuteMethod_ObtainOwnerLevelLock(IContainerOwner owner, string ownerLockFileName, string lockFileContent)
        {
            string claimedLockID = StorageSupport.TryClaimLockForOwner(owner, ownerLockFileName, lockFileContent);
            return claimedLockID;
        }

        public static void ExecuteMethod_ReportSystemLockToMatchOwnerLock(string obtainOwnerLevelLockOutput, string systemOwnerLockFileName, string lockFileContent)
        {
            if (obtainOwnerLevelLockOutput == null)
                return;
            var systemOwner = SystemSupport.SystemOwner;
            StorageSupport.ReplicateClaimedLock(systemOwner, systemOwnerLockFileName, lockFileContent);
        }

        public static ObtainSystemProcessLockReturnValue Get_ReturnValue(string obtainOwnerLevelLockOutput)
        {
            return new ObtainSystemProcessLockReturnValue { ObtainedLockID = obtainOwnerLevelLockOutput};
        }
    }
}