using System;
using System.Threading.Tasks;
using System.Web;
using AaltoGlobalImpact.OIP;
using static System.Net.WebUtility;

namespace TheBall.CORE
{
    partial class Email : IAdditionalFormatProvider, IBeforeStoreHandler
    {
        public static string GetIDFromEmailAddress(string emailAddress)
        {
            if (emailAddress == null)
                return null;
            return UrlEncode(emailAddress.ToLower());
        }

        public static string GetEmailAddressFromID(string emailID)
        {
            if (emailID == null)
                return null;
            return UrlDecode(emailID.ToLower());
        }

        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }

        public async Task PerformBeforeStoreUpdate()
        {
            if (ValidationProcessExpiration == DateTime.MinValue)
                ValidationProcessExpiration = DateTime.MinValue.ToUniversalTime();
        }
    }
}