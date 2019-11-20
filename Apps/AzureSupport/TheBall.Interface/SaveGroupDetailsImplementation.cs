using System;
using System.Linq;
using System.Threading.Tasks;
using TheBall.Core;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class SaveGroupDetailsImplementation
    {
        public static async Task<Group> GetTarget_GroupAsync()
        {
            var currentOwner = InformationContext.CurrentOwner;
            var isGroupOwner = currentOwner.IsGroupContainer();
            if(!isGroupOwner)
                throw new InvalidOperationException("SaveGroupDetails only allowed for group owner");
            var groupID = currentOwner.GetIDFromLocationPrefix();
            var group = await ObjectStorage.RetrieveFromSystemOwner<Group>(groupID);
            return group;
        }

        public static async Task ExecuteMethod_SaveGroupDetailsAsync(GroupDetails groupDetails, Group group)
        {
            await ObjectStorage.StoreInterfaceObject(group, groupDetails);
        }

        public static async Task<GroupMembership[]> GetTarget_CurrentMembershipsAsync(Group group)
        {
            var fetchMembershipTasks = group.GroupMemberships.Select(async membershipID =>
            {
                var membership = await ObjectStorage.RetrieveFromSystemOwner<GroupMembership>(membershipID);
                return membership;
            }).ToArray();
            await Task.WhenAll(fetchMembershipTasks);
            var memberships = fetchMembershipTasks.Select(task => task.Result).ToArray();
            return memberships;
        }

        public static async Task ExecuteMethod_UpdateDetailsChangeToMembersAsync(Group group, GroupMembership[] currentMemberships)
        {
            await InformationContext.ExecuteAsOwnerAsync(SystemSupport.SystemOwner, async () =>
            {
                var groupID = group.ID;
                var accountIDs = currentMemberships.Select(item => item.Account).ToArray();
                var operationTasks = accountIDs.Select(accountID =>
                    UpdateAccountMembershipStatuses.ExecuteAsync(new UpdateAccountMembershipStatusesParameters
                    {
                        AccountID = accountID,
                        GroupID = groupID
                    }));
                await Task.WhenAll(operationTasks);
            });
        }

    }
}