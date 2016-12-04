using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class AddLoginToAccountImplementation
    {
        public static async Task<Account> GetTarget_AccountAsync(string accountId)
        {
            var account = await ObjectStorage.RetrieveFromOwnerContentA<Account>(SystemSupport.SystemOwner, accountId);
            return account;
        }

        public static async Task<Login> GetTarget_LoginAsync(string loginUrl)
        {
            var loginID = Login.GetLoginIDFromLoginURL(loginUrl);
            var login = await ObjectStorage.RetrieveFromOwnerContentA<Login>(SystemSupport.SystemOwner, loginID);
            return login;
        }

        public static void ExecuteMethod_AddLoginToAccount(Account account, Login login)
        {
            var loginID = login.ID;
            if(!account.Logins.Contains(loginID))
                account.Logins.Add(loginID);
        }

        public static void ExecuteMethod_AddAccountToLogin(Login login, Account account)
        {
            login.Account = account.ID;
        }

        public static async Task ExecuteMethod_StoreObjectsAsync(Account account, Login login)
        {
            await Task.WhenAll(account.StoreInformationAsync(), login.StoreInformationAsync());
        }

    }
}