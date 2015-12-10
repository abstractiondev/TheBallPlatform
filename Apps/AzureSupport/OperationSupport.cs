using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using AaltoGlobalImpact.OIP;
using AzureSupport;
using TheBall.CORE;
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

        public static HttpOperationData GetHttpOperationDataFromRequest(this HttpRequest request, string executorAccountID, string ownerPrefix, string operationName, string operationRequestPath)
        {
            if (operationName.StartsWith("TheBall.Payments"))
                ownerPrefix = "grp/" + InstanceConfiguration.PaymentsGroupID;
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



        public static void ExecuteHttpOperation(HttpOperationData reqData)
        {
            string operationName = reqData.OperationName;
            string parametersTypeName = operationName + "Parameters";
            var operationType = Type.GetType(operationName);
            var parametersType = Type.GetType(parametersTypeName);
            if (operationType == null || parametersType == null)
                throw new InvalidDataException("Operation fully qualified type or parameter type not found in executing assembly: " + operationName);

            var paramObj = PrepareParameters(reqData, parametersType);

            var executeMethod = operationType.GetMethod("Execute");
            executeMethod.Invoke(null, new object[] { paramObj});

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
            if (reqData.RequestContent.Length > 0)
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

        public static string QueueHttpOperation(HttpOperationData reqData)
        {
            var interfaceOperation =
                CreateInterfaceOperationForExecution.Execute(new CreateInterfaceOperationForExecutionParameters
                {
                    DataType = "HTTPREQUEST",
                    OperationData = reqData.ToBytes()
                });
            return interfaceOperation.OperationID;
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
            { "CreateGroupWithTemplates", new Tuple<Type, ParameterManipulator>(typeof(CreateGroupWithTemplates), null)},
            { "InitiateAccountMergeFromEmail", new Tuple<Type, ParameterManipulator>(typeof(InitiateAccountMergeFromEmail), null)},
            { "UnregisterEmailAddress", new Tuple<Type, ParameterManipulator>(typeof(UnregisterEmailAddress), null)},
            { "BeginAccountEmailAddressRegistration", new Tuple<Type, ParameterManipulator>(typeof(BeginAccountEmailAddressRegistration), null)},
            { "CreateInformationInput", new Tuple<Type, ParameterManipulator>(typeof(object), null)},
            { "CreateSpecifiedInformationObjectWithValues", new Tuple<Type, ParameterManipulator>(typeof(CreateSpecifiedInformationObjectWithValues), null)},
            { "DeleteSpecifiedInformationObject", new Tuple<Type, ParameterManipulator>(typeof(DeleteSpecifiedInformationObject), null)},
            // { "", typeof(object)},
        };

        public const string QueueFileNameFormat = "{0:yyyy-MM-dd_HH-mm-ss}_{1}_{2}_{3}";
        public const string LockFileNameFormat = "_{0}.lock";

        public static Type GetLegacyMappedType(string operationLegacyName)
        {
            Tuple<Type, ParameterManipulator> legacyMappedType;
            if(LegacyMappedTypes.TryGetValue(operationLegacyName, out legacyMappedType))
                return legacyMappedType.Item1;
            return null;
        }

        public const string HttpOperationDataType = "HTTPREQUEST";
        public const string OperationQueueLocationName = "OPQueue";

        public static void GetQueueItemComponents(string fileNamePart, out string timestampPart, out string ownerPrefix,
            out string ownerID, out string operationID)
        {
            var split = fileNamePart.Split('_');
            timestampPart = split[0];
            ownerPrefix = split[1];
            ownerID = split[2];
            operationID = split[3];
        }
    }
}