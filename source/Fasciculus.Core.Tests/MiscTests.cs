using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Fasciculus.Core.Tests
{
    [TestClass]
    public class MiscTests
    {
        [TestMethod]
        public void TestArraySlice()
        {
            int[] input = { 1, 3, 5, 7 };
            int[] output1 = input[0..2];
            int[] output2 = input[0..0];

            Assert.AreEqual(2, output1.Length);
            Assert.AreEqual(1, output1[0]);
            Assert.AreEqual(3, output1[1]);

            Assert.AreEqual(0, output2.Length);
        }

        [TestMethod]
        public void TestArraySort()
        {
            int[] values = { 4, 2, 3, 1 };

            Array.Sort(values);

            Assert.IsTrue(values[0] < values[1]);
            Assert.IsTrue(values[1] < values[2]);
            Assert.IsTrue(values[2] < values[3]);
        }
    }
}
