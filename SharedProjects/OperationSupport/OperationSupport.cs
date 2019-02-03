using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheBall.CORE.Storage;

namespace TheBall.CORE
{
    public static class OperationSupport
    {

        public static HttpOperationData GetOperationDataFromParameters(IContainerOwner owner, string operationName,
            object jsonParameters)
        {
            byte[] requestContent = null;
            if (jsonParameters != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    JSONSupport.SerializeToJSONStream(jsonParameters, memoryStream);
                    requestContent = memoryStream.ToArray();
                }
            }
            var operationData = new HttpOperationData
            {
                OperationName = operationName,
                OwnerRootLocation = owner.GetOwnerPrefix(),
                RequestContent = requestContent
            };
            return operationData;
        }

        private static object PrepareParameters(HttpOperationData reqData, Type parametersType)
        {
            var paramObj = Activator.CreateInstance(parametersType);
            var parameterFields = parametersType.GetFields();
            var fieldValues = reqData.FormValues;
            if (fieldValues != null)
            {
                foreach (var param in parameterFields)
                {
                    if (param.Name == "Owner")
                    {
                        param.SetValue(paramObj, InformationContext.CurrentOwner);
                        continue;
                    }
                    if (param.Name != "FileCollection")
                    {
                        string fieldValue;
                        string fieldName = param.Name;
                        if (fieldValues.TryGetValue(fieldName, out fieldValue))
                        {
                            param.SetValue(paramObj, fieldValue);
                        }
                    }
                    else
                    {
                        param.SetValue(paramObj, reqData.FileCollection);
                    }
                }
            }
            if (reqData.RequestContent?.Length > 0)
            {
                string parameterTypeNS = parametersType.Namespace;
                string interfaceTypeNS = parameterTypeNS + ".INT";
                var firstInterfaceObjectParam =
                    parameterFields.FirstOrDefault(field => field.FieldType.Namespace == interfaceTypeNS);
                if (firstInterfaceObjectParam != null)
                {
                    object paramValue = JSONSupport.GetObjectFromData(reqData.RequestContent,
                        firstInterfaceObjectParam.FieldType);
                    firstInterfaceObjectParam.SetValue(paramObj, paramValue);
                }
            }
            return paramObj;
        }


        public static async Task ExecuteHttpOperationAsync(HttpOperationData reqData)
        {
            string operationName = reqData.OperationName;
            string parametersTypeName = operationName + "Parameters";
            var operationType = Type.GetType(operationName);
            if (operationType == null)
                throw new InvalidDataException(
                    "Operation fully qualified type or parameter type not found in executing assembly: " + operationName);
            var executeMethod = operationType.GetMethod("Execute");
            var executeMethodAsync = operationType.GetMethod("ExecuteAsync");
            if (executeMethod == null && executeMethodAsync == null)
                throw new InvalidDataException("Operationg Execute(Async) method is missing - cannot execute: " +
                                               operationName);
            var parametersType = Type.GetType(parametersTypeName);
            object[] paramObjs = null;
            if (parametersType != null)
            {
                var preparedParameters = PrepareParameters(reqData, parametersType);
                paramObjs = new object[] {preparedParameters};
            }
            await LogicalOperationContext.SetCurrentContext(reqData, async (initialOwner, executingOwner) =>
            {
                InformationContext.Current.OwnerStack.Push(executingOwner);
#if notsupported_devmodefocus
                if (initialOwner.IsAccountContainer())
                {
                    string accountID = initialOwner.LocationPrefix;
                    var currAccount = await ObjectStorage.RetrieveFromOwnerContentA<Account>(SystemSupport.SystemOwner,
                        accountID);
                    string accountEmailID = currAccount.Emails.FirstOrDefault();
                    string accountEmail;
                    if (accountEmailID == null)
                        accountEmail = "";
                    else
                        accountEmail = Email.GetEmailAddressFromID(accountEmailID);
                    var accountName = accountEmail;
                    InformationContext.Current.Account = new CoreAccountData(accountID,
                        accountName, accountEmail);
                }
#endif
            });
            try
            {
                if (executeMethodAsync != null)
                {
                    Task awaitable = (Task) executeMethodAsync.Invoke(null, paramObjs);
                    await awaitable;
                }
                else
                {
                    executeMethod.Invoke(null, paramObjs);
                }
                await InformationContext.Current.LogicalOperationContext.ExecuteRegisteredFinalizingActions();
            }
            finally
            {
                await LogicalOperationContext.ReleaseCurrentContext(async (initialOwner, executingOwner) =>
                {
                    InformationContext.Current.OwnerStack.Pop();
                });
            }

        }
    }
}