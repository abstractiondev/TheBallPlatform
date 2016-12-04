using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
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
    }
}