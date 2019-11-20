using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.Core.INT;
using TheBall.Core.Storage;

namespace TheBall.Core
{
    public class SyncCopyContentToDeviceTargetImplementation
    {
        public static async Task<AuthenticatedAsActiveDevice> GetTarget_AuthenticatedAsActiveDeviceAsync(string authenticatedAsActiveDeviceId)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<AuthenticatedAsActiveDevice>(InformationContext.CurrentOwner, authenticatedAsActiveDeviceId);
        }

        public static string GetTarget_ContentRootLocation(AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            var contentRootLocation = StorageSupport.GetOwnerContentLocation(InformationContext.CurrentOwner, authenticatedAsActiveDevice.RelativeLocation) + "/";
            if (contentRootLocation.EndsWith("/") == false)
                contentRootLocation += "/";
            return contentRootLocation;
        }

        public static async Task<ContentItemLocationWithMD5[]> GetTarget_ThisSideContentMD5ListAsync(string contentRootLocation)
        {
            var blobList = await BlobStorage.GetOwnerBlobsA(InformationContext.CurrentOwner, contentRootLocation);
            int contentRootLength = contentRootLocation.Length;
            List<ContentItemLocationWithMD5> list = new List<ContentItemLocationWithMD5>();
            foreach (var blob in blobList)
            {
                string relativeLocation = blob.Name.Substring(contentRootLength);
                list.Add(new ContentItemLocationWithMD5
                    {
                        ContentLocation = relativeLocation, ContentMD5 = blob.ContentMD5
                    });
            }
            return list.ToArray();
        }

        public static async Task<SyncCopyContentToDeviceTarget.CallPrepareTargetAndListItemsToCopyReturnValue> ExecuteMethod_CallPrepareTargetAndListItemsToCopyAsync(AuthenticatedAsActiveDevice authenticatedAsActiveDevice, ContentItemLocationWithMD5[] thisSideContentMd5List)
        {
            DeviceOperationData deviceOperationData = new DeviceOperationData
                {
                    OperationRequestString = "SYNCCOPYCONTENT",
                    OperationSpecificContentData = thisSideContentMd5List,
                    OperationParameters = new string[] { SyncSupport.RelativeRootFolderValue}
                };
            deviceOperationData = await DeviceSupport.ExecuteRemoteOperation<DeviceOperationData>(authenticatedAsActiveDevice.ID,
                                                                                            "TheBall.Core.RemoteDeviceCoreOperation", deviceOperationData);
            var returnValue = new SyncCopyContentToDeviceTarget.CallPrepareTargetAndListItemsToCopyReturnValue
                {
                    ItemsToCopy = deviceOperationData.OperationSpecificContentData.Where(item => item.ItemDatas.Any(iData => iData.DataName == "OPTODO" && iData.ItemTextData == "COPY")).ToArray(),
                    ItemsDeleted = deviceOperationData.OperationSpecificContentData.Where(item => item.ItemDatas.Any(iData => iData.DataName == "OPDONE" && iData.ItemTextData == "DELETED")).ToArray()
                };
            return returnValue;
        }

        public static async Task ExecuteMethod_CopyItemsToCopyToTargetDeviceAsync(AuthenticatedAsActiveDevice authenticatedAsActiveDevice, SyncCopyContentToDeviceTarget.CallPrepareTargetAndListItemsToCopyReturnValue callPrepareTargetAndListItemsToCopyOutput)
        {
            var itemsToCopy = callPrepareTargetAndListItemsToCopyOutput.ItemsToCopy;
            foreach(var itemToCopy in itemsToCopy)
            {
                string ownerRelatedLocation = StorageSupport.RemoveOwnerPrefixIfExists(itemToCopy.ContentLocation);
                await DeviceSupport.PushContentToDevice(authenticatedAsActiveDevice, ownerRelatedLocation, ownerRelatedLocation);
            }
        }

        public static SyncCopyContentToDeviceTargetReturnValue Get_ReturnValue(SyncCopyContentToDeviceTarget.CallPrepareTargetAndListItemsToCopyReturnValue callPrepareTargetAndListItemsToCopyOutput)
        {
            return new SyncCopyContentToDeviceTargetReturnValue
                {
                    CopiedItems = callPrepareTargetAndListItemsToCopyOutput.ItemsToCopy,
                    DeletedItems = callPrepareTargetAndListItemsToCopyOutput.ItemsDeleted
                };
        }
    }
}