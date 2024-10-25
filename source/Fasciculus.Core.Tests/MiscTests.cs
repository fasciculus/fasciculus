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
    }
}
