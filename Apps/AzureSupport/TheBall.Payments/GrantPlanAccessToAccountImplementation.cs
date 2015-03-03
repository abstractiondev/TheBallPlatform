using System;
using System.IO;
using AaltoGlobalImpact.OIP;

namespace TheBall.Payments
{
    public class GrantPlanAccessToAccountImplementation
    {
        public static GroupSubscriptionPlan GetTarget_GroupSubscriptionPlan(string planName)
        {
            var result = new GroupSubscriptionPlan
            {
                PlanName = planName,
                Description = planName
            };

            if (planName == "ONLINE")
            {
                //result.GroupIDs.Add("e710a1f8-94a3-4d38-85df-193936624ce4");
                //result.GroupIDs.Add("b22f0329-34f8-433d-bc44-b689627468cc");
                result.GroupIDs.Add("1b466a35-49ad-4608-949a-a1b029dc87f4");
            }

            return result;

            //return GroupSubscriptionPlan.RetrieveFromOwnerContent(InformationContext.CurrentOwner, planName);
        }

        public static void ExecuteMethod_GrantAccessToAccountForPlanGroups(string accountId, GroupSubscriptionPlan groupSubscriptionPlan)
        {
            foreach (var groupID in groupSubscriptionPlan.GroupIDs)
            {
                GrantPaidAccessToGroup.Execute(new GrantPaidAccessToGroupParameters
                {
                    AccountID = accountId,
                    GroupID = groupID
                });
            }
        }
    }
}