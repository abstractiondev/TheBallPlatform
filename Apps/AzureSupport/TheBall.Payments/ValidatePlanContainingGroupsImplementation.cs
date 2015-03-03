using System;
using System.IO;

namespace TheBall.Payments
{
    public class ValidatePlanContainingGroupsImplementation
    {
        public static GroupSubscriptionPlan GetTarget_GroupSubscriptionPlan(string planName)
        {
            // Hard coded prototype of local-version operation-modules
            var result = new GroupSubscriptionPlan
            {
                PlanName = planName,
                Description = planName
            };

            if (planName == "ONLINE")
            {
                result.GroupIDs.Add("1b466a35-49ad-4608-949a-a1b029dc87f4");
                //result.GroupIDs.Add("b22f0329-34f8-433d-bc44-b689627468cc");
            } 

            return result;

            var groupSubscriptionPlan = GroupSubscriptionPlan.RetrieveFromOwnerContent(InformationContext.CurrentOwner,
                planName);
            if(groupSubscriptionPlan == null)
                throw new InvalidDataException("Cannot find GroupSubscriptionPlan with name: " + planName);
            return groupSubscriptionPlan;
        }

        public static void ExecuteMethod_ValidateGroupsInPlan(GroupSubscriptionPlan groupSubscriptionPlan)
        {
            if(groupSubscriptionPlan.GroupIDs == null || groupSubscriptionPlan.GroupIDs.Count == 0)
                throw new InvalidDataException("GroupSubscriptionPlan requires at least one groupID - plan: " + groupSubscriptionPlan.PlanName);
        }
    }
}