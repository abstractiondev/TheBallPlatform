using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure;

namespace TheBall.CORE.InstanceSupport
{
    public class InstanceConfig
    {
        private static InstanceConfig currentConfig;

        public static void InitializeCurrent(SettingRetriever settingRetriever)
        {
            currentConfig = new InstanceConfig(settingRetriever);
        }
        public static InstanceConfig Current { get; } = currentConfig;


        public readonly string EmailFromAddress;
        public readonly string EmailValidationSubjectFormat;
        public readonly string EmailValidationMessageFormat;
        public readonly string EmailDeviceJoinSubjectFormat;
        public readonly string EmailDeviceJoinMessageFormat;
        public readonly string EmailGroupJoinSubjectFormat;
        public readonly string EmailGroupJoinMessageFormat;
        public readonly string EmailGroupAndPlatformJoinSubjectFormat;
        public readonly string EmailGroupAndPlatformJoinMessageFormat;
        public readonly string EmailInputJoinSubjectFormat;
        public readonly string EmailInputJoinMessageFormat;
        public readonly string EmailOutputJoinSubjectFormat;
        public readonly string EmailOutputJoinMessageFormat;
        public readonly string EmailAccountMergeValidationSubjectFormat;
        public readonly string EmailAccountMergeValidationMessageFormat;
        public readonly string EmailValidationURLWithoutID;
        public readonly string WorkerActiveContainerName;

        public readonly string[] DefaultAccountTemplateList;
        public readonly string AccountDefaultRedirect;
        public readonly string[] DefaultGroupTemplateList;
        public readonly string GroupDefaultRedirect;
        public readonly string[] PlatformDefaultGroupIDList;

        public readonly string AdminGroupID;
        public readonly string PaymentsGroupID;

        public Dictionary<string, string> ContainerRedirects = new Dictionary<string, string>();

        public readonly string AllowDirectServingRegexp;

        public InstanceConfig(SettingRetriever settingRetriever)
        {
            # region Infrastructure

            #endregion
            #region Monitoring

            #endregion

            #region Data storage

            WorkerActiveContainerName = settingRetriever("WorkerActiveContainerName");
            var containerRedirectValue = settingRetriever("ContainerRedirects");
            if (String.IsNullOrEmpty(containerRedirectValue) == false)
            {
                string[] redirectEntries = containerRedirectValue.Split(',');
                foreach (string redirectEntry in redirectEntries)
                {
                    var redirectComponents = redirectEntry.Split(':');
                    string redirectFrom = redirectComponents[0];
                    string redirectTo = redirectComponents[1];
                    ContainerRedirects.Add(redirectFrom, redirectTo);
                }
            }

            #endregion

            #region System Level

            AccountDefaultRedirect = settingRetriever("AccountDefaultRedirect");
            GroupDefaultRedirect = settingRetriever("GroupDefaultRedirect");
            AdminGroupID = settingRetriever("AdminGroupID");
            PaymentsGroupID = settingRetriever("PaymentsGroupID");
            AllowDirectServingRegexp = settingRetriever("AllowDirectServingRegexp");

            #endregion

            #region Email

            EmailFromAddress = settingRetriever("EmailFromAddress");
            EmailDeviceJoinMessageFormat = settingRetriever("EmailDeviceJoinMessageFormat");
            EmailDeviceJoinSubjectFormat = settingRetriever("EmailDeviceJoinSubjectFormat");
            EmailInputJoinSubjectFormat = settingRetriever("EmailInputJoinSubjectFormat");
            EmailInputJoinMessageFormat = settingRetriever("EmailInputJoinMessageFormat");
            EmailOutputJoinSubjectFormat = settingRetriever("EmailOutputJoinSubjectFormat");
            EmailOutputJoinMessageFormat = settingRetriever("EmailOutputJoinMessageFormat");
            EmailValidationSubjectFormat = settingRetriever("EmailValidationSubjectFormat");
            EmailValidationMessageFormat = settingRetriever("EmailValidationMessageFormat");
            EmailGroupJoinSubjectFormat = settingRetriever("EmailGroupJoinSubjectFormat");
            EmailGroupJoinMessageFormat = settingRetriever("EmailGroupJoinMessageFormat");

            EmailGroupAndPlatformJoinSubjectFormat = settingRetriever("EmailGroupAndPlatformJoinSubjectFormat");
            EmailGroupAndPlatformJoinMessageFormat = settingRetriever("EmailGroupAndPlatformJoinMessageFormat");

            EmailAccountMergeValidationSubjectFormat = settingRetriever("EmailAccountMergeValidationSubjectFormat");
            EmailAccountMergeValidationMessageFormat = settingRetriever("EmailAccountMergeValidationMessageFormat");
            EmailValidationURLWithoutID = settingRetriever("EmailValidationURLWithoutID");

            #endregion

            var defaultAccountTemplateList = settingRetriever("DefaultAccountTemplateList");
            if (defaultAccountTemplateList != null)
                DefaultAccountTemplateList = defaultAccountTemplateList.Split(',');
            var defaultGroupTemplateList = settingRetriever("DefaultGroupTemplateList");
            if (defaultGroupTemplateList != null)
                DefaultGroupTemplateList = defaultGroupTemplateList.Split(',');

            var platformDefaultGroupIDList = settingRetriever("PlatformDefaultGroupIDList");
            if (platformDefaultGroupIDList != null)
                PlatformDefaultGroupIDList = platformDefaultGroupIDList.Split(',');

        }
    }
}
