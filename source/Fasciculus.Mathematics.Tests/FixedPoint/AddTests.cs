using Fasciculus.Mathematics.FixedPoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Fasciculus.Mathematics.Tests.FixedPoint
{
    [TestClass]
    public class AddTests : TestsBase
    {
        [TestMethod]
        public void TestFP16Q8()
        {
            ushort a = FP16Q8.One;
            ushort sum;

            sum = FP16Q8.Add(a, a);
            Assert.AreEqual(2.0, FP16Q8.ToDouble(sum));

            sum = FP16Q8.Add(sum, a);
            Assert.AreEqual(3.0, FP16Q8.ToDouble(sum));
        }

        [TestMethod]
        public void PerformanceFP16Q8()
        {
            Thread thread = Thread.CurrentThread;
            ThreadPriority oldPriority = thread.Priority;

            thread.Priority = ThreadPriority.Highest;

            int count = 300;
            double[] doubles = Enumerable.Range(0, count).Select(x => x + 0.0).ToArray();
            ushort[] values = Enumerable.Range(0, count).Select(x => (ushort)x).ToArray();
            Stopwatch sw = new();
            double doublesTicks = 0;
            double valuesTicks = 0;

            for (int round = 0; round < 50; ++round)
            {
                Thread.Yield();
                sw.Start();

                for (int i = 0; i < count; ++i)
                {
                    for (int j = 0; j < count; ++j)
                    {
                        double x = doubles[i] + doubles[j];
                    }
                }

                sw.Stop();
                doublesTicks += sw.ElapsedTicks;

                sw.Reset();
                Thread.Yield();
                sw.Start();

                for (int i = 0; i < count; ++i)
                {
                    for (int j = 0; j < count; ++j)
                    {
                        ushort x = FP16Q8.Add(values[i], values[j]);
                    }
                }

                sw.Stop();
                valuesTicks += sw.ElapsedTicks;
            }

            Log($"FP16Q8 is {Math.Round(doublesTicks * 100.0 / valuesTicks) / 100.0} times faster than double");

            thread.Priority = oldPriority;
        }
    }
}
