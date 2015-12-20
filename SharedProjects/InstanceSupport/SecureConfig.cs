using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure;

namespace TheBall.CORE.InstanceSupport
{
    public class SecureConfig
    {
        public string AWSAccessKey;
        public string AWSSecretKey;
        public string AzureStorageConnectionString;
        public string AzureStorageKey;
        public string AzureAccountName;
        public string CoreFileShareAccountName;
        public string CoreFileShareAccountKey;
        public string CoreShareWithFolderName;

        public string WilmaSharedSecret;
        public string GoogleOAuthClientID;
        public string GoogleOAuthClientSecret;

        public string StripePublicKey;
        public string StripeSecretKey;

        public static SecureConfig Current => InformationContext.InstanceConfiguration.SecureConfig;
    }
}
