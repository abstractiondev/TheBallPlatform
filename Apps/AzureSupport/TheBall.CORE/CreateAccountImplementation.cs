using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class CreateAccountImplementation
    {
        public static async Task<Login> GetTarget_LoginAsync(string loginUrl)
        {
            var loginID = Login.GetLoginIDFromLoginURL(loginUrl);
            var login = await ObjectStorage.RetrieveFromDefaultLocationA<Login>(loginID);
            return login;
        }

        public static async Task<Email> GetTarget_EmailAsync(string emailAddress)
        {
            var emailID = Email.GetIDFromEmailAddress(emailAddress);
            var email = await ObjectStorage.RetrieveFromDefaultLocationA<Email>(emailID);
            return email;
        }

        public static Account GetTarget_AccountToBeCreated(string accountId, Login login, Email email)
        {
            var account = new Account();
            if (accountId != null)
                account.ID = accountId;
            account.UpdateRelativeLocationFromID();
            account.Logins.Add(login.ID);
            account.Emails.Add(email.ID);
            return account;
        }

        public static void ExecuteMethod_SetAccountIDToLogin(Login login, Account accountToBeCreated)
        {
            login.Account = accountToBeCreated.ID;
        }

        public static void ExecuteMethod_SetAccountIDToEmail(Email email, Account accountToBeCreated)
        {
            email.Account = accountToBeCreated.ID;
        }

        public static async Task ExecuteMethod_StoreObjectsAsync(Account accountToBeCreated, Login login, Email email)
        {
            await
                Task.WhenAll(accountToBeCreated.StoreInformationAsync(), login.StoreInformationAsync(),
                    email.StoreInformationAsync());
        }

        public static CreateAccountReturnValue Get_ReturnValue(Account accountToBeCreated)
        {
            return new CreateAccountReturnValue
            {
                CreatedAccount = accountToBeCreated
            };
        }
    }
}