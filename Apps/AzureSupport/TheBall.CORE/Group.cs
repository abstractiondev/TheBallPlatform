using AaltoGlobalImpact.OIP;

namespace TheBall.Core
{
    partial class Group : IContainerOwner, IAdditionalFormatProvider
    {
        public string ContainerName => "grp";
        public string LocationPrefix => ID;
        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }

        public static string[] GetAllGroupIDs()
        {
            throw new System.NotImplementedException();
        }
    }
}