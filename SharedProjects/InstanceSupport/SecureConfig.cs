using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure;

namespace TheBall.CORE.InstanceSupport
{
    public class SecureConfig
    {
        private static SecureConfig currentConfig;

        public static void InitializeCurrent(SettingRetriever settingRetriever, string azureConnStrFileName,
            string awsSecretFileName, bool IsDeveloperMachine)
        {
            currentConfig = new SecureConfig(settingRetriever, azureConnStrFileName, awsSecretFileName, IsDeveloperMachine);
        }
        public static SecureConfig Current { get; } = currentConfig;

        public readonly string AppInsightInstrumentationKey;
        public readonly string AWSAccessKey;
        public readonly string AWSSecretKey;
        public readonly string AzureStorageConnectionString;
        public readonly string AzureStorageKey;
        public readonly string AzureAccountName;
        public readonly string CoreFileShareAccountName;
        public readonly string CoreFileShareAccountKey;
        public readonly string CoreShareWithFolderName;

        // Infrastructure content/fields
        public static readonly int HARDCODED_StatusUpdateExpireSeconds = 300;

        public SecureConfig(SettingRetriever settingRetriever, string azureConnStrFileName, string awsSecretFileName, bool IsDeveloperMachine)
        {
            # region Infrastructure

            #endregion
            #region Monitoring

            AppInsightInstrumentationKey = settingRetriever("AppInsightInstrumentationKey");
            #endregion

            #region Data storage

            AzureStorageConnectionString = azureConnStrFileName != null && File.Exists(azureConnStrFileName) ? 
                File.ReadAllText(azureConnStrFileName) :
                settingRetriever("DataConnectionString");
            if (AzureStorageConnectionString == null)
                throw new InvalidDataException("DataConnectionString not set properly or not available in configuration");
            var connStrSplits = AzureStorageConnectionString.Split(new[] { ";AccountKey=" }, StringSplitOptions.None);
            AzureStorageKey = connStrSplits[1];
            connStrSplits = connStrSplits[0].Split(new[] { ";AccountName=" }, StringSplitOptions.None);
            AzureAccountName = connStrSplits[1];
            CoreFileShareAccountKey = settingRetriever("CoreFileShareAccountKey");
            CoreFileShareAccountName = settingRetriever("CoreFileShareAccountName");
            if (IsDeveloperMachine)
                CoreShareWithFolderName = String.Format(@"x:\{0}", "tbcore");
            else
                CoreShareWithFolderName = String.Format(@"\\{0}.file.core.windows.net\{1}", CoreFileShareAccountName, "tbcore");
            #endregion


            #region Email

            try
            {
                string configString;
                configString = awsSecretFileName != null && File.Exists(awsSecretFileName) ? 
                    File.ReadAllText(awsSecretFileName) :
                    settingRetriever("AmazonSESAccessInfo");
                string[] strValues = configString.Split(';');
                AWSAccessKey = strValues[0];
                AWSSecretKey = strValues[1];
            }
            catch // Neutral credentials - will revert to queue put when message send is failing at EmailSupport
            {
                AWSAccessKey = "";
                AWSSecretKey = "";
            }


            #endregion

        }
    }
}
