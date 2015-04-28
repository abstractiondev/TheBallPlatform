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

        public static void ExecuteHttpOperation(string operationName, HttpOperationData reqData)
        {
            string parametersTypeName = operationName + "Parameters";
            var operationType = Type.GetType(operationName);
            var parametersType = Type.GetType(parametersTypeName);
            if (operationType == null || parametersType == null)
                throw new InvalidDataException("Operation fully qualified type or parameter type not found in executing assembly: " + operationName);

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

            operationType.InvokeMember("Execute", BindingFlags.Public | BindingFlags.Static, null, null,
                new object[] { paramObj });

        }

        public static string QueueHttpOperation(string operationName, HttpOperationData reqData)
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

        private static Dictionary<string, Type> LegacyMappedTypes = new Dictionary<string, Type>()
        {
            { "FetchURLAsGroupContent", typeof(FetchURLAsGroupContent)},
            { "SetGroupAsDefaultForAccount", typeof(SetGroupAsDefaultForAccount)},
            { "ClearDefaultGroupFromAccount", typeof(ClearDefaultGroupFromAccount)},
            { "PublishToConnection", typeof(PublishCollaborationContentOverConnection)},
            { "FinalizeConnectionAfterGroupAuthorization", typeof(FinalizeConnectionAfterGroupAuthorization)},
            { "DeleteConnection", typeof(DeleteConnectionWithStructures)},
            { "SynchronizeConnectionCategories", typeof(SynchronizeConnectionCategories)},
            { "UpdateConnectionThisSideCategories", typeof(ExecuteConnectionProcess)},
            { "InitiateIntegrationConnection", typeof(InitiateIntegrationConnection)},
            { "DeleteCustomUI", typeof(DeleteCustomUI)},
            { "CreateOrUpdateCustomUI", typeof(CreateOrUpdateCustomUI)},
            { "AddCategories", typeof(CreateSpecifiedInformationObjectWithValues)},
            { "PublishGroupToWww", typeof(PublishGroupToWww)},
            { "UpdateUsageMonitoringItems", typeof(object)},
            { "ProcessAllResourceUsagesToOwnerCollections", typeof(ProcessAllResourceUsagesToOwnerCollections)},
            { "CreateInformationOutput", typeof(object)},
            { "DeleteInformationOutput", typeof(DeleteInformationOutput)},
            { "PushToInformationOutput", typeof(PushToInformationOutput)},
            { "DeleteInformationInput", typeof(DeleteInformationInput)},
            { "FetchInputInformation", typeof(FetchInputInformation)},
            { "DeleteDeviceMembership", typeof(DeleteDeviceMembership)},
            { "DeleteAuthenticatedAsActiveDevice", typeof(DeleteAuthenticatedAsActiveDevice)},
            { "PerformNegotiationAndValidateAuthenticationAsActiveDevice", typeof(PerformNegotiationAndValidateAuthenticationAsActiveDevice)},
            { "CreateAuthenticatedAsActiveDevice", typeof(CreateAuthenticatedAsActiveDevice)},
            { "RemoveCollaboratorFromGroup", typeof(RemoveMemberFromGroup)},
            { "InviteMemberToGroupAndPlatform", typeof(InviteNewMemberToPlatformAndGroup)},
            { "InviteMemberToGroup", typeof(InviteMemberToGroup)},
            { "CreateGroupWithTemplates", typeof(CreateGroupWithTemplates)},
            { "InitiateAccountMergeFromEmail", typeof(InitiateAccountMergeFromEmail)},
            { "UnregisterEmailAddress", typeof(UnregisterEmailAddress)},
            { "BeginAccountEmailAddressRegistration", typeof(BeginAccountEmailAddressRegistration)},
            { "CreateInformationInput", typeof(object)},
            { "CreateSpecifiedInformationObjectWithValues", typeof(CreateSpecifiedInformationObjectWithValues)},
            { "DeleteSpecifiedInformationObject", typeof(DeleteSpecifiedInformationObject)},
            // { "", typeof(object)},
        };

        public static Type GetLegacyMappedType(string operationLegacyName)
        {
            Type legacyMappedType;
            if(LegacyMappedTypes.TryGetValue(operationLegacyName, out legacyMappedType))
                return legacyMappedType;
            return null;
        }
    }
}