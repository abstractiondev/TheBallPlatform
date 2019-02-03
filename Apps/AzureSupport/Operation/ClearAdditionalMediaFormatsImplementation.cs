using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall;
using TheBall.CORE.Storage;

namespace AaltoGlobalImpact.OIP
{
    public static class ClearAdditionalMediaFormatsImplementation
    {
        public static async Task ExecuteMethod_ClearImageMediaFormatsAsync(string masterRelativeLocation)
        {
            string currExtension = Path.GetExtension(masterRelativeLocation);
            int currExtensionLength = currExtension?.Length ?? 0;
            string masterLocationWithoutExtension = masterRelativeLocation.Substring(0,
                                                                                     masterRelativeLocation.Length -
                                                                                     currExtensionLength);
            var masterRelatedBlobs = await  BlobStorage.GetBlobItemsA(null, masterLocationWithoutExtension + "_");
            var deleteTasks = new List<Task>();
            foreach(var blob in masterRelatedBlobs.Where(blob => blob.Name.EndsWith(".jpg") || blob.Name.EndsWith(".png") || blob.Name.EndsWith(".gif")))
            {
                deleteTasks.Add(BlobStorage.DeleteBlobA(blob.Name));
            }
            await Task.WhenAll(deleteTasks);
        }
    }
}