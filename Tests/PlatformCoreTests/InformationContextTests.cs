using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheBall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheBall.Tests
{
    [TestClass()]
    public class InformationContextTests
    {
        [TestMethod]
        public void CurrentCollidingLogicalCallContexts()
        {
            var firstContext = InformationContext.InitializeToLogicalContext();
            var task = Task.Run(async () =>
            {
                var taskCtx = InformationContext.InitializeToLogicalContext();
                return taskCtx;
            });
            InvalidOperationException expectedException = null;
            try
            {
                Task.WhenAll(task).Wait();
            }
            catch (AggregateException aggregateException)
            {
                expectedException = aggregateException.InnerExceptions[0] as InvalidOperationException;
            }
            Assert.IsNotNull(expectedException);
        }

        [TestMethod()]
        public void CurrentFromLogicalCallContextTest()
        {

            Random rnd = new Random();
            const int taskCount = 1000;
            var tasks = new List<Task<Tuple<InformationContext, InformationContext, InformationContext>>>();
            for (int i = 0; i < taskCount; i++)
            {
                var task = Task.Run(async () =>
                {

                    var firstCtx = InformationContext.InitializeToLogicalContext();
                    var secondCtx = await retrieveCurrentContext(rnd);
                    var thirdCtx = await Task.Run(async () =>
                    {
                        await Task.Delay(rnd.Next(200, 300));
                        return await retrieveCurrentContext(rnd);
                    });
                    return new Tuple<InformationContext, InformationContext, InformationContext>(firstCtx, secondCtx, thirdCtx);
                });
                tasks.Add(task);

            }
            Task.WhenAll(tasks).Wait();
            List<InformationContext> contexts = new List<InformationContext>();
            foreach(var task in tasks)
            {
                var result = task.Result;
                var firstCtx = result.Item1;
                var secondCtx = result.Item2;
                var thirdCtx = result.Item3;
                Assert.AreEqual(firstCtx, secondCtx);
                Assert.AreEqual(firstCtx, thirdCtx);
                Assert.AreEqual(firstCtx.SerialNumber, secondCtx.SerialNumber);
                Assert.AreEqual(firstCtx.SerialNumber, thirdCtx.SerialNumber);
                contexts.Add(firstCtx);
            }

            var verifyDict = contexts.ToDictionary(item => item.SerialNumber);
            Assert.AreEqual(taskCount, verifyDict.Count);
        }


        private async Task<InformationContext> retrieveCurrentContext(Random rnd)
        {
            await Task.Delay(rnd.Next(100, 200));
            return InformationContext.Current;
        }
   }
}