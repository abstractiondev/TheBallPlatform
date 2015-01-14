using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.TheBall.Payments;

namespace PlatformCoreTests
{
    [TestClass]
    public class SQLiteTests
    {
        [TestMethod]
        public void CreatePaymentDomainDataBaseFromScratchTest()
        {
            var ctx = new TheBallDataContext(new SQLiteConnection("Data Source=:memory:"));
            ctx.CreateDomainDatabaseTablesIfNotExists();
            var testCustomer = new CustomerAccount
            {
                ID = Guid.NewGuid().ToString(),
                Description = "Test Description",
                EmailAddress = "Email address",
                StripeID = "TestStripeID"
            };
            ctx.CustomerAccountTable.InsertOnSubmit(testCustomer);
            ctx.SubmitChanges();
            Assert.AreEqual(ctx.CustomerAccountTable.Count(), 1, "Insert confirmation failure");
        }

        [TestMethod]
        public void InsertAndRetrieveCollectionTest()
        {
            var ctx = new TheBallDataContext(new SQLiteConnection("Data Source=:memory:"));
            ctx.CreateDomainDatabaseTablesIfNotExists();
            var testCustomer = new CustomerAccount
            {
                ID = Guid.NewGuid().ToString(),
                Description = "Test Description",
                EmailAddress = "Email address",
                StripeID = "TestStripeID"
            };
            testCustomer.ActivePlans.Add("plan1");
            testCustomer.ActivePlans.Add("plan2");
            ctx.CustomerAccountTable.InsertOnSubmit(testCustomer);
            ctx.SubmitChanges();
            Assert.AreEqual(ctx.CustomerAccountTable.Count(), 1, "Insert confirmation failure");
            //var firstCustomer = ctx.CustomerAccountTable.Single();
            var firstCustomer = ctx.CustomerAccountTable.Single(customer => customer.ID == testCustomer.ID);
            Assert.AreEqual(firstCustomer.ActivePlans.Count, 2);
            Assert.IsTrue(firstCustomer.ActivePlans.Contains("plan1"));
            Assert.IsTrue(firstCustomer.ActivePlans.Contains("plan2"));
        }

    }
}
