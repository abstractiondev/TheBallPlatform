using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class InitiateImportedGroupWithUnchangedIDImplementation
    {
        public static IContainerOwner GetTarget_GroupAsOwner(string groupId)
        {
            return VirtualOwner.FigureOwner("grp/" + groupId);
        }

        public static async Task<GroupContainer> GetTarget_GroupContainerAsync(IContainerOwner groupAsOwner)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<GroupContainer>(groupAsOwner, "default");
        }

        public static void ExecuteMethod_ValidateGroupContainerID(string groupId, GroupContainer groupContainer)
        {
            if(groupContainer == null)
                throw new ArgumentNullException("groupContainer");
            if(groupContainer.RelativeLocation != "grp/" + groupId + "/AaltoGlobalImpact.OIP/GroupContainer/default")
                throw new InvalidDataException("Mismatch in group container location relative to group ID");
        }

        public static TBRGroupRoot GetTarget_GroupRoot(string groupID)
        {
            TBRGroupRoot groupRoot = new TBRGroupRoot();
            groupRoot.ID = groupID;
            groupRoot.UpdateRelativeLocationFromID();
            groupRoot.Group = new TBCollaboratingGroup();
            groupRoot.Group.ID = groupID;
            return groupRoot;
        }

        public static void ExecuteMethod_SetGroupInitiatorAccess(TBRGroupRoot groupRoot, GroupContainer groupContainer)
        {

        }

        public static async Task ExecuteMethod_StoreObjectsAsync(TBRGroupRoot groupRoot, GroupContainer groupContainer)
        {
            await groupRoot.StoreInformationAsync();
            await groupContainer.StoreInformationAsync();
        }

        public static void ExecuteMethod_FixContentTypesAndMetadataOfBlobs(IContainerOwner groupAsOwner)
        {
            throw new System.NotImplementedException();
        }

        public static void ExecuteMethod_FixRelativeLocationsOfInformationObjects(IContainerOwner groupAsOwner)
        {
            throw new NotImplementedException();
        }
    }
}