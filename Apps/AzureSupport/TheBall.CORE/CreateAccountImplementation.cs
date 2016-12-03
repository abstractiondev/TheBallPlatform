using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class CreateAccountImplementation
    {
        public static Account GetTarget_AccountToBeCreated(string accountId)
        {
            var account = new Account();
            if (accountId != null)
                account.ID = accountId;
            account.UpdateRelativeLocationFromID();
            account.SetLocationAsOwnerContent(SystemSupport.SystemOwner, account.ID);
            return account;
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

        public static EnsureLoginParameters Login_GetParameters(string loginUrl, Account accountToBeCreated)
        {
            return new EnsureLoginParameters
            {
                AccountID = accountToBeCreated.ID,
                LoginURL = loginUrl
            };
        }

        public static Login Login_GetOutput(EnsureLoginReturnValue operationReturnValue, string loginUrl, Account accountToBeCreated)
        {
            return operationReturnValue.EnsuredLogin;
        }

        public static EnsureEmailParameters Email_GetParameters(string emailAddress, Account accountToBeCreated)
        {
            return new EnsureEmailParameters
            {
                AccountID = accountToBeCreated.ID,
                EmailAddress = emailAddress
            };
        }

        public static Email Email_GetOutput(EnsureEmailReturnValue operationReturnValue, string emailAddress, Account accountToBeCreated)
        {
            return operationReturnValue.EnsuredEmail;
        }

        public static void ExecuteMethod_ConnectLoginAndAccount(Account accountToBeCreated, Login loginOutput)
        {
            if(!accountToBeCreated.Logins.Contains(loginOutput.ID))
                accountToBeCreated.Logins.Add(loginOutput.ID);
            loginOutput.Account = accountToBeCreated.ID;
        }

        public static void ExecuteMethod_ConnectEmailAndAccount(Account accountToBeCreated, Email emailOutput)
        {
            if(!accountToBeCreated.Emails.Contains(emailOutput.ID))
                accountToBeCreated.Emails.Add(emailOutput.ID);
            emailOutput.Account = accountToBeCreated.ID;
        }
    }
}