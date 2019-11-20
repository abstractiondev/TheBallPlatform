using System;
using System.Dynamic;
using System.Text;
using AaltoGlobalImpact.OIP;
using TheBall.Core.Storage;

namespace TheBall.Core
{
    partial class Account : IContainerOwner, IAdditionalFormatProvider
    {
        public string ContainerName => "acc";
        public string LocationPrefix => ID;

        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }

        public string GetClientMetadataAsBase64()
        {
            string base64ClientMetadata = null;
            if (!String.IsNullOrEmpty(ClientMetadataJSON))
            {
                var jsonData = Encoding.UTF8.GetBytes(ClientMetadataJSON);
                base64ClientMetadata = Convert.ToBase64String(jsonData);
            }
            return base64ClientMetadata;
        }

        public string GetServerMetadataAsBase64()
        {
            string base64ServerMetadata = null;
            if (!String.IsNullOrEmpty(ClientMetadataJSON))
            {
                var jsonData = Encoding.UTF8.GetBytes(ServerMetadataJSON);
                base64ServerMetadata = Convert.ToBase64String(jsonData);
            }
            return base64ServerMetadata;
        }

        public ExpandoObject GetClientMetadataObject()
        {
            if (String.IsNullOrEmpty(ClientMetadataJSON))
                return null;
            var expando = JSONSupport.GetExpandoObject(ClientMetadataJSON);
            return expando;
        }

        public static string[] GetAllAccountIDs()
        {
            throw new NotImplementedException();
        }
    }
}