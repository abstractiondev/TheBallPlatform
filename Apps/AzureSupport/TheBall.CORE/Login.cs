using System;
using System.IO;
using System.Web;
using AaltoGlobalImpact.OIP;
using AzureSupport;
//using RestSharp.Extensions.MonoHttp;

namespace TheBall.CORE
{
    partial class Login : IAdditionalFormatProvider
    {
        const string httpsPrefix = "https://";
        const string httpPrefix = "http://";
        const string emailPrefix = "email://";
        public static string GetLoginIDFromLoginURL(string loginURL)
        {
            string pureId;
            if (loginURL.StartsWith(httpsPrefix))
                pureId = loginURL.Substring(httpsPrefix.Length);
            else if (loginURL.StartsWith(httpPrefix))
                pureId = loginURL.Substring(httpPrefix.Length);
            else if (loginURL.StartsWith(emailPrefix))
                pureId = loginURL.Substring(emailPrefix.Length);
            else
                throw new NotSupportedException("Not supported user name prefix: " + loginURL);
            var loginID = HttpUtility.UrlEncode(pureId);
            return loginID;
        }

        public static string GetLoginUrlFromEmailAddress(string emailAddress)
        {
            if(emailAddress.StartsWith(emailPrefix))
                throw new InvalidDataException("Email address already starts with prefix: " + emailAddress);
            return emailPrefix + emailAddress;
        }

        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }
    }
}