using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    partial class GroupContainer : IBeforeStoreHandler, IAdditionalFormatProvider
    {
        public async Task PerformBeforeStoreUpdate()
        {
            this.GroupIndex.Icon = this.GroupProfile.ProfileImage;
            this.LocationCollection.IsCollectionFiltered = true;
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