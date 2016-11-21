using System.Linq;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class ImportAccountFromOIPLegacyImplementation
    {
        public static async Task<TBRAccountRoot> GetTarget_LegacyAccountRootAsync(TBRLoginRoot legacyLogin)
        {
            var accountID = legacyLogin.Account.ID;
            var legacyAccountRoot = await ObjectStorage.RetrieveFromDefaultLocationA<TBRAccountRoot>(accountID);
            return legacyAccountRoot;
        }
        public static TBAccount GetTarget_LegacyAccount(TBRAccountRoot legacyAccountRoot)
        {
            return legacyAccountRoot.Account;
        }

        public static async Task<Account> GetTarget_AccountAsync(TBAccount legacyAccount)
        {
            var accountID = legacyAccount.ID;
            var account = await ObjectStorage.RetrieveFromOwnerContentA<Account>(SystemOwner.CurrentSystem, accountID);
            if (account == null)
            {
                var firstLogin = legacyAccount.Logins.CollectionContent.FirstOrDefault();
                var firstEmail = legacyAccount.Emails.CollectionContent.FirstOrDefault();
                account = (await CreateAccount.ExecuteAsync(new CreateAccountParameters
                {
                    AccountID = accountID,
                    LoginUrl = firstLogin?.OpenIDUrl,
                    EmailAddress = firstEmail?.EmailAddress
                })).CreatedAccount;
            }
            return account;
        }

        public static Task ExecuteMethod_AddMissingLoginsAsync(Account account, TBAccount legacyAccount)
        {
            throw new System.NotImplementedException();
        }

        public static Task ExecuteMethod_AddMissingEmailsAsync(Account account, TBAccount legacyAccount)
        {
            throw new System.NotImplementedException();
        }

        public static ImportAccountFromOIPLegacyReturnValue Get_ReturnValue(Account account)
        {
            return new ImportAccountFromOIPLegacyReturnValue {ImportedAccount = account};
        }

    }
}