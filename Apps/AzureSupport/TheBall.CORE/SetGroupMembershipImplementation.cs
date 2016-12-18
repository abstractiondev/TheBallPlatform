using System.IO;
using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class SetGroupMembershipImplementation
    {
        public static async Task<Account> GetTarget_AccountAsync(string accountId)
        {
            var account = await ObjectStorage.RetrieveFromOwnerContentA<Account>(SystemSupport.SystemOwner, accountId);
            return account;
        }

        public static async Task<Group> GetTarget_GroupAsync(string groupId)
        {
            var group = await ObjectStorage.RetrieveFromOwnerContentA<Group>(SystemSupport.SystemOwner, groupId);
            return group;
        }

        public static async Task<GroupMembership> GetTarget_GroupMembershipAsync(string accountId, string groupId)
        {
            var membershipID = GroupMembership.GetIDFromAccountAndGroup(accountId, groupId);
            var membership = await ObjectStorage.RetrieveFromOwnerContentA<GroupMembership>(SystemSupport.SystemOwner, membershipID);
            if (membership == null)
            {
                membership = new GroupMembership();
                membership.ID = membershipID;
                membership.SetLocationAsOwnerContent(SystemSupport.SystemOwner, membershipID);
            }
            membership.Account = accountId;
            membership.Group = groupId;
            return membership;
        }

        public static void ExecuteMethod_SetRoleToMembership(string role, GroupMembership groupMembership)
        {
            if (!GroupMembership.IsValidRole(role))
                throw new InvalidDataException("Invalid group membership role: " + role);
            groupMembership.Role = role;
        }

        public static void ExecuteMethod_SetMembershipToGroup(GroupMembership groupMembership, Group @group)
        {
            var membershipID = groupMembership.ID;
            if(!@group.GroupMemberships.Contains(membershipID))
                @group.GroupMemberships.Add(membershipID);
        }

        public static void ExecuteMethod_SetMembershipToAccount(GroupMembership groupMembership, Account account)
        {
            var membershipID = groupMembership.ID;
            if(!account.GroupMemberships.Contains(membershipID))
                account.GroupMemberships.Add(membershipID);
        }

        public static async Task ExecuteMethod_StoreObjectsAsync(GroupMembership groupMembership, Group @group, Account account)
        {
            await
                Task.WhenAll(groupMembership.StoreInformationAsync(), @group.StoreInformationAsync(),
                    account.StoreInformationAsync());
        }

        public static UpdateAccountMembershipStatusesParameters UpdateAccountStatuses_GetParameters(string accountId, string groupId)
        {
            return new UpdateAccountMembershipStatusesParameters
            {
                AccountID = accountId,
                GroupID = groupId
            };
        }

        public static UpdateGroupMembershipStatusesParameters UpdateGroupStatuses_GetParameters(string groupID, string accountID)
        {
            return new UpdateGroupMembershipStatusesParameters
            {
                GroupID = groupID,
                AccountID = accountID
            };
        }
    }
}