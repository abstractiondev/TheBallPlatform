using AaltoGlobalImpact.OIP;

namespace ProBroz.OnlineTraining
{
    public partial class Member : IAdditionalFormatProvider
    {
        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }

        public object FindObjectByID(string objectId)
        {
            throw new System.NotImplementedException();
        }
    }
}