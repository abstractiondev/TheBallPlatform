using AaltoGlobalImpact.OIP;

namespace TheBall.Payments
{
    partial class GroupSubscriptionPlanCollection : IAdditionalFormatProvider
    {
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