using AaltoGlobalImpact.OIP;

namespace TheBall.Payments
{
    partial class CustomerAccount : IAdditionalFormatProvider
    {
        public object FindObjectByID(string objectId)
        {
            throw new System.NotImplementedException();
        }

        public AdditionalFormatContent[] GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag);
        }

        public string[] GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }
    }
}