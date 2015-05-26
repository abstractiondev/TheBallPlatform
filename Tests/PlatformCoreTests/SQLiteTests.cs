using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLiteSupport;
using PAY=SQLite.TheBall.Payments;
using TBC=SQLite.TheBall.CORE;
using AGI=SQLite.AaltoGlobalImpact.OIP;

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
                var fullName = Path.Combine(directoryName, "x86", "SQLite.Interop.dll");
                return Assembly.LoadFile(fullName);
            }
            return null;
        }

        [TestMethod]
        public void CreatePaymentDomainDataBaseFromScratchTest()
        {
            var ctx = new PAY.TheBallDataContext(new SQLiteConnection("Data Source=:memory:"));
            ctx.CreateDomainDatabaseTablesIfNotExists();
            var testCustomer = new PAY.CustomerAccount
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
        public void CreateCoreDomainDataBaseFromScratchTest()
        {
            var ctx = new TBC.TheBallDataContext(new SQLiteConnection("Data Source=:memory:"));
            ctx.CreateDomainDatabaseTablesIfNotExists();
            var testItem = new TBC.NetworkUsage()
            {
                ID = Guid.NewGuid().ToString(),
                AmountOfBytes = 112233,
                UsageType = "test",
            };
            ctx.NetworkUsageTable.InsertOnSubmit(testItem);
            ctx.SubmitChanges();
            Assert.AreEqual(ctx.NetworkUsageTable.Count(), 1, "Insert confirmation failure");
        }

        [TestMethod]
        public void CreateAGIDomainDataBaseFromScratchTest()
        {
            var ctx = new AGI.TheBallDataContext(new SQLiteConnection("Data Source=:memory:"));
            ctx.CreateDomainDatabaseTablesIfNotExists();
            var testItem = new AGI.TBEmailValidation()
            {
                ID = Guid.NewGuid().ToString(),
                AccountID = "123",
                Email = "testemail",
            };
            ctx.TBEmailValidationTable.InsertOnSubmit(testItem);
            ctx.SubmitChanges();
            Assert.AreEqual(ctx.TBEmailValidationTable.Count(), 1, "Insert confirmation failure");
        }


        [TestMethod]
        public void InsertAndRetrieveCollectionTest()
        {
            var ctx = new PAY.TheBallDataContext(new SQLiteConnection("Data Source=:memory:"));
            ctx.CreateDomainDatabaseTablesIfNotExists();
            var testCustomer = new PAY.CustomerAccount
            {
                ID = Guid.NewGuid().ToString(),
                Description = "Test Description",
                EmailAddress = "Email address",
                StripeID = "TestStripeID"
            };
            //testCustomer.ActivePlans.Add("plan1");
            //testCustomer.ActivePlans.Add("plan2");
            ctx.CustomerAccountTable.InsertOnSubmit(testCustomer);
            ctx.SubmitChanges();
            Assert.AreEqual(ctx.CustomerAccountTable.Count(), 1, "Insert confirmation failure");
            //var firstCustomer = ctx.CustomerAccountTable.Single();
            var firstCustomer = ctx.CustomerAccountTable.Single(customer => customer.ID == testCustomer.ID);
            Assert.AreEqual(0, firstCustomer.ActivePlans.Count);
            //Assert.IsTrue(firstCustomer.ActivePlans.Contains("plan1"));
            //Assert.IsTrue(firstCustomer.ActivePlans.Contains("plan2"));
        }


        [TestMethod]
        public void ReflectionInvokeAGIDatabase()
        {
            Type agiContextType = typeof (SQLite.AaltoGlobalImpact.OIP.TheBallDataContext);
            string sqLiteDbLocationFileName = ":memory:";
            using (
                IStorageSyncableDataContext dbContext =
                    (IStorageSyncableDataContext)
                        agiContextType.InvokeMember("CreateOrAttachToExistingDB", BindingFlags.InvokeMethod, null, null,
                            new object[] {sqLiteDbLocationFileName})
                //SQLite.TheBall.Payments.TheBallDataContext.CreateOrAttachToExistingDB(sqLiteDbLocationFileName)
                )
            {
            }
        }
        [TestMethod]
        public void ReflectionInvokeCoreDatabase()
        {
            Type contextType = typeof(SQLite.TheBall.CORE.TheBallDataContext);
            string sqLiteDbLocationFileName = ":memory:";
            using (
                IStorageSyncableDataContext dbContext =
                    (IStorageSyncableDataContext)
                        contextType.InvokeMember("CreateOrAttachToExistingDB", BindingFlags.InvokeMethod, null, null,
                            new object[] { sqLiteDbLocationFileName })
                //SQLite.TheBall.Payments.TheBallDataContext.CreateOrAttachToExistingDB(sqLiteDbLocationFileName)
                )
            {
            }
        }
        [TestMethod]
        public void ReflectionInvokePaymentDatabase()
        {
            Type contextType = typeof(SQLite.TheBall.CORE.TheBallDataContext);
            string sqLiteDbLocationFileName = ":memory:";
            using (
                IStorageSyncableDataContext dbContext =
                    (IStorageSyncableDataContext)
                        contextType.InvokeMember("CreateOrAttachToExistingDB", BindingFlags.InvokeMethod, null, null,
                            new object[] { sqLiteDbLocationFileName })
                //SQLite.TheBall.Payments.TheBallDataContext.CreateOrAttachToExistingDB(sqLiteDbLocationFileName)
                )
            {
            }
        }

    }
}
