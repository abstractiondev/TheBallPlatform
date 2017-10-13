using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class ProcessAllResourceUsagesToOwnerCollectionsImplementation
    {
        public static async Task ExecuteMethod_ExecuteBatchProcessorAsync(int processBatchSize)
        {
            bool continueProcessing;
            do
            {
                var processResult =
                    await ProcessBatchOfResourceUsagesToOwnerCollections.ExecuteAsync(new ProcessBatchOfResourceUsagesToOwnerCollectionsParameters
                        {
                            ProcessBatchSize = processBatchSize,
                            ProcessIfLess = false
                        });
                continueProcessing = processResult.ProcessedAnything && processResult.ProcessedFullCount;
            } while (continueProcessing);
        }
    }
}