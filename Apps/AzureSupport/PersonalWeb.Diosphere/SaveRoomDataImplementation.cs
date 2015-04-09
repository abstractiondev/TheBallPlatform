using System.Linq;
using System.Security;
using AaltoGlobalImpact.OIP;
using TheBall;
using TheBall.CORE;

namespace PersonalWeb.Diosphere
{
    public class SaveRoomDataImplementation
    {
        public static string GetTarget_OwnerRootRoomBlobName()
        {
            return "room.json";
        }

        public static void ExecuteMethod_SaveJSONContentToBlob(string jsonData, IContainerOwner owner, string ownerRootRoomBlobName)
        {
            var ownerRootedBlob = StorageSupport.GetOwnerBlobReference(owner, ownerRootRoomBlobName);
            ownerRootedBlob.UploadBlobText(jsonData);
        }

        public static IContainerOwner GetTarget_Owner(string roomId)
        {
            var currentOwner = InformationContext.CurrentOwner;
            if (roomId != null)
            {
                TBAccount account = currentOwner as TBAccount;
                if(account == null)
                    throw new SecurityException("RoomID based owner is only allowed to be used at account level");
                var roomRole = account.GroupRoleCollection.CollectionContent.FirstOrDefault(grp => grp.GroupID == roomId);
                if(roomRole == null || TBCollaboratorRole.HasModeratorRights(roomRole.GroupRole) == false)
                    throw new SecurityException("RoomID based owner is only allowed to be used with moderator level access");
                return new VirtualOwner("grp", roomId);
            }
            return InformationContext.CurrentOwner;
        }
    }
}