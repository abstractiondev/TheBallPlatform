using System;
using System.IO;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class RegisterEmailAddressImplementation
    {
        public static void ExecuteMethod_ValidateUnexistingEmail(string emailAddress)
        {
            if (String.IsNullOrWhiteSpace(emailAddress))
                throw new InvalidDataException("Email address is required");
            string emailRootID = TBREmailRoot.GetIDFromEmailAddress(emailAddress);
            throw new NotImplementedException();
            //TBREmailRoot emailRoot = ObjectStorage.RetrieveFromDefaultLocation<TBREmailRoot>(emailRootID);
            //if (emailRoot != null)
            //    throw new InvalidDataException("Email address '" + emailAddress + "' is already registered to the system.");
        }

        public static TBRAccountRoot GetTarget_AccountRoot(string accountId)
        {
            throw new NotImplementedException();
            //TBRAccountRoot accountRoot = ObjectStorage.RetrieveFromDefaultLocation<TBRAccountRoot>(accountId);
            //return accountRoot;
        }

        public static TBREmailRoot GetTarget_EmailRoot(string emailAddress)
        {
            string emailRootID = TBREmailRoot.GetIDFromEmailAddress(emailAddress);
            TBREmailRoot emailRoot = TBREmailRoot.CreateDefault();
            emailRoot.ID = emailRootID;
            emailRoot.UpdateRelativeLocationFromID();
            return emailRoot;
        }

        public static void ExecuteMethod_AddEmailToAccount(string emailAddress, TBRAccountRoot accountRoot)
        {
            TBEmail email = TBEmail.CreateDefault();
            email.EmailAddress = emailAddress;
            email.ValidatedAt = DateTime.Now;
            accountRoot.Account.Emails.CollectionContent.Add(email);
        }

        public static void ExecuteMethod_AddAccountToEmailRoot(TBRAccountRoot accountRoot, TBREmailRoot emailRoot)
        {
            emailRoot.Account = accountRoot.Account;
        }

        public static void ExecuteMethod_StoreEmailRoot(TBREmailRoot emailRoot)
        {
            throw new NotImplementedException();
            //emailRoot.StoreInformation();
        }

        public static void ExecuteMethod_StoreAccountRoot(TBRAccountRoot accountRoot)
        {
            throw new NotImplementedException();
            //accountRoot.StoreInformation();
        }

        public static void ExecuteMethod_UpdateAccountRootAndContainerWithChanges(string accountID)
        {
            /*
            UpdateAccountRootToReferences.Execute(new UpdateAccountRootToReferencesParameters
            {
                AccountID = accountID
            });
            UpdateAccountContainerFromAccountRoot.Execute(new UpdateAccountContainerFromAccountRootParameters
            {
                AccountID = accountID
            });
            */
            throw new NotImplementedException();
        }
    }
}