using Fasciculus.Algorithms.Comparing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Algorithms.Tests
{
    [TestClass]
    public class ByteArrayComparerTests
    {
        [TestMethod]
        public void Test()
        {
            ByteArrayComparer comparer = ByteArrayComparer.Instance;

            byte[]? a = null;
            byte[] b = { };
            byte[] c = { 1 };
            byte[] d = { 2 };
            byte[] e = { 1, 2 };

            Assert.AreEqual(0, comparer.Compare(a, a));
            Assert.AreEqual(0, comparer.Compare(b, b));
            Assert.AreEqual(0, comparer.Compare(c, c));
            Assert.AreEqual(0, comparer.Compare(d, d));
            Assert.AreEqual(0, comparer.Compare(e, e));

            Assert.AreEqual(0, comparer.Compare(a, b));
            Assert.AreEqual(0, comparer.Compare(b, a));

            Assert.AreEqual(-1, comparer.Compare(a, c));
            Assert.AreEqual(1, comparer.Compare(c, a));

            Assert.AreEqual(-1, comparer.Compare(a, d));
            Assert.AreEqual(1, comparer.Compare(d, a));

            Assert.AreEqual(-1, comparer.Compare(a, e));
            Assert.AreEqual(1, comparer.Compare(e, a));

            Assert.AreEqual(-1, comparer.Compare(b, c));
            Assert.AreEqual(1, comparer.Compare(c, b));

            Assert.AreEqual(-1, comparer.Compare(b, d));
            Assert.AreEqual(1, comparer.Compare(d, b));

            Assert.AreEqual(-1, comparer.Compare(b, e));
            Assert.AreEqual(1, comparer.Compare(e, b));

            Assert.AreEqual(-1, comparer.Compare(c, d));
            Assert.AreEqual(1, comparer.Compare(d, c));

            Assert.AreEqual(-1, comparer.Compare(c, e));
            Assert.AreEqual(1, comparer.Compare(e, c));

            Assert.AreEqual(1, comparer.Compare(d, e));
            Assert.AreEqual(-1, comparer.Compare(e, d));
        }
    }
}
