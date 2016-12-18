using System.Linq;
using System.Threading.Tasks;
using TheBall.Interface.INT;

namespace TheBall.CORE
{
    public class UpdateAccountMembershipStatusesImplementation
    {
        public static async Task<Account> GetTarget_AccountAsync(string accountID)
        {
            var account = await ObjectStorage.RetrieveFromSystemOwner<Account>(accountID);
            return account;
        }

        public static async Task<GroupMembership[]> GetTarget_MembershipsAsync(Account account)
        {
            var membershipIDs = account.GroupMemberships;
            var membershipFetchTasks =
                membershipIDs.Select(
                    membershipID => ObjectStorage.RetrieveFromSystemOwner<GroupMembership>(membershipID)).ToArray();
            await Task.WhenAll(membershipFetchTasks);
            var memberships = membershipFetchTasks.Select(task => task.Result).ToArray();
            return memberships;
        }

        public static async Task<AccountMembershipData> GetTarget_AccountMembershipDataAsync(Account account)
        {
            var membershipData = await ObjectStorage.GetInterfaceObject<AccountMembershipData>();
            if (membershipData == null)
            {
                membershipData = new AccountMembershipData();
                membershipData.Memberships = new AccountMembershipItem[0];
            }
            return membershipData;
        }

        public static async Task ExecuteMethod_UpdateMembershipDataAsync(string groupId, AccountMembershipData accountMembershipData, GroupMembership[] memberships)
        {
            var existingStatusesDict = accountMembershipData.Memberships.ToDictionary(item => item.GroupID);
            var currentMembershipData = memberships.Select(item => new AccountMembershipItem
            {
                GroupID = item.Group,
                Role = item.Role,
                Details = existingStatusesDict.ContainsKey(item.Group) ? existingStatusesDict[item.Group].Details : null
            }).ToArray();
            var updateGroupDetails =
                currentMembershipData.Where(item => item.Details == null || item.GroupID == groupId).ToArray();
            var updateTasks = updateGroupDetails.Select(async item =>
            {
                var groupAsOwner = VirtualOwner.FigureOwner("grp/" + item.GroupID);
                var groupDetails = await ObjectStorage.GetInterfaceObject<GroupDetails>(groupAsOwner);
                item.Details = groupDetails;
            }).ToArray();
            await Task.WhenAll(updateTasks);
            accountMembershipData.Memberships = currentMembershipData;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(AccountMembershipData accountMembershipData)
        {
            await ObjectStorage.StoreInterfaceObject(accountMembershipData);
        }

    }
}