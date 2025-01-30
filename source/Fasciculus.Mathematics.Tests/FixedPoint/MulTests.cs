using Fasciculus.Mathematics.FixedPoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Fasciculus.Mathematics.Tests.FixedPoint
{
    [TestClass]
    public class MulTests : TestsBase
    {
        [TestMethod]
        public void TestFP16Q8()
        {
            ushort x;
            ushort a = FP16Q8.Add(FP16Q8.One, FP16Q8.Epsilon);

            x = FP16Q8.Mul(FP16Q8.One, FP16Q8.One);
            Assert.AreEqual(FP16Q8.One, x);

            x = FP16Q8.Mul(FP16Q8.One, FP16Q8.NegativeOne);
            Assert.AreEqual(FP16Q8.NegativeOne, x);

            x = FP16Q8.Mul(FP16Q8.NegativeOne, FP16Q8.NegativeOne);
            Assert.AreEqual(FP16Q8.One, x);

            x = FP16Q8.Mul(a, a);
            Assert.AreEqual(1.0078125, FP16Q8.ToDouble(x));
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
                        double x = doubles[i] * doubles[j];
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
                        ushort x = FP16Q8.Mul(values[i], values[j]);
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
