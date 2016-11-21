using System.Security;
using System.Threading.Tasks;

namespace TheBall.CORE
{
    public class EnsureLoginImplementation
    {
        public static async Task<Login> GetTarget_LoginAsync(string loginUrl)
        {
            var loginID = Login.GetLoginIDFromLoginURL(loginUrl);
            var login = await ObjectStorage.RetrieveFromDefaultLocationA<Login>(loginID, SystemOwner.CurrentSystem);
            if (login == null)
            {
                login = new Login();
                login.ID = loginID;
                login.SetLocationAsOwnerContent(SystemOwner.CurrentSystem, loginID);
                await login.StoreInformationAsync();
            }
            return login;
        }

        public static void ExecuteMethod_ValidateExistingAccountIDToMatch(string accountId, Login login)
        {
            if (accountId != null)
            {
                if(login.Account != null && login.Account != accountId)
                    throw new SecurityException("Unexpected login account association: " + accountId + " expected, got " + login.Account);
            }
        }

        public static EnsureLoginReturnValue Get_ReturnValue(Login login)
        {
            return new EnsureLoginReturnValue
            {
                EnsuredLogin = login
            };
        }
    }
}