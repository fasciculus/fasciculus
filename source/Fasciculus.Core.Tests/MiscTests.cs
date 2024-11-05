using Fasciculus.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Fasciculus.Core.Tests
{
    [TestClass]
    public class MiscTests
    {
        [TestMethod]
        public unsafe void Test()
        {
            int[] array = { 1, 2, 3, 4, 5, 6, 7 };
            int reps = 1_000_000;

            Stopwatch sw1 = new();
            Stopwatch sw2 = new();

            sw1.Start();

            for (int i = 0; i < reps; ++i)
            {
                BinarySearch.IndexOf(array, i);
            }

            sw1.Stop();

            sw2.Start();

            for (int i = 0; i < reps; ++i)
            {
                IndexOf(array, i);
            }

            sw2.Stop();

            Debug.WriteLine($"spn: {sw1.Elapsed}");
            Debug.WriteLine($"ptr: {sw2.Elapsed}");
        }

        private static unsafe int IndexOf(int[] sorted, int value)
        {
            int lo = 0;
            int hi = sorted.Length - 1;

            fixed (int* a = sorted)
            {
                while (lo <= hi)
                {
                    int i = lo + ((hi - lo) >> 1);
                    int x = a[i];

                    if (x == value)
                    {
                        return i;
                    }

                    if (x < value)
                    {
                        lo = i + 1;
                    }
                    else
                    {
                        hi = i - 1;
                    }
                }
            }

            return -1;
        }
    }
}
