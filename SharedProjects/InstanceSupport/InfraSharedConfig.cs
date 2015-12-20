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
        public static InfraSharedConfig Current => RuntimeConfiguration.GetInfraSharedConfig();

        public string AppInsightInstrumentationKey;
        public string VersionString;
        public string RedirectFromFolderFileName;
        public string[] InstanceNames;

        public bool IsDeveloperMachine;
        public bool UseSQLiteMasterDatabase;

        // Infrastructure content/fields
        public static readonly int HARDCODED_StatusUpdateExpireSeconds = 300;
    }
}
