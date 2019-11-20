using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using TheBall;
using TheBall.Core;

namespace AaltoGlobalImpact.OIP
{
    partial class LinkToContent : IBeforeStoreHandler, IAdditionalFormatProvider
    {
        public async Task PerformBeforeStoreUpdate()
        {
            URL = URL?.Trim();
            if (ImageData == null && string.IsNullOrEmpty(URL) == false)
            {
                ImageData = new MediaContent();
                byte[] jpegData;
                var bitmap = WebsiteThumbnail.WebsiteThumbnailImageGenerator.GetWebSiteThumbnail(URL,
                                                                                                 1280, 800, 1280, 800);
                using (MemoryStream memStream = new MemoryStream())
                {
                    bitmap.Save(memStream, ImageFormat.Jpeg);
                    jpegData = memStream.ToArray();
                }
                MediaFileData mediaContent = new MediaFileData
                    {
                        FileName = "AutoFetch.jpg",
                        FileContent = jpegData
                    };
                await this.SetMediaContent(InformationContext.CurrentOwner, ImageData.ID, mediaContent);
            }
            if (Published == default(DateTime))
                Published = Published.ToUniversalTime();
        }

        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }
    }
}