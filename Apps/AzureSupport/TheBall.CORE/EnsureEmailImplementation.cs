using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheBall.Core
{
    public class EnsureEmailImplementation
    {
        public static async Task<Email> GetTarget_EmailAsync(string emailAddress)
        {
            var emailID = Email.GetIDFromEmailAddress(emailAddress);
            var email = await ObjectStorage.RetrieveFromDefaultLocationA<Email>(emailID, SystemOwner.CurrentSystem);
            if (email == null)
            {
                email = new Email();
                email.ID = emailID;
                email.EmailAddress = emailAddress;
                email.SetLocationAsOwnerContent(SystemOwner.CurrentSystem, emailID);
                await email.StoreInformationAsync();
            }
            return email;
        }

        public static void ExecuteMethod_ValidateExistingAccountIDToMatch(string accountId, Email email)
        {
            if (accountId != null)
            {
                if (email.Account != null && email.Account != accountId)
                    throw new SecurityException("Unexpected email account association: " + accountId + " expected, got " + email.Account);
            }
        }

        public static EnsureEmailReturnValue Get_ReturnValue(Email email)
        {
            return new EnsureEmailReturnValue
            {
                EnsuredEmail = email
            };
        }

        public static void ExecuteMethod_ValidateEmailAddress(string emailAddress)
        {
            var regex = new Regex(@"^[^@]+@[^@]+\.[^@]+$");
            if (!regex.IsMatch(emailAddress))
                throw new InvalidDataException("Not accepted email address: " + emailAddress);
        }
    }
}