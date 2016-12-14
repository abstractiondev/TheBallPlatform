using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class CreateGroupImplementation
    {
        public static Group GetTarget_GroupToBeCreated(string groupId)
        {
            Group group = new Group();
            if (groupId != null)
                group.ID = groupId;
            group.SetLocationAsOwnerContent(SystemSupport.SystemOwner, group.ID);
            return group;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Group groupToBeCreated)
        {
            await groupToBeCreated.StoreInformationAsync();
        }
    }
}