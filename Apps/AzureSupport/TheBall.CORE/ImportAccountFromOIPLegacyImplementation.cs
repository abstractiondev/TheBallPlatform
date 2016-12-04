using System.Collections.Generic;
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

        public static async Task ExecuteMethod_AddMissingLoginsAsync(Account account, TBAccount legacyAccount)
        {
            var legacyLoginUrls = legacyAccount.Logins.CollectionContent.Select(login => login.OpenIDUrl).ToArray();
            List<Login> logins = new List<Login>();
            foreach(var loginUrl in legacyLoginUrls)
            {
                var ensureOpResult = await EnsureLogin.ExecuteAsync(new EnsureLoginParameters
                {
                    AccountID = account.ID,
                    LoginURL = loginUrl
                });
                var login = ensureOpResult.EnsuredLogin;
                if (login.Account != account.ID)
                {
                    login.Account = account.ID;
                    await login.StoreInformationAsync();
                }
                logins.Add(login);
            }
            account.Logins = account.Logins.Union(logins.Select(login => login.ID)).ToList();
        }

        public static async Task ExecuteMethod_AddMissingEmailsAsync(Account account, TBAccount legacyAccount)
        {
            var legacyEmailAddresses =
                legacyAccount.Emails.CollectionContent.Select(email => email.EmailAddress.ToLower()).ToArray();
            List<Email> emails = new List<Email>();
            foreach (var emailAddress in legacyEmailAddresses)
            {
                var ensureOpResult = await EnsureEmail.ExecuteAsync(new EnsureEmailParameters
                {
                    AccountID = account.ID,
                    EmailAddress = emailAddress
                });
                var email = ensureOpResult.EnsuredEmail;
                if (email.Account != account.ID)
                {
                    email.Account = account.ID;
                    await email.StoreInformationAsync();
                }
                emails.Add(ensureOpResult.EnsuredEmail);
            }
            account.Emails = account.Emails.Union(emails.Select(email => email.ID)).ToList();
        }

        public static ImportAccountFromOIPLegacyReturnValue Get_ReturnValue(Account account)
        {
            return new ImportAccountFromOIPLegacyReturnValue {ImportedAccount = account};
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Account account)
        {
            await account.StoreInformationAsync();
        }
    }
}