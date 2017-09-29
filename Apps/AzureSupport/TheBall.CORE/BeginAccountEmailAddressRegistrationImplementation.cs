using System;
using System.IO;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class BeginAccountEmailAddressRegistrationImplementation
    {
        public static async Task ExecuteMethod_ValidateUnexistingEmailAsync(string emailAddress)
        {
            if(String.IsNullOrWhiteSpace(emailAddress))
                throw new InvalidDataException("Email address is required");
            string emailRootID = TBREmailRoot.GetIDFromEmailAddress(emailAddress);
            TBREmailRoot emailRoot = await ObjectStorage.RetrieveFromDefaultLocationA<TBREmailRoot>(emailRootID);
            if (emailRoot != null)
                throw new InvalidDataException("Email address '" + emailAddress + "' is already registered to the system.");
        }

        public static TBEmailValidation GetTarget_EmailValidation(string accountID, string emailAddress, string redirectUrlAfterValidation)
        {
            TBEmailValidation emailValidation = new TBEmailValidation();
            emailValidation.AccountID = accountID;
            emailValidation.Email = emailAddress;
            emailValidation.RedirectUrlAfterValidation = redirectUrlAfterValidation;
            emailValidation.ValidUntil = DateTime.UtcNow.AddMinutes(30);
            return emailValidation;
        }

        public static async Task ExecuteMethod_StoreObjectAsync(TBEmailValidation emailValidation)
        {
            await emailValidation.StoreInformationAsync();
        }

        public static void ExecuteMethod_SendEmailConfirmation(TBEmailValidation emailValidation)
        {
            EmailSupport.SendValidationEmail(emailValidation);
        }
    }
}