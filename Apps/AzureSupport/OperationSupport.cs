using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

        public static void ExecuteHttpOperation(HttpOperationData reqData)
        {
            string operationName = reqData.OperationName;
            string parametersTypeName = operationName + "Parameters";
            var operationType = Type.GetType(operationName);
            var parametersType = Type.GetType(parametersTypeName);
            if (operationType == null || parametersType == null)
                throw new InvalidDataException("Operation fully qualified type or parameter type not found in executing assembly: " + operationName);

            var paramObj = PrepareParameters(reqData, parametersType);

            operationType.InvokeMember("Execute", BindingFlags.Public | BindingFlags.Static, null, null,
                new object[] { paramObj });

        }

        private static object PrepareParameters(HttpOperationData reqData, Type parametersType)
        {
            var paramObj = Activator.CreateInstance(parametersType);
            var parameterFields = parametersType.GetFields(BindingFlags.Public);
            var fieldValues = reqData.FormValues;
            if (fieldValues != null)
            {
                foreach (var param in parameterFields)
                {
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
            string parameterTypeName = operationTypeName + "QueryParameters";
            Type operationType = TypeSupport.GetTypeByName(operationTypeName);
            if(operationType == null)
                throw new InvalidDataException("Operation type not found: " + operationTypeName);
            Type parameterType = TypeSupport.GetTypeByName(parameterTypeName);
            if(parameterType == null)
                throw new InvalidDataException("Operation parameter type not found: " + parameterTypeName);
            var parameters = Activator.CreateInstance(parameterType);
            foreach (var par in parameterValues)
            {
                var fieldName = par.Item1;
                var fieldValue = par.Item2;
                var field = parameterType.GetField(par.Item1);
                if(field == null)
                    throw new InvalidDataException("Parameter invalid field name: " + fieldName);
                field.SetValue(parameters, fieldValue);
            }
            var method = operationType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);
            if(method == null)
                throw new InvalidDataException("Execute method not found in operation class: " + operationTypeName);
            method.Invoke(null, new object[] { parameters });
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

        public static Type GetLegacyMappedType(string operationLegacyName)
        {
            Tuple<Type, ParameterManipulator> legacyMappedType;
            if(LegacyMappedTypes.TryGetValue(operationLegacyName, out legacyMappedType))
                return legacyMappedType.Item1;
            return null;
        }
    }
}