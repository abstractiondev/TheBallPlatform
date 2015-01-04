namespace TheBall.Payments
{
    public class RevokePlanAccessFromAccountImplementation
    {
        public static GroupSubscriptionPlan GetTarget_GroupSubscriptionPlan(string planName)
        {
            return GroupSubscriptionPlan.RetrieveFromOwnerContent(InformationContext.CurrentOwner, planName);
        }

        public static void ExecuteMethod_RevokeAccessFromAccountForPlanGroups(string accountId, GroupSubscriptionPlan groupSubscriptionPlan)
        {
            foreach (var groupID in groupSubscriptionPlan.GroupIDs)
            {
                RevokePaidAccessFromGroup.Execute(new RevokePaidAccessFromGroupParameters
                {
                    AccountID = accountId,
                    GroupID = groupID
                });
            }
        }
    }
}