using System.IO;
using System.Threading.Tasks;
using TheBall.Core;
using TheBall.Core.StorageCore;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class DeleteInterfaceJSONImplementation
    {
        public static string GetTarget_DataName(InterfaceJSONData saveDataInfo)
        {
            return saveDataInfo.Name;
        }

        public static string GetTarget_JSONDataFileLocation(string dataName)
        {
            var relativePath = Path.Combine("TheBall.Interface", "InterfaceData", $"{dataName}.json").Replace("\\", "/");
            return relativePath;
        }

        public static async Task ExecuteMethod_DeleteJSONDataAsync(string jsonDataFileLocation)
        {
            var storageService = CoreServices.GetCurrent<IStorageService>();
            var blobPath = storageService.GetOwnerContentLocation(InformationContext.CurrentOwner, jsonDataFileLocation);
            await storageService.DeleteBlobA(blobPath);
            //var blob = StorageSupport.GetOwnerBlobReference(jsonDataFileLocation);
            //await blob.DeleteIfExistsAsync();
        }
    }
}