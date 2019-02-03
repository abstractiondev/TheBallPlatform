using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using AzureSupport;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.CORE.INT;
using TheBall.CORE.Storage;

namespace TheBall.CORE
{
    public class RemoteDeviceCoreOperationImplementation
    {
        public static DeviceOperationData GetTarget_DeviceOperationData(Stream inputStream)
        {
            return JSONSupport.GetObjectFromStream<DeviceOperationData>(inputStream);
        }

        public static DeviceMembership GetTarget_CurrentDevice()
        {
            return InformationContext.CurrentExecutingForDevice;
        }

        public static async Task ExecuteMethod_PerformOperationAsync(DeviceMembership currentDevice, DeviceOperationData deviceOperationData)
        {
            switch (deviceOperationData.OperationRequestString)
            {
                case "SYNCCOPYCONTENT":
                    await performSyncQueryForCopyContent(currentDevice, deviceOperationData);
                    break;
                case "DELETEREMOTEDEVICE":
                    await currentDevice.DeleteInformationObjectAsync();
                    break;
                case "GETCONTENTMD5LIST":
                    await getContentMD5List(deviceOperationData);
                    break;
                case "COPYSYNCEDCONTENTTOOWNER":
                    await copySyncedContentToOwner(deviceOperationData);
                    break;
                case "GETACCOUNTGROUPS":
                    getAccountGroups(currentDevice, deviceOperationData);
                    break;
                default:
                    throw new NotImplementedException("Not implemented RemoteDevice operation for request string: " + deviceOperationData.OperationRequestString);
            }
            deviceOperationData.OperationResult = true;
        }

        private static void getAccountGroups(DeviceMembership currentDevice, DeviceOperationData deviceOperationData)
        {
            var currAccount = InformationContext.CurrentOwner as TBAccount;
            if(currAccount == null)
                throw new InvalidOperationException("Account group operation requires that current owner is an account");
            deviceOperationData.OperationReturnValues = currAccount.GroupRoleCollection.CollectionContent
                .Where(role => TBCollaboratorRole.IsRoleStatusValidMember(role.RoleStatus))
                .Select(role => role.GroupID).ToArray();
            deviceOperationData.OperationResult = true;
        }

        private static async Task copySyncedContentToOwner(DeviceOperationData deviceOperationData)
        {
            string folderPrefix = deviceOperationData.OperationParameters[0];
            if (!isValidFolderName(folderPrefix))
                throw new InvalidDataException("Invalid data for remote folder name");
            var currentDevice = InformationContext.CurrentExecutingForDevice;
            string deviceInputRoot = getDeviceInputRoot(currentDevice.ID);
            string syncSourceRootFolder = deviceInputRoot + folderPrefix;
            var owner = InformationContext.CurrentOwner;
            ContentItemLocationWithMD5[] syncContentList = (await BlobStorage.GetBlobItemsA(owner, syncSourceRootFolder))
                                                                .Select(blob => new ContentItemLocationWithMD5
                                                                    {
                                                                        ContentLocation = StorageSupport.RemoveOwnerPrefixIfExists(blob.Name)
                                                                            .Substring(syncSourceRootFolder.Length),
                                                                        ContentMD5 = blob.ContentMD5
                                                                    }).ToArray();
            string syncTargetRootFolder = folderPrefix;
            SyncSupport.SynchronizeSourceListToTargetFolder(syncSourceRootFolder, syncContentList, syncTargetRootFolder);
        }

        static bool isValidFolderName(string folderName)
        {
            return folderName.StartsWith("DEV_") || folderName == "wwwsite/";
        }

        private static async Task getContentMD5List(DeviceOperationData deviceOperationData)
        {
            var initialEntries = deviceOperationData.OperationParameters;
            var entries = initialEntries.Select(entry =>
            {
                if (String.IsNullOrEmpty(entry) || entry == "F:")
                    throw new InvalidDataException("Empty entry not supported as prefix");
                string entryName;
                bool isFile = entry.StartsWith("F:");
                if (isFile)
                    entryName = entry.Substring(2);
                else
                {
                    entryName = entry;
                    if (!entryName.EndsWith("/"))
                        entryName += "/";
                }
                return entryName;
            }).ToArray();
            bool hasInvalidFolderNames = entries.Any(entry => SystemSupport.ReservedDomainNames.Any(entry.StartsWith));
            if(hasInvalidFolderNames)
                throw new InvalidDataException("Invalid parameter for remote entry name");
            var owner = InformationContext.CurrentOwner;
            var md5TaskList = entries.Select(entry =>
            {
                var result = BlobStorage.GetBlobItemsA(owner, entry);
                return result;
            }).ToArray();
            await Task.WhenAll(md5TaskList);
            var md5List = md5TaskList.SelectMany(task => task.Result).Select(blob => new ContentItemLocationWithMD5
            {
                ContentLocation = StorageSupport.RemoveOwnerPrefixIfExists(blob.Name),
                ContentMD5 = blob.ContentMD5
            }).ToArray();
            deviceOperationData.OperationSpecificContentData = md5List;
            deviceOperationData.OperationResult = true;
        }

        public static void ExecuteMethod_SerializeDeviceOperationDataToOutput(Stream outputStream, DeviceOperationData deviceOperationData)
        {
            JSONSupport.SerializeToJSONStream(deviceOperationData, outputStream);
        }

        private static async Task performSyncQueryForCopyContent(DeviceMembership currentDevice, DeviceOperationData deviceOperationData)
        {
            string targetNamedFolder = deviceOperationData.OperationParameters != null && deviceOperationData.OperationParameters.Length > 0 ?
                deviceOperationData.OperationParameters[0] : SyncSupport.RelativeRootFolderValue;
            List<ContentItemLocationWithMD5> itemsToCopy = new List<ContentItemLocationWithMD5>();
            List<ContentItemLocationWithMD5> itemsDeleted = new List<ContentItemLocationWithMD5>();
            SyncSupport.CopySourceToTargetMethod copySourceToTarget = async (location, blobLocation) => itemsToCopy.Add(new ContentItemLocationWithMD5
                {
                    ContentLocation = StorageSupport.RemoveOwnerPrefixIfExists(location),
                    ItemDatas = new ItemData[] {new ItemData {DataName = "OPTODO", ItemTextData = "COPY"}}
                });
            SyncSupport.DeleteObsoleteTargetMethod deleteObsoleteTarget = async (location) =>
                {
                    itemsDeleted.Add(new ContentItemLocationWithMD5
                        {
                            ContentLocation = location,
                            ItemDatas = new ItemData[] { new ItemData { DataName = "OPDONE", ItemTextData = "DELETED"}}
                        });
                    await SyncSupport.DeleteObsoleteTargetAsync(location);
                };

            string syncTargetRootFolder = getDeviceInputRoot(currentDevice.ID) + targetNamedFolder;
            if (syncTargetRootFolder.EndsWith("/") == false)
                syncTargetRootFolder += "/";
            await SyncSupport.SynchronizeSourceListToTargetFolder(SyncSupport.RelativeRootFolderValue, deviceOperationData.OperationSpecificContentData, syncTargetRootFolder,
                                                            copySourceToTarget, deleteObsoleteTarget);
            deviceOperationData.OperationSpecificContentData = itemsToCopy.Union(itemsDeleted).ToArray();
        }


        static string getDeviceInputRoot(string deviceID)
        {
            return String.Format("TheBall.CORE/DeviceMembership/{0}_Input/", deviceID);
        }
        /*
                    ItemsToCopy = deviceOperationData.OperationSpecificContentData.Where(item => item.ItemDatas.Any(iData => iData.DataName == "OPTODO" && iData.ItemTextData == "COPY")).ToArray(),
                    ItemsDeleted = deviceOperationData.OperationSpecificContentData.Where(item => item.ItemDatas.Any(iData => iData.DataName == "OPDONE" && iData.ItemTextData == "DELETED")).ToArray()
         * */

    }
}