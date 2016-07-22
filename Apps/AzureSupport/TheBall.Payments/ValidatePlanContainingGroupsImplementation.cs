using System;
using System.IO;

namespace TheBall.Payments
{
    public class ValidatePlanContainingGroupsImplementation
    {
        public static GroupSubscriptionPlan GetTarget_GroupSubscriptionPlan(string planName)
        {
            var result = new GroupSubscriptionPlan
            {
                PlanName = planName,
                Description = planName
            };

            // TODO: Fetch groups mapped to plan names...
            return result;
        }

        public static void ExecuteMethod_ValidateGroupsInPlan(GroupSubscriptionPlan groupSubscriptionPlan)
        {
            if(groupSubscriptionPlan.GroupIDs == null || groupSubscriptionPlan.GroupIDs.Count == 0)
                throw new InvalidDataException("GroupSubscriptionPlan requires at least one groupID - plan: " + groupSubscriptionPlan.PlanName);
        }
    }
}