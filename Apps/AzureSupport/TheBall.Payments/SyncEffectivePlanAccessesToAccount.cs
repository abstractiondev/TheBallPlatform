using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using AzureSupport;
using Stripe;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;
using TheBall.CORE.Storage;
using TheBall.Payments.INT;

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

            return result;
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

        public static async Task<PlanStatus[]> GetTarget_ActivePlanStatusesFromStripeAsync(CustomerAccount account, bool isTestMode)
        {
            StripeCustomerService customerService = new StripeCustomerService(isTestMode
                ? SecureConfig.Current.StripeTestSecretKey
                : SecureConfig.Current.StripeLiveSecretKey);
            StripeCustomer stripeCustomer = await customerService.GetAsync(account.StripeID);
            if (stripeCustomer == null)
                return new PlanStatus[0];
            var stripeSubs =
                stripeCustomer.StripeSubscriptionList.Data
                    .Select(stripeSub => new {Plan = stripeSub.StripePlan, Subscription = stripeSub})
                    .ToArray();
            var plans = stripeSubs.Select(stripeSub => new PlanStatus()
            {
                name = stripeSub.Plan.Id,
                validuntil = stripeSub.Subscription.CurrentPeriodEnd.GetValueOrDefault(DateTime.MaxValue),
                cancelatperiodend = stripeSub.Subscription.CancelAtPeriodEnd
            }).ToArray();
            return plans;
        }


        public static GroupSubscriptionPlan[] GetTarget_ActivePlansFromStripe(PlanStatus[] activePlanStatusesFromStripe)
        {
            var plans = activePlanStatusesFromStripe.Select(stripePlan => GetGroupSubscriptionPlan(stripePlan.name)).ToArray();
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

        public static async Task<SubscriptionPlanStatus[]> ExecuteMethod_SyncCurrentStripePlansToAccountAsync(CustomerAccount account, GroupSubscriptionPlan[] currentPlansBeforeSync, GroupSubscriptionPlan[] activePlansFromStripe)
        {
            var activePlanIDs = activePlansFromStripe.Select(plan => plan.PlanName).ToArray();

            var currentStatusIDs = account.ActivePlans;
            var currentStatuses =
                currentStatusIDs.Select(
                    statusID => ObjectStorage.RetrieveFromOwnerContent<SubscriptionPlanStatus>(InformationContext.CurrentOwner, statusID))
                    .Where(status => status != null).ToArray();
            var statusesToRemove =
                currentStatuses.Where(status => activePlanIDs.All(planID => planID != status.SubscriptionPlan))
                    .ToArray();
            var plansToAddStatusesFor =
                activePlanIDs.Where(planID => currentStatuses.All(status => status.SubscriptionPlan != planID))
                    .ToArray();
            foreach (var status in statusesToRemove)
                status.DeleteInformationObject(InformationContext.CurrentOwner);
            List<string> addedStatusIDs = new List<string>();
            foreach (var planToAddStatus in plansToAddStatusesFor)
            {
                SubscriptionPlanStatus planStatus = new SubscriptionPlanStatus();
                planStatus.SetLocationAsOwnerContent(InformationContext.CurrentOwner, planStatus.ID);
                planStatus.SubscriptionPlan = planToAddStatus;
                await planStatus.StoreInformationAsync(InformationContext.CurrentOwner);
                addedStatusIDs.Add(planStatus.ID);
            }

            //account.ActivePlans.Clear();
            account.ActivePlans.RemoveAll(
                removedPlan => statusesToRemove.Any(status => status.ID == removedPlan));
            account.ActivePlans.AddRange(addedStatusIDs);
            await account.StoreInformationAsync(InformationContext.CurrentOwner);
            return currentStatuses;
        }

        public async static Task ExecuteMethod_UpdateStatusesOnAccountAsync(string accountID, PlanStatus[] activePlanStatusesFromStripe)
        {
            // TODO: Sync to account InterfaceData
            string objectName = "TheBall.Interface/InterfaceData/CustomerActivePlans.json";
            var accountOwner = new VirtualOwner("acc", accountID);
            var jsonBlob = StorageSupport.GetOwnerBlobReference(accountOwner, objectName);
            var customerActivePlans = new CustomerActivePlans();
            customerActivePlans.PlanStatuses =
                activePlanStatusesFromStripe;
            using (var memStream = new MemoryStream())
            {
                JSONSupport.SerializeToJSONStream(customerActivePlans, memStream);
                var data = memStream.ToArray();
                await jsonBlob.UploadFromByteArrayAsync(data, 0, data.Length);
            }
        }

        public static bool GetTarget_IsTestMode(CustomerAccount account)
        {
            return account.IsTestAccount;
        }
    }
}