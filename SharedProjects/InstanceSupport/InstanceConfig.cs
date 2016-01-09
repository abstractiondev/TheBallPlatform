using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;

namespace TheBall.CORE.InstanceSupport
{
    public class InstanceConfig
    {
        public string EmailFromAddress;
        public string EmailValidationSubjectFormat;
        public string EmailValidationMessageFormat;
        public string EmailDeviceJoinSubjectFormat;
        public string EmailDeviceJoinMessageFormat;
        public string EmailGroupJoinSubjectFormat;
        public string EmailGroupJoinMessageFormat;
        public string EmailGroupAndPlatformJoinSubjectFormat;
        public string EmailGroupAndPlatformJoinMessageFormat;
        public string EmailInputJoinSubjectFormat;
        public string EmailInputJoinMessageFormat;
        public string EmailOutputJoinSubjectFormat;
        public string EmailOutputJoinMessageFormat;
        public string EmailAccountMergeValidationSubjectFormat;
        public string EmailAccountMergeValidationMessageFormat;
        public string EmailValidationURLWithoutID;

        public string[] DefaultAccountTemplateList;
        public string AccountDefaultRedirect;
        public string[] DefaultGroupTemplateList;
        public string GroupDefaultRedirect;
        public string[] PlatformDefaultGroupIDList;

        public string AdminGroupID;
        public string PaymentsGroupID;
        public string AllowDirectServingRegexp;

        public string[] ContainerRedirects;
        private Dictionary<string, string> _containerRedirectsDict;
        public Dictionary<string, string> ContainerRedirectsDict
        {
            get
            {
                if (_containerRedirectsDict == null)
                {
                    _containerRedirectsDict = ContainerRedirects?.ToDictionary(keyItem =>
                    {
                        var split = keyItem.Split(':');
                        return split[0];
                    }, valItem =>
                    {
                        var split = valItem.Split(':');
                        return split[1];
                    }) ?? new Dictionary<string, string>();
                }
                return _containerRedirectsDict;
            }
        }

        public string[] WebhookHandlers;
        private Dictionary<string, Tuple<string, string>> _webhookHandlerDict = null;
        public Dictionary<string, Tuple<string, string>> WebhookHandlersDict
        {
            get
            {
                if (_webhookHandlerDict == null)
                {
                    _webhookHandlerDict = new Dictionary<string, Tuple<string, string>>();
                    if (WebhookHandlers != null)
                    {
                        string[] handlerEntries = WebhookHandlers;
                        foreach (string handlerEntry in handlerEntries)
                        {
                            var handlerComponents = handlerEntry.Split(':');
                            if (handlerComponents.Length != 3)
                                continue;
                            string handlerUrlName = handlerComponents[0];
                            string handlerOperationFullName = handlerComponents[1];
                            string handlerOwnerGroup = handlerComponents[2];
                            _webhookHandlerDict.Add(handlerUrlName, new Tuple<string, string>(handlerOperationFullName, handlerOwnerGroup));
                        }
                    }

                }
                return _webhookHandlerDict;
            }
        }


        public static InstanceConfig Current => InformationContext.InstanceConfiguration.InstanceConfig;
    }
}
