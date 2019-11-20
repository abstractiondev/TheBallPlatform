using System.Collections.Specialized;
using AzureSupport;
using TheBall.Core;

namespace TheBall.Interface
{
    public class ExecuteLegacyHttpPostRequestImplementation
    {
        public static HttpOperationData GetTarget_RequestData()
        {
            return LogicalOperationContext.Current.HttpParameters;
        }

        public static void ExecuteMethod_ExecutePostRequest(HttpOperationData requestData)
        {
            NameValueCollection nvCollection = new NameValueCollection();
            var formValues = requestData.FormValues;
            foreach (var key in formValues.Keys)
            {
                nvCollection.Add(key, formValues[key]);
            }

            var operationResult = ModifyInformationSupport.ExecuteOwnerWebPOST(InformationContext.CurrentOwner,
                nvCollection, null);
        }
    }
}