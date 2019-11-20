using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.Core.Storage;

namespace TheBall.Core
{
    public class UpdateUsageMonitoringSummariesImplementation
    {
        public static async Task<UsageMonitorItem[]> GetTarget_SourceItemsAsync(IContainerOwner owner, int amountOfDays)
        {
            string filterPrefix = "TheBall.Core/UsageMonitorItem/";
            DateTime today = DateTime.UtcNow.Date;
            List<UsageMonitorItem> result = new List<UsageMonitorItem>();
            Type type = typeof (UsageMonitorItem);
            for (DateTime fromDate = today.AddDays(-amountOfDays); fromDate <= today; fromDate = fromDate.AddDays(1))
            {
                string dateStr = fromDate.ToString("yyyyMMdd");
                var dayBlobs = await BlobStorage.GetBlobItemsA(owner, filterPrefix + dateStr);
                foreach (var blob in dayBlobs)
                {
                    UsageMonitorItem item = (UsageMonitorItem) await StorageSupport.RetrieveInformationA(blob.Name, type);
                    result.Add(item);
                }
            }
            return result.ToArray();
        }

        public static async Task ExecuteMethod_CreateUsageMonitoringSummariesAsync(IContainerOwner owner, int amountOfDays, UsageMonitorItem[] sourceItems)
        {
            var groupedByDay =
                sourceItems.OrderBy(item => item.RelativeLocation)
                           .GroupBy(item => item.TimeRangeInclusiveStartExclusiveEnd.StartTime.Date);
            if(amountOfDays < 31)
                throw new ArgumentException("Amount of days needs to be at least 31 so that last month makes sense with data");
            // Last 7 days
            UsageSummary lastWeekHourlySummary = null;
            DateTime today = DateTime.UtcNow.Date;
            DateTime weekAgoStartDay = today.AddDays(-7);
            DateTime todayEndTime = today.AddDays(1);
            lastWeekHourlySummary = new UsageSummary
                {
                    SummaryName = "Last Week (7 days) Summary",
                    SummaryMonitoringItem = UsageMonitorItem.Create(weekAgoStartDay, todayEndTime, 60)
                };
            lastWeekHourlySummary.SetLocationAsOwnerContent(owner, "LastWeekSummary_Hourly");

            // Last month
            DateTime monthAgoStartDay = today.AddMonths(-1);
            UsageSummary lastMonthDailySummary = new UsageSummary
                {
                    SummaryName = "Last Week Summary",
                    SummaryMonitoringItem = UsageMonitorItem.Create(monthAgoStartDay, todayEndTime, 60 * 24)
                };
            lastMonthDailySummary.SetLocationAsOwnerContent(owner, "LastMonthSummary_Daily");

            foreach (var dayList in groupedByDay)
            {
                //UsageMonitorItemCollection hourlyCollection = new UsageMonitorItemCollection();
                //hourlyCollection.
                string hourlySummaryName = "Hourly Summary of " + dayList.Key.ToShortDateString();
                DateTime startTime = dayList.Key;
                DateTime endTime = startTime.AddDays(1);
                var currDaysData = dayList.ToArray();
                UsageSummary dailyHourlySummary = new UsageSummary
                    {
                        SummaryName = hourlySummaryName,
                        SummaryMonitoringItem = UsageMonitorItem.Create(startTime, endTime, 60)
                    };
                dailyHourlySummary.SummaryMonitoringItem.AggregateValuesFrom(currDaysData);
                string prefixName = startTime.ToString("yyyyMMdd");
                dailyHourlySummary.SetLocationAsOwnerContent(owner, prefixName + "_Hourly");
                await dailyHourlySummary.StoreInformationAsync(null, true);
                string detailedSummaryName = "Detailed (5 min) Summary of " + dayList.Key.ToShortDateString();
                UsageSummary dailyDetailedSummary = new UsageSummary
                    {
                        SummaryName = detailedSummaryName,
                        SummaryMonitoringItem = UsageMonitorItem.Create(startTime, endTime, 5)
                    };
                dailyDetailedSummary.SummaryMonitoringItem.AggregateValuesFrom(currDaysData);
                dailyDetailedSummary.SetLocationAsOwnerContent(owner, prefixName + "_Detailed");
                await dailyDetailedSummary.StoreInformationAsync(null, true);

                // Weekly summary
                if (startTime >=
                    lastWeekHourlySummary.SummaryMonitoringItem.TimeRangeInclusiveStartExclusiveEnd.StartTime)
                {
                    lastWeekHourlySummary.SummaryMonitoringItem.AggregateValuesFrom(currDaysData);
                }
                if (startTime >=
                    lastMonthDailySummary.SummaryMonitoringItem.TimeRangeInclusiveStartExclusiveEnd.StartTime)
                {
                    lastMonthDailySummary.SummaryMonitoringItem.AggregateValuesFrom(currDaysData);
                }
            }

            await lastWeekHourlySummary.StoreInformationAsync(null, true);
            await lastMonthDailySummary.StoreInformationAsync(null, true);
        }
    }
}