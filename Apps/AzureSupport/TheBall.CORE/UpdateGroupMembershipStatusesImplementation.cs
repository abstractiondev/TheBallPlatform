using System.Linq;
using System.Threading.Tasks;
using TheBall.Interface.INT;

namespace TheBall.CORE
{
    public class UpdateGroupMembershipStatusesImplementation
    {
        public static async Task<Group> GetTarget_GroupAsync(string groupID)
        {
            var group = await ObjectStorage.RetrieveFromSystemOwner<Group>(groupID);
            return group;
        }

        public static async Task<GroupMembership[]> GetTarget_MembershipsAsync(Group @group)
        {
            var membershipIDs = @group.GroupMemberships;
            var membershipFetchTasks =
                membershipIDs.Select(
                    membershipID => ObjectStorage.RetrieveFromSystemOwner<GroupMembership>(membershipID)).ToArray();
            await Task.WhenAll(membershipFetchTasks);
            var memberships = membershipFetchTasks.Select(task => task.Result).ToArray();
            return memberships;
        }

        public static async Task<GroupMembershipData> GetTarget_GroupMembershipDataAsync(Group @group)
        {
            var membershipData = await ObjectStorage.GetInterfaceObject<GroupMembershipData>(@group);
            if (membershipData == null)
            {
                membershipData = new GroupMembershipData();
                membershipData.Memberships = new GroupMembershipItem[0];
            }
            return membershipData;
        }

        public static async Task ExecuteMethod_UpdateMembershipDataAsync(string accountID, GroupMembershipData groupMembershipData, GroupMembership[] memberships)
        {
            var existingStatusesDict = groupMembershipData.Memberships.ToDictionary(item => item.AccountID);
            var currentMembershipData = memberships.Select(item => new GroupMembershipItem
            {
                AccountID = item.Account,
                Role = item.Role,
                Details = existingStatusesDict.ContainsKey(item.Account) ? existingStatusesDict[item.Account].Details : null
            }).ToArray();
            var updateGroupDetails =
                currentMembershipData.Where(item => item.Details == null || item.AccountID == accountID).ToArray();
            var updateTasks = updateGroupDetails.Select(async item =>
            {
                var account = await ObjectStorage.RetrieveFromSystemOwner<Account>(item.AccountID);
                var emailID = account.Emails.FirstOrDefault();
                var emailAddress = emailID != null ? Email.GetEmailAddressFromID(emailID) : null;
                item.Details = new AccountDetails
                {
                    EmailAddress = emailAddress
                };
            }).ToArray();
            await Task.WhenAll(updateTasks);
            groupMembershipData.Memberships = currentMembershipData;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Group @group, GroupMembershipData groupMembershipData)
        {
            await ObjectStorage.StoreInterfaceObject(@group, groupMembershipData);
        }
    }
}