using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class JoinAccountToGroupImplementation
    {
        public static void ExecuteMethod_JoinAccountToGroup(string accountEmailAddress, string groupId, string memberRole)
        {
            InviteMemberToGroup.Execute(new InviteMemberToGroupParameters
            {
                DontSendEmailInvitation = true,
                GroupID = groupId,
                MemberEmailAddress = accountEmailAddress,
                MemberRole = memberRole
            });
            ConfirmInviteToJoinGroup.Execute(new ConfirmInviteToJoinGroupParameters
            {
                GroupID = groupId,
                MemberEmailAddress = accountEmailAddress
            });
        }
    }
}