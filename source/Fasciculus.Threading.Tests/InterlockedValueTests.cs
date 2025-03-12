using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Threading.Tests
{
    [TestClass]
    public class InterlockedValueTests
    {
        [TestMethod]
        public void TestBool()
        {
            InterlockedBool interlocked = new();

            Assert.IsFalse(interlocked.Read());

            Assert.IsTrue(interlocked.Replace(true, false));
            Assert.IsTrue(interlocked.Read());
        }

        [TestMethod]
        public void TestLong()
        {
            InterlockedLong interlocked = new();

            Assert.AreEqual(0, interlocked.Read());
            Assert.AreEqual(1, interlocked.Increment());
            Assert.AreEqual(0, interlocked.Decrement());
        }
    }
}
