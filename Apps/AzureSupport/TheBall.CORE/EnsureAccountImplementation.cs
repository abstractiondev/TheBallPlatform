using System.Linq;
using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class EnsureAccountImplementation
    {
        public static EnsureEmailParameters EnsureEmail_GetParameters(string emailAddress)
        {
            return new EnsureEmailParameters
            {
                EmailAddress = emailAddress
            };
        }

        public static Email EnsureEmail_GetOutput(EnsureEmailReturnValue operationReturnValue, string emailAddress)
        {
            return operationReturnValue.EnsuredEmail;
        }

        public static async Task<Account> GetTarget_ExistingAccountAsync(Email ensureEmailOutput)
        {
            var existingAccountID = ensureEmailOutput.Account;
            if (existingAccountID == null)
                return null;
            return await ObjectStorage.RetrieveFromOwnerContentA<Account>(SystemSupport.SystemOwner, existingAccountID);
        }

        public static async Task<Account> GetTarget_ResultingAccountAsync(string loginUrl, Email ensureEmailOutput, Account existingAccount)
        {
            Account resultingAccount;
            if (existingAccount != null)
            {
                resultingAccount = existingAccount;
                var accountID = resultingAccount.ID;
                var loginID = Login.GetLoginIDFromLoginURL(loginUrl);
                bool hasLoginDefined = resultingAccount.Logins.Any(login => login == loginID);
                if (!hasLoginDefined)
                {
                    var ensuredLoginResult = await EnsureLogin.ExecuteAsync(new EnsureLoginParameters
                    {
                        AccountID = accountID,
                        LoginURL = loginUrl
                    });
                    var login = ensuredLoginResult.EnsuredLogin;
                    login.Account = accountID;
                    await login.StoreInformationAsync();
                    resultingAccount.Logins.Add(loginID);
                    await resultingAccount.StoreInformationAsync();
                }
            }
            else
            {
                var emailAddress = ensureEmailOutput.EmailAddress;
                var createResult = await CreateAccount.ExecuteAsync(new CreateAccountParameters
                {
                    EmailAddress = emailAddress,
                    LoginUrl = loginUrl
                });
                resultingAccount = createResult.CreatedAccount;
            }
            return resultingAccount;
        }

        public static EnsureAccountReturnValue Get_ReturnValue(Account resultingAccount)
        {
            return new EnsureAccountReturnValue
            {
                EnsuredAccount = resultingAccount
            };
        }
    }
}