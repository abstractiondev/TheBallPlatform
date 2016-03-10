using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using AzureSupport;
using TheBall;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;
using TheBall.Interface;

namespace WebInterface
{
    public class GenericWebhookHandler : HttpTaskAsyncHandler
    {
        /*
        internal class StripeEventData
        {
            public string id { get; set; }
            public bool livemode { get; set; }
        }

        public override Task ProcessRequestAsync(HttpContext context)
        {
            var request = context.Request;
            var stripeEventData = request.InputStream.ParseJSON<Stripe.StripeEventData>();
            
        }
         * */

        public override bool IsReusable
        {
            get { return true; }
        }

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            var webhookComponents = context.Request.Path.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            string webhookName = webhookComponents.Length > 1 ?  webhookComponents[1] : String.Empty;
            if (InstanceConfig.Current.WebhookHandlersDict.ContainsKey(webhookName))
            {
                var handlerInfo = InstanceConfig.Current.WebhookHandlersDict[webhookName];
                string operationFullName = handlerInfo.Item1;
                string handlerOwningGroup = handlerInfo.Item2;
                await new OperationWebhookHandler().ProcessRequest(context, operationFullName, handlerOwningGroup);
            }
        }
    }

    public abstract class WebhookHandler
    {
        public abstract Task ProcessRequest(HttpContext context, string operationFullName, string handlerOwningGroup);
    }

    public class OperationWebhookHandler : WebhookHandler
    {
        public override async Task ProcessRequest(HttpContext context, string operationFullName, string handlerOwningGroup)
        {
            var request = context.Request;
            string owningGroupPrefix = string.Format("grp/{0}", handlerOwningGroup);
            var owner = VirtualOwner.FigureOwner(owningGroupPrefix);
            await InformationContext.ExecuteAsOwnerAsync(owner, async () =>
            {
                var operationData = request.GetHttpOperationDataFromRequest(null, owningGroupPrefix,
                    operationFullName, request.Path);
                var operationResult = await 
                    CreateInterfaceOperationForExecution.ExecuteAsync(new CreateInterfaceOperationForExecutionParameters
                    {
                        DataType = OperationSupport.HttpOperationDataType,
                        OperationData = operationData.ToBytes()
                    });
                await ExecuteInterfaceOperation.ExecuteAsync(new ExecuteInterfaceOperationParameters
                {
                    OperationID = operationResult.OperationID
                });
            });
            context.Response.StatusCode = 200;
        }
    }

}