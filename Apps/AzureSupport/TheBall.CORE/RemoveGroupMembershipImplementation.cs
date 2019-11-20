using System.Threading.Tasks;

namespace TheBall.Core
{
    public class RemoveGroupMembershipImplementation
    {
        public static async Task<Account> GetTarget_AccountAsync(string accountId)
        {
            var account = await ObjectStorage.RetrieveFromDefaultLocationA<Account>(accountId);
            return account;
        }

        public static async Task<Group> GetTarget_GroupAsync(string groupId)
        {
            var group = await ObjectStorage.RetrieveFromDefaultLocationA<Group>(groupId);
            return group;
        }

        public static async Task<GroupMembership> GetTarget_GroupMembershipAsync(string accountId, string groupId)
        {
            var membershipID = GroupMembership.GetIDFromAccountAndGroup(accountId, groupId);
            var membership = await ObjectStorage.RetrieveFromDefaultLocationA<GroupMembership>(membershipID);
            return membership;
        }

        public static void ExecuteMethod_RemoveMembershipFromGroup(GroupMembership groupMembership, Group @group)
        {
            @group.GroupMemberships.RemoveAll(item => item == groupMembership.ID);
        }

        public static void ExecuteMethod_RemoveMembershipFromAccount(GroupMembership groupMembership, Account account)
        {
            account.GroupMemberships.RemoveAll(item => item == groupMembership.ID);
        }

        public static async Task ExecuteMethod_StoreObjectsAsync(Group @group, Account account)
        {
            await Task.WhenAll(@group.StoreInformationAsync(), account.StoreInformationAsync());
        }

        public static async Task ExecuteMethod_DeleteObjectAsync(GroupMembership groupMembership)
        {
            await groupMembership.DeleteInformationObjectAsync();
        }
    }
}