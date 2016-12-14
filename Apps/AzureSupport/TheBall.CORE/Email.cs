using System;
using System.Web;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    partial class Email : IAdditionalFormatProvider, IBeforeStoreHandler
    {
        public static string GetIDFromEmailAddress(string emailAddress)
        {
            return HttpUtility.UrlEncode(emailAddress.ToLower());
        }

        public static string GetEmailAddressFromID(string emailID)
        {
            return HttpUtility.UrlDecode(emailID.ToLower());
        }

        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }

        public void PerformBeforeStoreUpdate()
        {
            if (ValidationProcessExpiration == DateTime.MinValue)
                ValidationProcessExpiration = DateTime.MinValue.ToUniversalTime();
        }
    }
}