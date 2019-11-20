using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.Core;
using TheBall.Core.INT;
using TheBall.Core.Storage;

namespace TheBall
{
    public static class SyncSupport
    {

        public static async Task SynchronizeSourceListToTargetFolder(string syncSourceRootFolder, ContentItemLocationWithMD5[] sourceContentList, string syncTargetRootFolder, 
            CopySourceToTargetMethod copySourceToTarget = null, DeleteObsoleteTargetMethod deleteObsoleteTarget = null)
        {
            if (copySourceToTarget == null)
                copySourceToTarget = CopySourceToTargetAsync;
            if (deleteObsoleteTarget == null)
                deleteObsoleteTarget = DeleteObsoleteTargetAsync;

            if (String.IsNullOrEmpty(syncSourceRootFolder) || syncSourceRootFolder == "/")
                syncSourceRootFolder = RelativeRootFolderValue;
            else if(syncSourceRootFolder.EndsWith("/") == false)
            {
                syncSourceRootFolder += "/";
            }

            var blobListing = await BlobStorage.GetOwnerBlobsA(InformationContext.CurrentOwner, syncTargetRootFolder);
            string fullTargetRootPath = StorageSupport.GetOwnerContentLocation(InformationContext.CurrentOwner, syncTargetRootFolder);
            int fullTargetRootPathLength = fullTargetRootPath.Length;
            ContentItemLocationWithMD5[] targetContents = (from blob in blobListing
                                                           let relativeName = blob.Name.Substring(fullTargetRootPathLength)
                                                           select new ContentItemLocationWithMD5
                                                           {
                                                               ContentLocation = relativeName,
                                                               ContentMD5 = blob.ContentMD5
                                                           }).OrderBy(item => item.ContentLocation).ToArray();
            var sourceContents = sourceContentList.OrderBy(item => item.ContentLocation).ToArray();
            foreach (var sourceContent in sourceContents)
            {
                string originalLocation = sourceContent.ContentLocation;
                string nonOwnerLocation = StorageSupport.RemoveOwnerPrefixIfExists(originalLocation);
                string fixedContentLocation;
                if (originalLocation != nonOwnerLocation && syncSourceRootFolder != RelativeRootFolderValue)
                {
                    if(nonOwnerLocation.StartsWith(syncSourceRootFolder) == false)
                        throw new InvalidDataException("Sync source must start with given root location");
                    fixedContentLocation = nonOwnerLocation.Substring(syncSourceRootFolder.Length);
                }
                else
                    fixedContentLocation = nonOwnerLocation;
                sourceContent.ContentLocation = fixedContentLocation;
            }
            int currSourceIX = 0;
            int currTargetIX = 0;
            while (currSourceIX < sourceContents.Length || currTargetIX < targetContents.Length)
            {
                var currSource = currSourceIX < sourceContents.Length ? sourceContents[currSourceIX] : null;
                var currTarget = currTargetIX < targetContents.Length ? targetContents[currTargetIX] : null;
                string currTargetBlobLocation = null;
                string currSourceBlobLocation = null;
                if (currSource != null && currTarget != null)
                {
                    if (currSource.ContentLocation == currTarget.ContentLocation)
                    {
                        currSourceIX++;
                        currTargetIX++;
                        if (currSource.ContentMD5 == currTarget.ContentMD5)
                            continue;
                        currSourceBlobLocation = StorageSupport.GetOwnerContentLocation(InformationContext.CurrentOwner, syncSourceRootFolder + currSource.ContentLocation);
                        currTargetBlobLocation = fullTargetRootPath + currTarget.ContentLocation;
                    }
                    else if (String.Compare(currSource.ContentLocation, currTarget.ContentLocation) < 0)
                    {
                        currSourceIX++;
                        currSourceBlobLocation = StorageSupport.GetOwnerContentLocation(InformationContext.CurrentOwner, syncSourceRootFolder + currSource.ContentLocation);
                        currTargetBlobLocation = fullTargetRootPath + currSource.ContentLocation;
                    }
                    else // source == null, target != null
                    {
                        currTargetIX++;
                        currTargetBlobLocation = fullTargetRootPath + currTarget.ContentLocation;
                    }
                }
                else if (currSource != null)
                {
                    currSourceIX++;
                    currSourceBlobLocation = StorageSupport.GetOwnerContentLocation(InformationContext.CurrentOwner, syncSourceRootFolder + currSource.ContentLocation);
                    currTargetBlobLocation = fullTargetRootPath + currSource.ContentLocation;
                }
                else if (currTarget != null)
                {
                    currTargetIX++;
                    currTargetBlobLocation = fullTargetRootPath + currTarget.ContentLocation;
                }

                // at this stage we have either both set (that's copy) or just target set (that's delete)
                if (currSourceBlobLocation != null && currTargetBlobLocation != null)
                    await copySourceToTarget(currSourceBlobLocation, currTargetBlobLocation);
                else if (currTargetBlobLocation != null)
                    await deleteObsoleteTarget(currTargetBlobLocation);

            }
        }

        public delegate Task DeleteObsoleteTargetMethod(string currTargetBlobLocation);

        public delegate Task CopySourceToTargetMethod(string currSourceBlobLocation, string currTargetBlobLocation);

        public static async Task DeleteObsoleteTargetAsync(string currTargetBlobLocation)
        {
            CloudBlockBlob blob = (CloudBlockBlob)StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, currTargetBlobLocation);
            await blob.DeleteAsync();
        }

        public static async Task CopySourceToTargetAsync(string currSourceBlobLocation, string currTargetBlobLocation)
        {
            CloudBlockBlob targetBlob = (CloudBlockBlob)StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, currTargetBlobLocation);
            CloudBlockBlob sourceblob = (CloudBlockBlob)StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, currSourceBlobLocation);
            await targetBlob.StartCopyAsync(sourceblob);
        }

        public const string RelativeRootFolderValue = "";
    }
}