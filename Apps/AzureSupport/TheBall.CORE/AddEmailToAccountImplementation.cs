using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class AddEmailToAccountImplementation
    {
        public static async Task<Account> GetTarget_AccountAsync(string accountId)
        {
            var account = await ObjectStorage.RetrieveFromDefaultLocationA<Account>(accountId);
            return account;
        }

        public static async Task<Email> GetTarget_EmailAsync(string emailAddress)
        {
            var emailId = Email.GetIDFromEmailAddress(emailAddress);
            var email = await ObjectStorage.RetrieveFromDefaultLocationA<Email>(emailId);
            return email;
        }

        public static void ExecuteMethod_AddEmailToAccount(Account account, Email email)
        {
            var emailID = email.ID;
            if(!account.Emails.Contains(emailID))
                account.Emails.Add(emailID);
        }

        public static void ExecuteMethod_AddAccountToEmail(Email email, Account account)
        {
            var accountID = account.ID;
            email.Account = accountID;
        }

        public static async Task ExecuteMethod_StoreObjectsAsync(Account account, Email email)
        {
            await Task.WhenAll(account.StoreInformationAsync(), email.StoreInformationAsync());
        }
    }
}