using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace WebInterface
{
    public class NotFoundJSONDependencyFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        // You can pass values from .config
        //public string MyParamFromConfigFile { get; set; }

        public void Process(ITelemetry item)
        {
            if (!OKToSend(item))
                return;
            ModifyItem(item);
            this.Next.Process(item);
        }

        private void ModifyItem(ITelemetry item)
        {
            //throw new System.NotImplementedException();
        }

        private bool OKToSend(ITelemetry item)
        {
            var dependency = item as DependencyTelemetry;
            if (dependency == null) return true;

            bool isAzureBlob = dependency.Type == "Azure blob";
            bool isMissing = dependency.ResultCode == "404";
            bool isJson = dependency.Data.EndsWith(".json");
            bool filterOut = isMissing && isJson && isAzureBlob;
            return !filterOut;
        }

        public NotFoundJSONDependencyFilter(ITelemetryProcessor next)
        {
            this.Next = next;
        }
    }
}