using System;

namespace TheBall.CORE
{
    public class ReleaseSystemProcessLockImplementation
    {
        private const string TimeStampConstant = "yyyyMMdd_HHmmss";

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

        public static void ExecuteMethod_ReleaseOwnedOwnerLevelLock(IContainerOwner owner, string lockID, string ownerLockFileName)
        {
            StorageSupport.ReleaseLockForOwner(owner, ownerLockFileName, lockID);
        }

        public static void ExecuteMethod_ReleaseReportingSystemLock(string systemOwnerLockFileName)
        {
            var owner = SystemSupport.SystemOwner;
            StorageSupport.ReleaseLockForOwner(owner, systemOwnerLockFileName);
        }
    }
}