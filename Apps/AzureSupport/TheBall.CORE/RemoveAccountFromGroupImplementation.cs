using AaltoGlobalImpact.OIP;

namespace TheBall.CORE
{
    public class RemoveAccountFromGroupImplementation
    {
        public static void ExecuteMethod_RemoveAccountFromGroup(string accountEmailAddress, string accountId, string groupId)
        {
            RemoveMemberFromGroup.Execute(new RemoveMemberFromGroupParameters
            {
                AccountID = accountId,
                EmailAddress = accountEmailAddress,
                GroupID = groupId
            });
        }
    }
}