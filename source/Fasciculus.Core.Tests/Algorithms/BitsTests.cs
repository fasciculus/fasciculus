using Fasciculus.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Core.Tests.Algorithms
{
    [TestClass]
    public class BitsTests
    {
        [TestMethod]
        public void TestLog2()
        {
            Assert.AreEqual(0, Bits.Log2(1));
            Assert.AreEqual(1, Bits.Log2(2));
            Assert.AreEqual(1, Bits.Log2(3));
            Assert.AreEqual(2, Bits.Log2(4));
        }
    }
}
