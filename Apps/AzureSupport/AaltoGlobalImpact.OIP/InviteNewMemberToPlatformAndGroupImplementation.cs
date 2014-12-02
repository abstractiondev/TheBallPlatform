using System;
using System.IO;
using System.Linq;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public class InviteNewMemberToPlatformAndGroupImplementation
    {
        public static void ExecuteMethod_ValidateThatEmailAddressIsNew(string memberEmailAddress)
        {
            var emailRootID = TBREmailRoot.GetIDFromEmailAddress(memberEmailAddress);
            var emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
            if(emailRoot != null)
                throw new InvalidDataException("Email is already registered in the platform");
        }

        public static TBRGroupRoot GetTarget_GroupRoot(string groupId)
        {
            return TBRGroupRoot.RetrieveFromDefaultLocation(groupId);
        }

        public static TBEmailValidation GetTarget_EmailValidation(string memberEmailAddress, string groupId)
        {
            TBEmailValidation emailValidation = new TBEmailValidation();
            emailValidation.Email = memberEmailAddress;
            emailValidation.ValidUntil = DateTime.UtcNow.AddDays(14); // Two weeks to accept the group join
            emailValidation.GroupJoinConfirmation = new TBGroupJoinConfirmation
            {
                GroupID = groupId,
                InvitationMode = "PLATFORM"
            };
            return emailValidation;
        }

        public static void ExecuteMethod_AddAsPendingInvitationToGroupRoot(string memberEmailAddress, TBRGroupRoot groupRoot)
        {
            TBCollaboratorRole role =
                groupRoot.Group.Roles.CollectionContent.FirstOrDefault(
                    candidate => candidate.Email.EmailAddress == memberEmailAddress);
            if (role != null)
            {
                if (role.IsRoleStatusValidMember())
                    throw new InvalidDataException("Person to be invited is already member of the group");
            }
            else
            {
                role = TBCollaboratorRole.CreateDefault();
                role.Email.EmailAddress = memberEmailAddress;
                role.Role = TBCollaboratorRole.CollaboratorRoleValue;
                role.SetRoleAsInvited();
                groupRoot.Group.Roles.CollectionContent.Add(role);
            }
        }

        public static void ExecuteMethod_StoreObjects(TBRGroupRoot groupRoot, TBEmailValidation emailValidation)
        {
            groupRoot.StoreInformation();
            emailValidation.StoreInformation();
        }

        public static void ExecuteMethod_SendEmailConfirmation(TBEmailValidation emailValidation, TBRGroupRoot groupRoot)
        {
            EmailSupport.SendGroupAndPlatformJoinEmail(emailValidation, groupRoot.Group);
        }
    }
}