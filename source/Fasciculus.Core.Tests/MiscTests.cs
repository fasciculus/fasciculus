using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Fasciculus.Core.Tests
{
    [TestClass]
    public class MiscTests
    {
        [TestMethod]
        public void TestBinarySearch()
        {
            int[] values = { 1, 3, 5, 7 };

            Assert.AreEqual(0, Array.BinarySearch(values, 0, 4, 1));
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
