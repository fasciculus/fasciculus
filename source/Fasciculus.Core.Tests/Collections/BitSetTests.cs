using Fasciculus.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fasciculus.Core.Tests.Collections
{
    [TestClass]
    public class BitSetTests
    {
        [TestMethod]
        public void TestCreate()
        {
            BitSet a = BitSet.Create();
            BitSet b = BitSet.Create(1, 3);

            Assert.AreEqual(0, a.Count);
            Assert.AreEqual(2, b.Count);

            Assert.IsFalse(b[0]);
            Assert.IsTrue(b[1]);
            Assert.IsFalse(b[2]);
            Assert.IsTrue(b[3]);
            Assert.IsFalse(b[4]);

            Assert.IsFalse(a.Intersects(b));
        }

        [TestMethod]
        public void TestOperations()
        {
            BitSet a = BitSet.Create(1, 3, 5);
            BitSet b = BitSet.Create(0, 3, 6);

            BitSet c = a + b;

            AssertEqual(BitSet.Create(0, 1, 3, 5, 6), c);

            BitSet d = a - b;

            AssertEqual(BitSet.Create(1, 5), d);

            BitSet e = a * b;

            AssertEqual(BitSet.Create(3), e);

            Assert.IsTrue(a.Intersects(b));
            Assert.IsTrue(a.Intersects(c));
            Assert.IsTrue(b.Intersects(c));
        }

        [TestMethod]
        public void TestEmpty()
        {
            BitSet a = BitSet.Create();
            BitSet b = BitSet.Create(1, 3);

            BitSet c = a + b;
            BitSet d = b + a;

            AssertEqual(b, c);
            AssertEqual(b, d);

            BitSet e = a - b;
            BitSet f = b - a;

            AssertEqual(a, e);
            AssertEqual(b, f);

            BitSet g = a * b;
            BitSet h = b * a;

            AssertEqual(a, g);
            AssertEqual(a, h);
        }

        private static void AssertEqual(BitSet a, BitSet b)
        {
            Assert.AreEqual(a.Count, b.Count);

            int[] aa = a.ToArray();
            int[] bb = b.ToArray();

            for (int i = 0, n = a.Count; i < n; ++i)
            {
                Assert.AreEqual(aa[i], bb[i]);
            }
        }
    }
}
