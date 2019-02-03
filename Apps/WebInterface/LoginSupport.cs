using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AzureSupport;
using Konscious.Security.Cryptography;
using TheBall;
using TheBall.CORE;
using TheBall.CORE.INT;
using TheBall.CORE.Storage;

namespace WebInterface
{
    internal class LoginSupport
    {
        public static async Task HandleLoginOperationRequest(IContainerOwner containerOwner, HttpContext context, string operationName)
        {
            var request = context.Request;
            var response = context.Response;
            switch (operationName)
            {
                case "TheBall.Login.LoginOperation":
                    await performLogin(context, request, response);
                    break;
                case "TheBall.Login.SendEmailConfirmationCode":
                    await performConfirmCodeSend(request, response);
                    break;

                case "TheBall.Login.RegisterOperation":
                    await performRegistration(request, response);
                    break;
            }
        }

        private static async Task performConfirmCodeSend(HttpRequest request, HttpResponse response)
        {
            var loginInfo = JSONSupport.GetObjectFromStream<LoginInfo>(request.GetBufferedInputStream());
            var emailResult = await EnsureEmail.ExecuteAsync(new EnsureEmailParameters()
            {
                EmailAddress = loginInfo.EmailAddress
            });
            var email = emailResult.EnsuredEmail;
            await ActivateEmailValidation.ExecuteAsync(new ActivateEmailValidationParameters
            {
                Email = email,
                SendValidationCodeIfReissued = true,
                ResendValidationCode = false
            });
        }

        private static async Task performRegistration(HttpRequest request, HttpResponse response)
        {
            var registrationInfo =
                JSONSupport.GetObjectFromStream<ConfirmedLoginInfo>(request.GetBufferedInputStream());
            var emailResult = await EnsureEmail.ExecuteAsync(new EnsureEmailParameters
            {
                EmailAddress = registrationInfo.LoginInfo.EmailAddress
            });
            var email = emailResult.EnsuredEmail;
            if (registrationInfo.ConfirmationCode == email.ValidationKey) // TODO: Add expiration check here as well
            {
                var emailAddress = email.EmailAddress;
                var accountID = email.Account;
                Account account;
                var loginUrl = Login.GetLoginUrlFromEmailAddress(emailAddress);
                if (accountID == null)
                {
                    var accountResult = await CreateAccount.ExecuteAsync(new CreateAccountParameters
                    {
                        LoginUrl = loginUrl,
                        EmailAddress = email.EmailAddress,
                    });
                    account = accountResult.CreatedAccount;
                }
                else
                    account =
                        await ObjectStorage.RetrieveFromOwnerContentA<Account>(SystemSupport.SystemOwner, accountID);
                var loginID = Login.GetLoginIDFromLoginURL(loginUrl);
                if (!account.Logins.Contains(loginID))
                {
                    var ensuredLogin = (await EnsureLogin.ExecuteAsync(new EnsureLoginParameters
                    {
                        AccountID = accountID,
                        LoginURL = loginUrl
                    })).EnsuredLogin;
                    await AddLoginToAccount.ExecuteAsync(new AddLoginToAccountParameters
                    {
                        AccountID = accountID,
                        LoginUrl = loginUrl
                    });
                }
                var login = await ObjectStorage.RetrieveFromOwnerContentA<Login>(SystemSupport.SystemOwner, loginID);
                var salt = BCrypt.Net.BCrypt.GenerateSalt();
                var password = registrationInfo.LoginInfo.Password;
                var passwordHash = getPasswordHash(password, salt);
                login.PasswordSalt = salt;
                login.PasswordHash = passwordHash;
                await login.StoreInformationAsync();
                email = await ObjectStorage.RetrieveFromOwnerContentA<Email>(SystemSupport.SystemOwner, email.ID);
                email.PendingValidation = false;
                email.ValidationProcessExpiration = DateTime.UtcNow;
                email.ValidationKey = null;
                await email.StoreInformationAsync();
            }
            else
                throw new SecurityException("Invalid confirmation code");
        }

        private static string getPasswordHash(string password, string salt)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hash;
        }

        static string getPasswordHashx(string password, string salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var argon2 = new Argon2d(passwordBytes);
            argon2.Salt = Encoding.UTF8.GetBytes(salt);
            argon2.DegreeOfParallelism = 16;
            argon2.Iterations = 2;
            argon2.MemorySize = 8192;
            var hashedResult = argon2.GetBytes(128);
            var passwordHash = Convert.ToBase64String(hashedResult);
            return passwordHash;
        }

        private static async Task performLogin(HttpContext context, HttpRequest request, HttpResponse response)
        {
            var loginInfo = JSONSupport.GetObjectFromStream<LoginInfo>(request.GetBufferedInputStream());

            var emailAddress = loginInfo.EmailAddress;
            var password = loginInfo.Password;

            var loginUrl = Login.GetLoginUrlFromEmailAddress(emailAddress);
            var loginID = Login.GetLoginIDFromLoginURL(loginUrl);
            var login = await ObjectStorage.RetrieveFromSystemOwner<Login>(loginID);
            var salt = login.PasswordSalt;
            var accountID = login.Account;
            bool validLogin = BCrypt.Net.BCrypt.Verify(password, login.PasswordHash);
            if (validLogin)
            {
                var account = await ObjectStorage.RetrieveFromSystemOwner<Account>(accountID);
                string base64ClientMetadata = account.GetClientMetadataAsBase64();
                AuthenticationSupport.SetUserAuthentication(context, loginUrl, emailAddress, accountID, base64ClientMetadata);
            } else
                throw new SecurityException("Invalid login or password");
        }
    }
}