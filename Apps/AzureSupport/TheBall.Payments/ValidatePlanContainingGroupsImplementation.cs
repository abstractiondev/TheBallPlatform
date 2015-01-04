using System;
using System.IO;

namespace TheBall.Payments
{
    public class ValidatePlanContainingGroupsImplementation
    {
        public static GroupSubscriptionPlan GetTarget_GroupSubscriptionPlan(string planName)
        {
            var groupSubscriptionPlan = GroupSubscriptionPlan.RetrieveFromOwnerContent(InformationContext.CurrentOwner,
                planName);
            if(groupSubscriptionPlan == null)
                throw new InvalidDataException("Cannot find GroupSubscriptionPlan with name: " + planName);
            return groupSubscriptionPlan;
        }

        public static void ExecuteMethod_ValidateGroupsInPlan(GroupSubscriptionPlan groupSubscriptionPlan)
        {
            if(groupSubscriptionPlan.GroupIDs == null || groupSubscriptionPlan.GroupIDs.Count == 0)
                throw new InvalidDataException("GroupSubscriptionPlan requires at least one grouID - plan: " + groupSubscriptionPlan.PlanName);
        }
    }
}