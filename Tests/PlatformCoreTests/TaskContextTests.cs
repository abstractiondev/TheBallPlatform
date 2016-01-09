using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheBall;
using TheBall.CORE;

namespace PlatformCoreTests
{
    /// <summary>
    /// Summary description for TaskContextTests
    /// </summary>
    //[TestClass]
    public class TaskContextTests
    {
        public TaskContextTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;
        private static Random LocalRandom;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            LocalRandom = new Random();
        }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void NoTaskFailureTest()
        {
            var ctx = InformationContext.Current;
        }


        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ImplicitTaskTest()
        {
            //InformationContext.InitializeFunctionality(2);
            var task = CallerStubWithCtx1(10, null);
            task.Wait();
            var result = task.Result;
            Assert.AreNotEqual(0, result);
        }

        [TestMethod]
        public void ExplicitTaskTest()
        {
            try
            {
                InformationContext.InitializeToLogicalContext(new VirtualOwner("tst", "tst"), "tstInstance");
                var currentCtx = InformationContext.Current;
                int result = 0;
                var task = Task.Factory.StartNew(() =>
                {
                    var implicitTask = CallerStubWithCtx1(10, currentCtx);
                    implicitTask.Wait();
                    result = implicitTask.Result;
                });
                task.Wait();
                Assert.AreNotEqual(task.Id, result);
                Assert.AreEqual(0, result);
            }
            finally
            {
                InformationContext.RemoveFromLogicalContext();
            }
        }

        [TestMethod]
        public void MassiveParallelTasksTest()
        {
            const int TaskCount = 1000;
            try
            {
                Dictionary<InformationContext, bool> uniqueDict = new Dictionary<InformationContext, bool>();
                var tasks = new Task[TaskCount];
                for (int i = 0; i < TaskCount; i++)
                {
                    var task = Task.Run(async () =>
                    {
                        InformationContext.InitializeToLogicalContext(new VirtualOwner("tst", "tst"), "tstInstance");
                        var ctx = InformationContext.Current;
                        lock(uniqueDict)
                            uniqueDict.Add(ctx, true);
                        await CallerStubWithCtx1(LocalRandom.Next(100, 1000), ctx);
                        InformationContext.RemoveFromLogicalContext();
                    });
                    tasks[i] = task;
                }
                Task.WaitAll(tasks);
                Assert.AreEqual(uniqueDict.Count, TaskCount);
            }
            finally
            {
            }
        }


        private async Task<int> CallerStubWithoutCtx1()
        {
            var result = await AsyncStubWithoutCtx1();
            return result;
        }

        private async Task<int> CallerStubWithCtx1(int waitMilliseconds, InformationContext ctx)
        {
            var result = await AsyncStubWithCtx1(waitMilliseconds, ctx);
            return result;
        }

        private async Task<int> AsyncStubWithoutCtx1()
        {
            //await Task.Delay(LocalRandom.Next(10));
            return Task.CurrentId.GetValueOrDefault(0);
        }

        private async Task<int> AsyncStubWithCtx1(int waitMilliseconds, InformationContext expectedContext)
        {
            var ctx = InformationContext.Current;
            await Task.Delay(waitMilliseconds);
            //Task.Delay(TimeSpan.FromMilliseconds(waitMilliseconds)).Wait();
            var ctx2 = InformationContext.Current;
            if(ctx != ctx2 || ctx != expectedContext)
                throw new InvalidOperationException("Context mismatch");
            return Task.CurrentId.GetValueOrDefault(0);
        }

    }
}
