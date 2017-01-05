using System.Linq;
using System.Threading.Tasks;
using TheBall.Admin.INT;
using TheBall.CORE;
using TheBall.CORE.Storage;

namespace TheBall.Admin
{
    public class UpdateUsersDataImplementation
    {
        public static async Task<UsersData> GetTarget_UsersDataAsync()
        {
            var accountIDs = await ObjectStorage.ListOwnerObjectIDs<Account>(SystemSupport.SystemOwner);
            var accountObjectTasks = accountIDs.Select(async accountID =>
            {
                var account = await ObjectStorage.RetrieveFromSystemOwner<Account>(accountID);
                return account;
            }).ToArray();
            await Task.WhenAll(accountObjectTasks);
            var accounts = accountObjectTasks.Select(task => task.Result);

            var usersData = new UsersData();
            usersData.AccountInfos = accounts.Select(account => new AccountInfo
            {
                AccountID = account.ID,
                EmailAddress = Email.GetEmailAddressFromID(account.Emails.FirstOrDefault())
            }).ToArray();
            return usersData;
        }

        public static async Task ExecuteMethod_StoreInterfaceObjectAsync(UsersData usersData)
        {
            await ObjectStorage.StoreInterfaceObject(usersData, "UsersData", true);
        }
    }
}