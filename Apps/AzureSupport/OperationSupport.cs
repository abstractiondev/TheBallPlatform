using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using System.Web;
using AaltoGlobalImpact.OIP;
using AzureSupport;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;
using TheBall.CORE.Storage;
using TheBall.Interface;

namespace TheBall
{
    public static class OperationSupport
    {
        public static byte[] ToBytes(this Stream inputStream)
        {
            using (var memStream = new MemoryStream())
            {
                inputStream.CopyTo(memStream);
                return memStream.ToArray();
            }
        }

        public static T ParseJSON<T>(this Stream inputStream)
        {
            return JSONSupport.GetObjectFromStream<T>(inputStream);
        }

        public static HttpOperationData GetOperationDataFromParameters(IContainerOwner owner, string operationName, object jsonParameters)
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

        public static HttpOperationData GetHttpOperationDataFromRequest(this HttpRequest request, string executorAccountID, string ownerPrefix, string operationName, string operationRequestPath)
        {
            if (operationName.StartsWith("TheBall.Payments"))
            {
                ownerPrefix = "grp/" + InstanceConfig.Current.PaymentsGroupID;
            }
            if (operationName.StartsWith("TheBall.CORE") || operationName.StartsWith("TheBall.Admin"))
            {
                var owner = VirtualOwner.FigureOwner(ownerPrefix);
                var isAdminOwner = owner.IsGroupContainer() &&
                                   owner.GetIDFromLocationPrefix() == InstanceConfig.Current.AdminGroupID;
                if(!isAdminOwner)
                    throw new SecurityException("TheBall.CORE or TheBall.Admin operations are only allowed from admin group");
            }
            var fileCollection = request.Files.AllKeys.ToDictionary(key => key, key =>
            {
                var file = request.Files[key];
                return new Tuple<string, byte[]>(file.FileName, file.InputStream.ToBytes());
            });
            Dictionary<string, string> formValues = request.Form.AllKeys.ToDictionary(key => key, key => request.Form[key]);
            Dictionary<string, string> queryParameters = request.Params.AllKeys.ToDictionary(key => key,
                key => request.Params[key]);
            byte[] requestContent = request.InputStream.ToBytes();
            HttpOperationData operationData = new HttpOperationData
            {
                OperationName = operationName,
                ExecutorAccountID = executorAccountID,
                FileCollection = fileCollection,
                FormValues = formValues,
                OwnerRootLocation = ownerPrefix,
                OperationRequestPath = operationRequestPath,
                QueryParameters = queryParameters,
                RequestContent = requestContent
            };
            return operationData;
        }


        public static async Task ExecuteHttpOperationAsync(HttpOperationData reqData)
        {
            string operationName = reqData.OperationName;
            string parametersTypeName = operationName + "Parameters";
            var operationType = Type.GetType(operationName);
            if (operationType == null)
                throw new InvalidDataException("Operation fully qualified type or parameter type not found in executing assembly: " + operationName);
            var executeMethod = operationType.GetMethod("Execute");
            var executeMethodAsync = operationType.GetMethod("ExecuteAsync");
            if (executeMethod == null && executeMethodAsync == null)
                throw new InvalidDataException("Operationg Execute(Async) method is missing - cannot execute: " + operationName);
            var parametersType = Type.GetType(parametersTypeName);
            object[] paramObjs = null;
            if (parametersType != null)
            {
                var preparedParameters = PrepareParameters(reqData, parametersType);
                paramObjs = new object[] { preparedParameters };
            }
            await LogicalOperationContext.SetCurrentContext(reqData, async (initialOwner, executingOwner) =>
            {
                InformationContext.Current.OwnerStack.Push(executingOwner);
                if (initialOwner.IsAccountContainer())
                {
                    string accountID = initialOwner.LocationPrefix;
                    var currAccount = await ObjectStorage.RetrieveFromOwnerContentA<Account>(SystemSupport.SystemOwner, accountID);
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


            //operationType.InvokeMember("Execute", BindingFlags.Public | BindingFlags.Static, null, null,
            //    new object[] { paramObj });

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

        public static async Task<string> QueueHttpOperationAsync(HttpOperationData reqData)
        {
            var operationResult =
                await CreateInterfaceOperationForExecution.ExecuteAsync(new CreateInterfaceOperationForExecutionParameters
                {
                    DataType = "HTTPREQUEST",
                    OperationData = reqData.ToBytes()
                });
            string operationID = operationResult.OperationID;
            await PutInterfaceOperationToQueue.ExecuteAsync(new PutInterfaceOperationToQueueParameters
            {
                OperationID = operationID
            });
            return operationID;
        }


        public static void ExecuteOperation(string operationTypeName, params Tuple<string, object>[] parameterValues)
        {
            string parameterTypeName = operationTypeName + "Parameters";
            Type operationType = TypeSupport.GetTypeByName(operationTypeName);
            if(operationType == null)
                throw new InvalidDataException("Operation type not found: " + operationTypeName);
            Type parameterType = TypeSupport.GetTypeByName(parameterTypeName);
            object[] parametersArg = null;
            if (parameterType != null)
            {
                var parameters = Activator.CreateInstance(parameterType);
                foreach (var par in parameterValues)
                {
                    var fieldName = par.Item1;
                    var fieldValue = par.Item2;
                    var field = parameterType.GetField(par.Item1);
                    if (field == null)
                        throw new InvalidDataException("Parameter invalid field name: " + fieldName);
                    field.SetValue(parameters, fieldValue);
                }
                parametersArg = new object[] {parameters};
            }
            var method = operationType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);
            if (method == null)
                throw new InvalidDataException("Execute method not found in operation class: " + operationTypeName);
            method.Invoke(null, parametersArg);

        }

        private delegate void ParameterManipulator(object paramObj, HttpOperationData operationData);

        private static Dictionary<string, Tuple<Type, ParameterManipulator>> LegacyMappedTypes = 
            new Dictionary<string, Tuple<Type, ParameterManipulator>>()
        {
            { "FetchURLAsGroupContent", new Tuple<Type, ParameterManipulator>(typeof(FetchURLAsGroupContent), null) },
            { "SetGroupAsDefaultForAccount", new Tuple<Type, ParameterManipulator>(typeof(SetGroupAsDefaultForAccount), null) },
            { "ClearDefaultGroupFromAccount", new Tuple<Type, ParameterManipulator>(typeof(ClearDefaultGroupFromAccount), null) },
            { "PublishToConnection", new Tuple<Type, ParameterManipulator>(typeof(PublishCollaborationContentOverConnection), null)},
            { "FinalizeConnectionAfterGroupAuthorization", new Tuple<Type, ParameterManipulator>(typeof(FinalizeConnectionAfterGroupAuthorization), null)},
            { "DeleteConnection", new Tuple<Type, ParameterManipulator>(typeof(DeleteConnectionWithStructures), null)},
            { "SynchronizeConnectionCategories", new Tuple<Type, ParameterManipulator>(typeof(SynchronizeConnectionCategories), null)},
            { "UpdateConnectionThisSideCategories", new Tuple<Type, ParameterManipulator>(typeof(ExecuteConnectionProcess), null)},
            { "InitiateIntegrationConnection", new Tuple<Type, ParameterManipulator>(typeof(InitiateIntegrationConnection), null)},
            { "DeleteCustomUI", new Tuple<Type, ParameterManipulator>(typeof(DeleteCustomUI), null)},
            { "CreateOrUpdateCustomUI", new Tuple<Type, ParameterManipulator>(typeof(CreateOrUpdateCustomUI), null)},
            { "AddCategories", new Tuple<Type, ParameterManipulator>(typeof(CreateSpecifiedInformationObjectWithValues), null)},
            { "PublishGroupToWww", new Tuple<Type, ParameterManipulator>(typeof(PublishGroupToWww), null)},
            { "UpdateUsageMonitoringItems", new Tuple<Type, ParameterManipulator>(typeof(object), null)},
            { "ProcessAllResourceUsagesToOwnerCollections", new Tuple<Type, ParameterManipulator>(typeof(ProcessAllResourceUsagesToOwnerCollections), null)},
            { "CreateInformationOutput", new Tuple<Type, ParameterManipulator>(typeof(object), null)},
            { "DeleteInformationOutput", new Tuple<Type, ParameterManipulator>(typeof(DeleteInformationOutput), null)},
            { "PushToInformationOutput", new Tuple<Type, ParameterManipulator>(typeof(PushToInformationOutput), null)},
            { "DeleteInformationInput", new Tuple<Type, ParameterManipulator>(typeof(DeleteInformationInput), null)},
            { "FetchInputInformation", new Tuple<Type, ParameterManipulator>(typeof(FetchInputInformation), null)},
            { "DeleteDeviceMembership", new Tuple<Type, ParameterManipulator>(typeof(DeleteDeviceMembership), null)},
            { "DeleteAuthenticatedAsActiveDevice", new Tuple<Type, ParameterManipulator>(typeof(DeleteAuthenticatedAsActiveDevice), null)},
            { "PerformNegotiationAndValidateAuthenticationAsActiveDevice", new Tuple<Type, ParameterManipulator>(typeof(PerformNegotiationAndValidateAuthenticationAsActiveDevice), null)},
            { "CreateAuthenticatedAsActiveDevice", new Tuple<Type, ParameterManipulator>(typeof(CreateAuthenticatedAsActiveDevice), null)},
            { "RemoveCollaboratorFromGroup", new Tuple<Type, ParameterManipulator>(typeof(RemoveMemberFromGroup), null)},
            { "InviteMemberToGroupAndPlatform", new Tuple<Type, ParameterManipulator>(typeof(InviteNewMemberToPlatformAndGroup), null)},
            { "InviteMemberToGroup", new Tuple<Type, ParameterManipulator>(typeof(InviteMemberToGroup), null)},
            //{ "CreateGroupWithTemplates", new Tuple<Type, ParameterManipulator>(typeof(CreateGroupWithTemplates), null)},
            { "InitiateAccountMergeFromEmail", new Tuple<Type, ParameterManipulator>(typeof(InitiateAccountMergeFromEmail), null)},
            { "UnregisterEmailAddress", new Tuple<Type, ParameterManipulator>(typeof(UnregisterEmailAddress), null)},
            { "BeginAccountEmailAddressRegistration", new Tuple<Type, ParameterManipulator>(typeof(BeginAccountEmailAddressRegistration), null)},
            { "CreateInformationInput", new Tuple<Type, ParameterManipulator>(typeof(object), null)},
            { "CreateSpecifiedInformationObjectWithValues", new Tuple<Type, ParameterManipulator>(typeof(CreateSpecifiedInformationObjectWithValues), null)},
            { "DeleteSpecifiedInformationObject", new Tuple<Type, ParameterManipulator>(typeof(DeleteSpecifiedInformationObject), null)},
            // { "", typeof(object)},
        };

        public const string LockExtension = ".lock";
        public const string DedicatedLockExtension = ".dedicatedlock";

        public const string QueueFileNameFormat = "{0:yyyy-MM-dd-HH-mm-ss}_{1}_{2}_{3}";
        public const string LockFileNameFormat = "0000_{0}{1}";

        public static Type GetLegacyMappedType(string operationLegacyName)
        {
            Tuple<Type, ParameterManipulator> legacyMappedType;
            if(LegacyMappedTypes.TryGetValue(operationLegacyName, out legacyMappedType))
                return legacyMappedType.Item1;
            return null;
        }

        public const string HttpOperationDataType = "HTTPREQUEST";
        public const string OperationQueueLocationName = "OPQueue";

        public static void GetLockItemComponents(string fileName, out string ownerPrefix, out string ownerID)
        {
            Contract.Assert(fileName.EndsWith(LockExtension) || fileName.EndsWith(DedicatedLockExtension));
            var nameData = fileName.Replace(LockExtension, "").Replace(DedicatedLockExtension, "");
            var split = nameData.Split('_');
            ownerPrefix = split[1];
            ownerID = split[2];
        }

        public static void GetQueueItemComponents(string fileName, out string timestampPart, out string ownerPrefix,
            out string ownerID, out string operationID)
        {
            Contract.Assert(!fileName.EndsWith(LockExtension) && !fileName.EndsWith(DedicatedLockExtension));
            var split = fileName.Split('_');
            timestampPart = split[0];
            ownerPrefix = split[1];
            ownerID = split[2];
            operationID = split[3] + "_" + split[4];
        }
    }
}