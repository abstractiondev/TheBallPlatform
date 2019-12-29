using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using AzureSupport;
using TheBall.Core;
using TheBall.Core.Storage;
using TheBall.Core.StorageCore;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class SaveInterfaceJSONImplementation
    {
        public static string GetTarget_DataName(InterfaceJSONData saveDataInfo)
        {
            return saveDataInfo.Name;
        }

        public static ExpandoObject GetTarget_DataObject(InterfaceJSONData saveDataInfo)
        {
            return saveDataInfo.Data;
        }

        public static string GetTarget_JSONDataFileLocation(string dataName)
        {
            var relativePath = Path.Combine("TheBall.Interface", "InterfaceData", $"{dataName}.json").Replace("\\", "/");
            return relativePath;
        }

        public static async Task ExecuteMethod_StoreJSONDataAsync(string jsonDataFileLocation, ExpandoObject dataObject)
        {
            var storageService = CoreServices.GetCurrent<IStorageService>();
            var content = JSONSupport.SerializeToJSONString(dataObject);
            await storageService.UploadBlobTextA(InformationContext.CurrentOwner, jsonDataFileLocation, content);
            //var blob = StorageSupport.GetOwnerBlobReference(jsonDataFileLocation);

            //await blob.UploadBlobTextAsync(content, false);
        }
    }
}