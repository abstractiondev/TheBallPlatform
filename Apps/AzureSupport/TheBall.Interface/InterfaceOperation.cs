using System;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;

namespace TheBall.Interface
{
    partial class InterfaceOperation : IAdditionalFormatProvider, IBeforeStoreHandler
    {
        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }

        async Task IBeforeStoreHandler.PerformBeforeStoreUpdate()
        {
            if (Created == default(DateTime))
                Created = DateTime.UtcNow;
            if (Started == default(DateTime))
                Started = DateTime.MinValue.ToUniversalTime();
            if (Finished == default(DateTime))
                Finished = DateTime.MinValue.ToUniversalTime();
        }
    }
}