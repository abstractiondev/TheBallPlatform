using System;
using System.Collections.Generic;
using System.IO;
using AzureSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProBroz.OnlineTraining;
using TheBall.Core;
using TheBall.Core.Storage;

namespace PlatformCoreTests
{
    [TestClass]
    public class InformationObjectSerializationTests
    {
        [TestMethod]
        public void ProBrozOnlineTrainingSerializationTest()
        {
            var member1 = new Member
            {
                FirstName = "Donald",
                LastName = "Duck",
                Address = "Likusteritie 123 A 9",
                BirthDay = new DateTime(1917, 12, 6),
                Country = "Åland",
                Email = "donald.duck@theball.me",
                MiddleName = "Birdie",
                PhoneNumber = "+123 456 789",
                PostOffice = "Helsinki",
                VideoPermission = true,
                PhotoPermission = false,
                ZipCode = "123456",
                FederationLicense = "invalid",
            };
            var member2 = new Member
            {
                FirstName = "Mickey",
                LastName = "Mouse",
                Address = "Tylypahka 2 Z 9",
                BirthDay = new DateTime(2002, 2, 2),
                Country = "Lakeland",
                Email = "mickey@theball.me",
                MiddleName = null,
                PhoneNumber = "+123 456 789",
                PostOffice = "Helsinki",
                VideoPermission = true,
                PhotoPermission = true,
                ZipCode = "123456",
                FederationLicense = "invalid",
            };
            var paymentOption1 = new PaymentOption
            {
                OptionName = "Family 1",
                PeriodInMonths = 4,
                Price = 120
            };
            var paymentOption2 = new PaymentOption
            {
                OptionName = "Family 1",
                PeriodInMonths = 12,
                Price = 300
            };
            var paymentOption3 = new PaymentOption
            {
                OptionName = "Premium",
                PeriodInMonths = 4,
                Price = 1200
            };
            var paymentOption4 = new PaymentOption
            {
                OptionName = "Premium",
                PeriodInMonths = 12,
                Price = 3600
            };
            var tenantGym = new TenantGym
            {
                Address = "Vuosaari",
                Country = "Suomi",
                Email = "vuosaarigym@theball.me",
                PhoneNumber = "555 555 555",
                PostOffice = "Helsinki",
                GymName = "iZENZEi Vuosaari",
                ZipCode = "55555",
            };
            var membershipPlan1 = new MembershipPlan
            {
                PlanName = "Family 1",
                Description = "Family 1 plan for family bunch",
                PaymentOptions = new List<string>() { paymentOption1.ID, paymentOption2.ID },
                Gym = tenantGym.ID
            };

            var membershipPlan2 = new MembershipPlan
            {
                PlanName = "Premium",
                Description = "Premium for everything",
                PaymentOptions = new List<string>() { paymentOption3.ID, paymentOption4.ID },
                Gym = tenantGym.ID
            };

            var subscription1 = new Subscription
            {
                Created = DateTime.UtcNow,
                PaymentOption = paymentOption1.ID,
                Plan = membershipPlan1.ID,
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddDays(120),
            };

            var subscription2 = new Subscription
            {
                Created = DateTime.UtcNow,
                PaymentOption = paymentOption3.ID,
                Plan = membershipPlan2.ID,
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddDays(120),
            };

            member1.Subscriptions.Add(subscription1.ID);
            member2.Subscriptions.Add(subscription2.ID);

            cleanupOnlineTraining();


            writeObjectJSON(member1);
            writeObjectJSON(member2);
            var members = new MemberCollection();
            members.CollectionContent.Add(member1);
            members.CollectionContent.Add(member2);
            members.ID = "MasterCollection";
            writeObjectJSON(members);

            writeObjectJSON(paymentOption1);
            writeObjectJSON(paymentOption2);
            writeObjectJSON(paymentOption3);
            writeObjectJSON(paymentOption4);
            var paymentOptions = new PaymentOptionCollection();
            paymentOptions.CollectionContent.Add(paymentOption1);
            paymentOptions.CollectionContent.Add(paymentOption2);
            paymentOptions.CollectionContent.Add(paymentOption3);
            paymentOptions.CollectionContent.Add(paymentOption4);
            paymentOptions.ID = "MasterCollection";
            writeObjectJSON(paymentOptions);

            writeObjectJSON(tenantGym);
            var tenantGyms = new TenantGymCollection();
            tenantGyms.CollectionContent.Add(tenantGym);
            tenantGyms.ID = "MasterCollection";
            writeObjectJSON(tenantGyms);


            writeObjectJSON(membershipPlan1);
            writeObjectJSON(membershipPlan2);
            var membershipPlans = new MembershipPlanCollection();
            membershipPlans.CollectionContent.Add(membershipPlan1);
            membershipPlans.CollectionContent.Add(membershipPlan2);
            membershipPlans.ID = "MasterCollection";
            writeObjectJSON(membershipPlans);


            writeObjectJSON(subscription1);
            writeObjectJSON(subscription2);
            var subscriptions = new SubscriptionCollection();
            subscriptions.CollectionContent.Add(subscription1);
            subscriptions.CollectionContent.Add(subscription2);
            subscriptions.ID = "MasterCollection";
            writeObjectJSON(subscriptions);
        }

        private void writeObjectJSON(IInformationObject iObject)
        {
            JSONSupport.SerializeToJSONStream(iObject, getWritableStream(iObject));
        }

        private void cleanupOnlineTraining()
        {
            var dirInfo = new DirectoryInfo(TestSupport.OnlineTrainingPath);
            if(dirInfo.Exists)
                dirInfo.Delete(true);
        }

        string getObjectFileName(IInformationObject iObject)
        {
            return $"{iObject.SemanticDomainName}/{iObject.Name}/{iObject.ID}.json";
        }

        Stream getWritableStream(IInformationObject iObject)
        {
            var relativeName = getObjectFileName(iObject);
            var fullName = Path.Combine(TestSupport.OnlineTrainingPath, relativeName);
            var fileInfo = new FileInfo(fullName);
            var dirInfo = fileInfo.Directory;
            if(!dirInfo.Exists)
                dirInfo.Create();
            return File.Create(fullName);
        }

    }
}