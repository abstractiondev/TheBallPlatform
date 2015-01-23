using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class JoinUserToGroupImplementation
    {
        public static void ExecuteMethod_JoinUserToGroup(string userEmailAddress, string groupId, string memberRole)
        {
            InviteMemberToGroup.Execute(new InviteMemberToGroupParameters
            {
                DontSendEmailInvitation = true,
                GroupID = groupId,
                MemberEmailAddress = userEmailAddress,
                MemberRole = memberRole
            });
            ConfirmInviteToJoinGroup.Execute(new ConfirmInviteToJoinGroupParameters
            {
                GroupID = groupId,
                MemberEmailAddress = userEmailAddress
            });
        }
    }
}