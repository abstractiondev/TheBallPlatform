using TheBall;

namespace PersonalWeb.Diosphere
{
    public class SaveRoomDataImplementation
    {
        public static string GetTarget_OwnerRootRoomBlobName()
        {
            return "room.json";
        }

        public static void ExecuteMethod_SaveJSONContentToBlob(string jsonData, string ownerRootRoomBlobName)
        {
            var owner = InformationContext.CurrentOwner;
            var ownerRootedBlob = StorageSupport.GetOwnerBlobReference(owner, ownerRootRoomBlobName);
            ownerRootedBlob.UploadBlobText(jsonData);
        }
    }
}