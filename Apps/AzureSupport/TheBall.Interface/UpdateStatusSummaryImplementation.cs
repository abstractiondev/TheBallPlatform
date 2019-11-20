using System;
using System.Linq;
using System.Threading.Tasks;
using TheBall.Core;

namespace TheBall.Interface
{
    public class UpdateStatusSummaryImplementation
    {
        public static async Task ExecuteMethod_EnsureUpdateOnStatusSummaryAsync(IContainerOwner owner, DateTime updateTime, string[] changedIDList, int removeExpiredEntriesSeconds)
        {
            int retryCount = 10;
            while (retryCount-- >= 0)
            {
                try
                {
                    var statusSummary = await ObjectStorage.RetrieveFromOwnerContentA<StatusSummary>(owner, "default");
                    if (statusSummary == null)
                    {
                        statusSummary = new StatusSummary();
                        statusSummary.SetLocationAsOwnerContent(owner, "default");
                    }
                    string latestTimestampEntry = statusSummary.ChangeItemTrackingList.FirstOrDefault();
                    long currentTimestampTicks = updateTime.ToUniversalTime().Ticks;
                    if (latestTimestampEntry != null)
                    {
                        long latestTimestampTicks = Convert.ToInt64(latestTimestampEntry.Substring(2));
                        if (currentTimestampTicks <= latestTimestampTicks)
                            currentTimestampTicks = latestTimestampTicks + 1;
                    }
                    string currentTimestampEntry = "T:" + currentTimestampTicks;
                    var timestampedList = statusSummary.ChangeItemTrackingList;
                    // Remove possible older entries of new IDs
                    timestampedList.RemoveAll(changedIDList.Contains);
                    // Add Timestamp and new IDs
                    timestampedList.Insert(0, currentTimestampEntry);
                    timestampedList.InsertRange(1, changedIDList);
                    var removeOlderThanTicks = currentTimestampTicks -
                                               TimeSpan.FromSeconds(removeExpiredEntriesSeconds).Ticks;
                    int firstBlockToRemoveIX = timestampedList.FindIndex(candidate =>
                    {
                        if (candidate.StartsWith("T:"))
                        {
                            long candidateTicks = Convert.ToInt64(candidate.Substring(2));
                            return candidateTicks < removeOlderThanTicks;
                        }
                        return false;
                    });
                    if (firstBlockToRemoveIX > -1)
                    {
                        timestampedList.RemoveRange(firstBlockToRemoveIX, timestampedList.Count - firstBlockToRemoveIX);
                    }
                    await statusSummary.StoreInformationAsync();
                    return; // Break from while loop
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}