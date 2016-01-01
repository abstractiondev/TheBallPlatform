using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlatformCoreTests
{
    [TestClass]
    public class DataShardingTests
    {

        //[TestMethod]
        public void DefaultGUIDDistributionTest()
        {
            const int testIterations = 10;
            const int guidIterations = 10000000;
            const int shardCount = 8192;
            const double acceptableRatioThreshold = 0.94;

            var iterationRatios = new List<Tuple<double, double, int, int>>();

            for (int testIteration = 0; testIteration < testIterations; testIteration++)
            {
                var counters = new int[shardCount];
                for (int i = 0; i < guidIterations; i++)
                {
                    var currShard = getGUIDShardIndex(Guid.NewGuid(), shardCount);
                    counters[currShard]++;
                }

                var sortedData = counters.OrderBy(item => item).ToArray();
                var minCounter = counters.Min();
                var maxCounter = counters.Max();
                var min90p = sortedData[shardCount / 10];
                var max90p = sortedData[(shardCount * 9) / 10];
                var minPerMaxRatio = (double)minCounter/ (double)maxCounter;
                var minPerMax90p = (double) min90p/(double) max90p;
                iterationRatios.Add(new Tuple<double, double, int, int>(minPerMax90p, minPerMaxRatio, min90p, max90p));
                //Assert.IsTrue(minPerMaxRatio > acceptableRatioThreshold, $"Max {maxCounter} and min {minCounter} differ too much");
            }
            var doubleComparer = Comparer<double>.Default;
            iterationRatios.Sort((a, b) => Comparer<double>.Default.Compare(a.Item1, b.Item1));
            var maxRatio = iterationRatios.Last();
            var minRatio = iterationRatios.First();
            int xi = 0;
        }

        private int getGUIDShardIndex(Guid guid, int shardCount)
        {
            var guidBytes = guid.ToByteArray();
            var intVal = Math.Abs(BitConverter.ToInt32(guidBytes, 0));
            var index = intVal%shardCount;
            return (int) index;
        }
    }
}