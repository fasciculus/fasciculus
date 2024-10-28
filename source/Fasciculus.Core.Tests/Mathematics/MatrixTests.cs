using Fasciculus.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Core.Tests.Mathematics
{
    [TestClass]
    public class MatrixTests
    {
        [TestMethod]
        public void TestBoolMatrix()
        {
            MutableSparseBoolMatrix m1 = MutableSparseBoolMatrix.Create(3, 3);

            m1.Set(0, 0, true);
            m1.Set(1, 1, true);
            m1.Set(2, 2, true);

            Assert.IsTrue(m1.Get(0, 0));
            Assert.IsFalse(m1.Get(0, 1));
            Assert.IsFalse(m1.Get(0, 2));

            Assert.IsFalse(m1.Get(1, 0));
            Assert.IsTrue(m1.Get(1, 1));
            Assert.IsFalse(m1.Get(1, 2));

            Assert.IsFalse(m1.Get(2, 0));
            Assert.IsFalse(m1.Get(2, 1));
            Assert.IsTrue(m1.Get(2, 2));

            IMatrix<bool> m2 = m1.ToMatrix();

            Assert.IsTrue(m2.Get(0, 0));
            Assert.IsFalse(m2.Get(0, 1));
            Assert.IsFalse(m2.Get(0, 2));

            Assert.IsFalse(m2.Get(1, 0));
            Assert.IsTrue(m2.Get(1, 1));
            Assert.IsFalse(m2.Get(1, 2));

            Assert.IsFalse(m2.Get(2, 0));
            Assert.IsFalse(m2.Get(2, 1));
            Assert.IsTrue(m2.Get(2, 2));
        }
    }
}
