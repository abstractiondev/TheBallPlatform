using System;
using System.Threading.Tasks;
using TheBall.Core.StorageCore;

namespace TheBall.Core
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
            var storageService = CoreServices.GetCurrent<IStorageService>();
            string claimedLockID = await storageService.TryClaimLockForOwnerAsync(owner, ownerLockFileName, lockFileContent);
            return claimedLockID;
        }

        public static async Task ExecuteMethod_ReportSystemLockToMatchOwnerLockAsync(string obtainOwnerLevelLockOutput, string systemOwnerLockFileName, string lockFileContent)
        {
            if (obtainOwnerLevelLockOutput == null)
                return;
            var storageService = CoreServices.GetCurrent<IStorageService>();
            var systemOwner = SystemSupport.SystemOwner;
            await storageService.ReplicateClaimedLockAsync(systemOwner, systemOwnerLockFileName, lockFileContent);
        }

        public static ObtainSystemProcessLockReturnValue Get_ReturnValue(string obtainOwnerLevelLockOutput)
        {
            return new ObtainSystemProcessLockReturnValue { ObtainedLockID = obtainOwnerLevelLockOutput};
        }
    }
}