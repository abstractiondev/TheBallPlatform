using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class RemoveLoginImplementation
    {
        public static async Task<Login> GetTarget_LoginAsync(string loginUrl)
        {
            var loginID = Login.GetLoginIDFromLoginURL(loginUrl);
            var login = await ObjectStorage.RetrieveFromDefaultLocationA<Login>(loginID);
            return login;
        }

        public static async Task<Account> GetTarget_AccountAsync(Login login)
        {
            var accountID = login.Account;
            var account = await ObjectStorage.RetrieveFromDefaultLocationA<Account>(accountID);
            return account;
        }

        public static void ExecuteMethod_RemoveLoginFromAccount(Account account, Login login)
        {
            account.Logins.RemoveAll(item => item == login.ID);
        }

        public static async Task ExecuteMethod_DeleteObjectAsync(Login login)
        {
            await login.DeleteInformationObjectAsync();
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Account account)
        {
            await account.StoreInformationAsync();
        }
    }
}