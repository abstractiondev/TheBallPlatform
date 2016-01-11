using AzureSupport;

namespace TheBall
{
    public class LogicalOperationContext
    {
        public static LogicalOperationContext Current => InformationContext.Current.LogicalOperationContext;

        public readonly HttpOperationData HttpParameters;

        public static void SetCurrentContext(HttpOperationData httpOperationData)
        {
            InformationContext.Current.LogicalOperationContext = new LogicalOperationContext(httpOperationData);
        }

        public static void ReleaseCurrentContext()
        {
            InformationContext.Current.LogicalOperationContext = null;
        }

        private LogicalOperationContext(HttpOperationData httpOperationData = null)
        {
            HttpParameters = httpOperationData;
        }
    }
}