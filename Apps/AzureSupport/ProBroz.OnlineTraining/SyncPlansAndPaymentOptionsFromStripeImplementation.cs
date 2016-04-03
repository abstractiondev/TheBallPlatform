using System;
using System.Linq;
using System.Threading.Tasks;
using Stripe;
using TheBall;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;

namespace ProBroz.OnlineTraining
{
    public class SyncPlansAndPaymentOptionsFromStripeImplementation
    {
        public static async Task<MembershipPlan[]> GetTarget_CurrentPlansAsync()
        {
            var masterCollection = await ObjectStorage.RetrieveFromOwnerContentA<MembershipPlanCollection>("MasterCollection");
            var existingCollection = masterCollection?.CollectionContent.ToArray();
            return existingCollection ?? new MembershipPlan[0];
        }

        public static async Task<PaymentOption[]> GetTarget_CurrentPaymentOptionsAsync()
        {
            var masterCollection =
                await ObjectStorage.RetrieveFromOwnerContentA<PaymentOptionCollection>("MasterCollection");
            var existingCollection = masterCollection?.CollectionContent.ToArray();
            return existingCollection ?? new PaymentOption[0];
        }

        public static Tuple<MembershipPlan[], PaymentOption[]> GetTarget_StripeFetchedPlansAndPaymentOptions()
        {
            var planService = new Stripe.StripePlanService(SecureConfig.Current.StripeSecretKey);
            var allStripePlans =
                planService.List(new StripeListOptions {Limit = 100}).ToArray();
            var plans = allStripePlans
                    .Where(plan => plan.Metadata.ContainsKey("PLANTYPE") && plan.Metadata["PLANTYPE"] == "MEMBERSHIP")
                    .ToArray();

            var logicalPlanDict = plans.GroupBy(plan => plan.Id.Substring(0, plan.Id.LastIndexOf('_')))
                .ToDictionary(grp => grp.Key, grp => grp.ToArray());

            var paymentOptionsDict = plans.Select(plan => new PaymentOption
            {
                OptionName = plan.Id,
                PeriodInMonths = plan.Interval == "year" ? plan.IntervalCount * 12 : plan.IntervalCount,
                Price = ((double) plan.Amount / 100),
            }).ToDictionary(po => po.OptionName);

            var membershipPlans = logicalPlanDict.Keys.Select(key =>
            {
                var stripePlans = logicalPlanDict[key];
                var infoSource = stripePlans.First();
                var planName = infoSource.Name.Substring(0, infoSource.Name.IndexOf("-")).Trim();
                return new MembershipPlan
                {
                    PlanName = planName,
                    Description = planName,
                    PaymentOptions = stripePlans.Select(splan => paymentOptionsDict[splan.Id].ID).ToList()
                };
            }).ToArray();
            return new Tuple<MembershipPlan[], PaymentOption[]>(membershipPlans, paymentOptionsDict.Values.ToArray());
        }


        public static MembershipPlan[] GetTarget_StripeFetchedPlans(Tuple<MembershipPlan[], PaymentOption[]> stripeFetchedPlansAndPaymentOptions)
        {
            return stripeFetchedPlansAndPaymentOptions.Item1;
        }

        public static PaymentOption[] GetTarget_StripeFetchedPaymentOptions(Tuple<MembershipPlan[], PaymentOption[]> stripeFetchedPlansAndPaymentOptions)
        {
            return stripeFetchedPlansAndPaymentOptions.Item2;
        }

        public static async Task ExecuteMethod_SynchronizeLocalFromStripeAsync(MembershipPlan[] currentPlans, PaymentOption[] currentPaymentOptions, MembershipPlan[] stripeFetchedPlans, PaymentOption[] stripeFetchedPaymentOptions)
        {
            var currentPlansToDelete =
                currentPlans.Where(plan => stripeFetchedPlans.All(splan => splan.PlanName != plan.PlanName)).Cast<IInformationObject>().ToArray();
            var currentPaymentOptionsToDelete =
                currentPaymentOptions.Where(
                    po => stripeFetchedPaymentOptions.All(spo => spo.OptionName != po.OptionName)).Cast<IInformationObject>().ToArray();

            var combinedObjectsToDelete = currentPlansToDelete.Concat(currentPaymentOptionsToDelete);
            var deleteAwaitables =
                combinedObjectsToDelete.Select(obj => obj.DeleteInformationObjectAsync(InformationContext.CurrentOwner))
                    .ToArray();
            await Task.WhenAll(deleteAwaitables);

            currentPlans = currentPlans.Except(currentPlansToDelete.Cast<MembershipPlan>()).ToArray();
            currentPaymentOptions =
                currentPaymentOptions.Except(currentPaymentOptionsToDelete.Cast<PaymentOption>()).ToArray();

            var newPlans =
                stripeFetchedPlans.Where(splan => currentPlans.All(plan => plan.PlanName != splan.PlanName)).ToArray();
            var newPaymentOptions =
                stripeFetchedPaymentOptions.Where(
                    spo => currentPaymentOptions.All(po => po.OptionName != spo.OptionName)).ToArray();

            var currentModifiedPlans = currentPlans.Where(plan =>
            {
                var removed = plan.PaymentOptions.RemoveAll(id => currentPaymentOptions.Any(cpo => cpo.ID == id));
                bool anyRemoved = removed > 0;
                bool anyAdded = plan.PaymentOptions.Any(po => newPaymentOptions.Any(added => added.ID == po));
                return anyRemoved || anyAdded;
            }).ToArray();

            var objectsToSave =
                currentModifiedPlans.Cast<IInformationObject>().Concat(newPlans).Concat(newPaymentOptions).ToArray();
            var saveAwaitables =
                objectsToSave.Select(obj => obj.StoreInformationAsync(InformationContext.CurrentOwner)).ToArray();
            await Task.WhenAll(saveAwaitables);
        }

    }
}