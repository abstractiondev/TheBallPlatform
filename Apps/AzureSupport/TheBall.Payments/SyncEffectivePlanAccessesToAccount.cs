using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using Stripe;

namespace TheBall.Payments
{
    public class SyncEffectivePlanAccessesToAccountImplementation
    {
        public static GroupSubscriptionPlan GetGroupSubscriptionPlan(string planID)
        {
            var result = new GroupSubscriptionPlan
            {
                PlanName = planID,
                Description = planID
            };

            if (planID == "ONLINE")
            {
                //result.GroupIDs.Add("e710a1f8-94a3-4d38-85df-193936624ce4");
                //result.GroupIDs.Add("b22f0329-34f8-433d-bc44-b689627468cc"); // test.theball.me
                result.GroupIDs.Add("1b466a35-49ad-4608-949a-a1b029dc87f4"); // members.onlinetaekwondo.net
            }

            return result;

            //return GroupSubscriptionPlan.RetrieveFromOwnerContent(InformationContext.CurrentOwner, planID);
        }

        public static async Task<CustomerAccount> GetTarget_AccountAsync(string accountID)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<CustomerAccount>(InformationContext.CurrentOwner, accountID);
        }

        public static GroupSubscriptionPlan[] GetTarget_CurrentPlansBeforeSync(CustomerAccount account)
        {
            var subscriptionPlanStatusIDs = account.ActivePlans;
            var planStatuses =
                subscriptionPlanStatusIDs.Select(
                    planStatusID =>
                        ObjectStorage.RetrieveFromOwnerContent<SubscriptionPlanStatus>(InformationContext.CurrentOwner, planStatusID))
                    .Where(status => status != null).ToArray();
            var plans = planStatuses.Select(planStatus => GetGroupSubscriptionPlan(planStatus.SubscriptionPlan)).ToArray();
            return plans;
        }

        public static GroupSubscriptionPlan[] GetTarget_ActivePlansFromStripe(CustomerAccount account)
        {
            StripeCustomerService customerService = new StripeCustomerService();
            StripeCustomer stripeCustomer = customerService.Get(account.StripeID);
            if (stripeCustomer == null)
                return new GroupSubscriptionPlan[0];
            var stripePlans =
                stripeCustomer.StripeSubscriptionList.Data.Select(stripeSub => stripeSub.StripePlan)
                    .ToArray();
            var plans = stripePlans.Select(stripePlan => GetGroupSubscriptionPlan(stripePlan.Id)).ToArray();
            return plans;
        }

        public static string[] GetTarget_GroupsToHaveAccessTo(GroupSubscriptionPlan[] activePlansFromStripe)
        {
            var groupIDs = activePlansFromStripe.SelectMany(plan => plan.GroupIDs).Distinct().ToArray();
            return groupIDs;
        }

        public static string[] GetTarget_CurrentGroupAccesses(GroupSubscriptionPlan[] currentPlansBeforeSync)
        {
            var groupIDs = currentPlansBeforeSync.SelectMany(plan => plan.GroupIDs).Distinct().ToArray();
            return groupIDs;
        }

        public static string[] GetTarget_GroupsToAddAccessTo(string[] groupsToHaveAccessTo, string[] currentGroupAccesses)
        {
            var groupsToAdd = groupsToHaveAccessTo.Except(currentGroupAccesses).ToArray();
            return groupsToAdd;
        }

        public static string[] GetTarget_GroupsToRevokeAccessFrom(string[] groupsToHaveAccessTo, string[] currentGroupAccesses)
        {
            var groupsToRemove = currentGroupAccesses.Except(groupsToHaveAccessTo).ToArray();
            return groupsToRemove;
        }

        public static void ExecuteMethod_GrantAccessToGroups(string accountID, string[] groupsToAddAccessTo)
        {
            foreach (var groupID in groupsToAddAccessTo)
            {
                try
                {
                    GrantPaidAccessToGroup.Execute(new GrantPaidAccessToGroupParameters
                    {
                        AccountID = accountID,
                        GroupID = groupID
                    });
                }
                catch (Exception exception)
                {
                    ErrorSupport.ReportException(exception);
                }
            }
        }

        public static void ExecuteMethod_RevokeAccessFromGroups(string accountID, string[] groupsToRevokeAccessFrom)
        {
            foreach (var groupID in groupsToRevokeAccessFrom)
            {
                try
                {
                    RevokePaidAccessFromGroup.Execute(new RevokePaidAccessFromGroupParameters
                    {
                        AccountID = accountID,
                        GroupID = groupID
                    });
                }
                catch (Exception exception)
                {
                    ErrorSupport.ReportException(exception);
                }
            }
        }

        public static void ExecuteMethod_SyncCurrentStripePlansToAccount(CustomerAccount account, GroupSubscriptionPlan[] currentPlansBeforeSync, GroupSubscriptionPlan[] activePlansFromStripe)
        {
            var activePlanIDs = activePlansFromStripe.Select(plan => plan.PlanName).ToArray();

            var currentStatusIDs = account.ActivePlans;
            var currentStatuses =
                currentStatusIDs.Select(
                    statusID =>
                        ObjectStorage.RetrieveFromOwnerContent<SubscriptionPlanStatus>(InformationContext.CurrentOwner, statusID))
                    .Where(status => status != null).ToArray();
            var statusesToRemove =
                currentStatuses.Where(status => activePlanIDs.All(planID => planID != status.SubscriptionPlan))
                    .ToArray();
            var plansToAddStatusesFor =
                activePlanIDs.Where(planID => currentStatuses.All(status => status.SubscriptionPlan != planID))
                    .ToArray();
            foreach(var status in statusesToRemove)
                status.DeleteInformationObject(InformationContext.CurrentOwner);
            List<string> addedStatusIDs = new List<string>();
            foreach (var planToAddStatus in plansToAddStatusesFor)
            {
                SubscriptionPlanStatus planStatus = new SubscriptionPlanStatus();
                planStatus.SetLocationAsOwnerContent(InformationContext.CurrentOwner, planStatus.ID);
                planStatus.SubscriptionPlan = planToAddStatus;
                planStatus.StoreInformation(InformationContext.CurrentOwner);
                addedStatusIDs.Add(planStatus.ID);
            }

            //account.ActivePlans.Clear();
            account.ActivePlans.RemoveAll(
                removedPlan => statusesToRemove.Any(status => status.ID == removedPlan));
            account.ActivePlans.AddRange(addedStatusIDs);
            account.StoreInformation(InformationContext.CurrentOwner);
        }

    }
}