using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;

namespace TheBall.Core.InstanceSupport
{
    public class SecureConfig
    {
        public class environment
        {
            public string name;
            public EnvironmentConfig config;
        }

        private Dictionary<string, EnvironmentConfig> environmentConfigs = new Dictionary<string, EnvironmentConfig>();
        public Dictionary<string, EnvironmentConfig> EnvironmentConfigs
        {
            get
            {
                if (environmentConfigs == null)
                {
                    environmentConfigs = environments?.ToDictionary(env => env.name, env => env.config);
                }
                return environmentConfigs;
            }
        }
        public class EnvironmentConfig
        {
            public string stripeSecretKey;
            public string stripePublicKey;
            public string applicationInsightsID;
        }


        public string AWSAccessKey;
        public string AWSSecretKey;
        //public string AzureStorageConnectionString;
        public string AzureStorageKey;
        public string AzureAccountName;
        public string CoreFileShareAccountName;
        public string CoreFileShareAccountKey;
        public string CoreShareWithFolderName;

        public string WilmaSharedSecret;
        public string GoogleOAuthClientID;
        public string GoogleOAuthClientSecret;

        public string FacebookOAuthClientID;
        public string FacebookOAuthClientSecret;


        public string StripeLivePublicKey;
        public string StripeLiveSecretKey;

        public string StripeTestPublicKey;
        public string StripeTestSecretKey;
        public environment[] environments;

        public static SecureConfig Current => InformationContext.InstanceConfiguration.SecureConfig;
    }
}
