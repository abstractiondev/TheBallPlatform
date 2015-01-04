using AaltoGlobalImpact.OIP;

namespace TheBall.Payments
{
    public class RevokePaidAccessFromGroupImplementation
    {
        public static void ExecuteMethod_RemoveAccountFromGroup(string groupId, string accountId)
        {
            RemoveMemberFromGroup.Execute(new RemoveMemberFromGroupParameters
            {
                AccountID = accountId,
                GroupID = groupId
            });
        }
    }
}