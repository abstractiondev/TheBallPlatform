using System.Threading.Tasks;

namespace TheBall.Core
{
    public class RemoveEmailImplementation
    {
        public static async Task<Email> GetTarget_EmailAsync(string emailAddress)
        {
            var emailID = Email.GetIDFromEmailAddress(emailAddress);
            var email = await ObjectStorage.RetrieveFromDefaultLocationA<Email>(emailID);
            return email;
        }

        public static async Task<Account> GetTarget_AccountAsync(Email email)
        {
            var accountID = email.Account;
            var account = await ObjectStorage.RetrieveFromDefaultLocationA<Account>(accountID);
            return account;
        }

        public static void ExecuteMethod_RemoveEmailFromAccount(Account account, Email email)
        {
            account.Emails.RemoveAll(item => item == email.ID);
        }

        public static async Task ExecuteMethod_DeleteObjectAsync(Email email)
        {
            await email.DeleteInformationObjectAsync();
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Account account)
        {
            await account.StoreInformationAsync();
        }
    }
}