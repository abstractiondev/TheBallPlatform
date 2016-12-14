using System;
using System.Linq;
using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    partial class GroupMembership : IAdditionalFormatProvider
    {
        public const string InitiatorRoleValue = "Initiator";
        public const string ModeratorRoleValue = "Moderator";
        public const string CollaboratorRoleValue = "Collaborator";
        public const string ViewerRoleValue = "Viewer";

        private static string[] ValidRoles = new[]
        {
            InitiatorRoleValue,
            ModeratorRoleValue,
            CollaboratorRoleValue,
            ViewerRoleValue
        };

        public static bool IsValidRole(string role)
        {
            return ValidRoles.Contains(role);
        }

        public static bool HasInitiatorRights(string role)
        {
            return role.ToLower() == InitiatorRoleValue.ToLower();
        }

        public static bool HasModeratorRights(string role)
        {
            return role.ToLower() == ModeratorRoleValue.ToLower() || role.ToLower() == InitiatorRoleValue.ToLower();
        }

        public static bool HasCollaboratorRights(string role)
        {
            return role.ToLower() == ModeratorRoleValue.ToLower() || role.ToLower() == InitiatorRoleValue.ToLower() || role.ToLower() == CollaboratorRoleValue.ToLower();
        }

        public static bool HasViewerRights(string role)
        {
            return role.ToLower() == ModeratorRoleValue.ToLower() || role.ToLower() == InitiatorRoleValue.ToLower() || role.ToLower() == CollaboratorRoleValue.ToLower() ||
                   role.ToLower() == ViewerRoleValue.ToLower();
        }

        public static string GetIDFromAccountAndGroup(string accountId, string groupId)
        {
            return accountId + "_" + groupId;
        }
        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }

    }
}