using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure;

namespace TheBall.CORE.InstanceSupport
{
    public delegate string SettingRetriever(string settingName);
    public class InfraSharedConfig
    {
        private static InfraSharedConfig currentConfig;

        public static void InitializeCurrent(SettingRetriever settingRetriever)
        {
            currentConfig = new InfraSharedConfig(settingRetriever);
        }
        public static InfraSharedConfig Current { get; } = currentConfig;


        public readonly string VersionString = "v1.0.6d";
        public readonly string RedirectFromFolderFileName;
        public readonly string[] DefaultAccountTemplateList;
        public readonly string AccountDefaultRedirect;
        public readonly string[] DefaultGroupTemplateList;
        public readonly string GroupDefaultRedirect;
        public readonly string[] PlatformDefaultGroupIDList;
        public readonly string CoreShareWithFolderName;

        public Dictionary<string, Tuple<string, string>> WebhookHandlers = new Dictionary<string, Tuple<string, string>>();
        public readonly bool IsDeveloperMachine;
        public readonly bool UseSQLiteMasterDatabase;
        public readonly string[] DynamicDataCRUDDomains;

        // Infrastructure content/fields
        public static readonly int HARDCODED_StatusUpdateExpireSeconds = 300;

        public InfraSharedConfig(SettingRetriever settingRetriever)
        {

            string webhookHandlersValue = settingRetriever("WebhookHandlers");
            if (String.IsNullOrEmpty(webhookHandlersValue) == false)
            {
                string[] handlerEntries = webhookHandlersValue.Split(',');
                foreach (string handlerEntry in handlerEntries)
                {
                    var handlerComponents = handlerEntry.Split(':');
                    if (handlerComponents.Length != 3)
                        continue;
                    string handlerUrlName = handlerComponents[0];
                    string handlerOperationFullName = handlerComponents[1];
                    string handlerOwnerGroup = handlerComponents[2];
                    WebhookHandlers.Add(handlerUrlName, new Tuple<string, string>(handlerOperationFullName, handlerOwnerGroup));
                }
            }

            RedirectFromFolderFileName = settingRetriever("RedirectFromFolderFileName");
            UseSQLiteMasterDatabase = Convert.ToBoolean(settingRetriever("UseSQLiteMasterDatabase"));
            string dynamicDataDomains = settingRetriever("DynamicDataCRUDDomains");
            if (string.IsNullOrEmpty(dynamicDataDomains))
                DynamicDataCRUDDomains = new string[] { };
            else
                DynamicDataCRUDDomains = dynamicDataDomains.Split(',');
        }
    }
}
