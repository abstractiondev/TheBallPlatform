using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.TheBall.Payments;

namespace PlatformCoreTests
{
    [TestClass]
    public class SQLiteTests
    {

        [AssemblyInitialize]
        public static void FixBindings(TestContext context)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var asmName = new AssemblyName(args.Name);
            if (asmName.Name == "SQLite.Interop.dll")
            {
                var currAsm = Assembly.GetExecutingAssembly();
                var directoryName = Path.GetDirectoryName(currAsm.Location);
                var fullName = Path.Combine(directoryName, "x64", "SQLite.Interop.dll");
                return Assembly.LoadFile(fullName);
            }
            return null;
        }

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
