using Fasciculus.Algorithms.Comparing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Algorithms.Tests
{
    [TestClass]
    public class EditDistanceTests
    {
        [TestMethod]
        public void TestString()
        {
            int actual;

            actual = EditDistance.GetDistance(string.Empty, "ab");
            Assert.AreEqual(2, actual);

            actual = EditDistance.GetDistance("GEEXSFRGEEKKS", "GEEKSFORGEEKS");
            Assert.AreEqual(3, actual);
        }

        [TestMethod]
        public void TestBytes()
        {
            byte[] a = { };
            byte[] b = { 1, 2 };
            byte[] c = { 1, 2, 3 };
            byte[] d = { 1, 3 };

            Assert.AreEqual(2, EditDistance.GetDistance(a, b));
            Assert.AreEqual(2, EditDistance.GetDistance(b, a));

            Assert.AreEqual(3, EditDistance.GetDistance(a, c));
            Assert.AreEqual(3, EditDistance.GetDistance(c, a));

            Assert.AreEqual(2, EditDistance.GetDistance(a, d));
            Assert.AreEqual(2, EditDistance.GetDistance(d, a));

            Assert.AreEqual(1, EditDistance.GetDistance(b, c));
            Assert.AreEqual(1, EditDistance.GetDistance(c, b));

            Assert.AreEqual(1, EditDistance.GetDistance(b, d));
            Assert.AreEqual(1, EditDistance.GetDistance(d, b));

            Assert.AreEqual(1, EditDistance.GetDistance(c, d));
            Assert.AreEqual(1, EditDistance.GetDistance(d, c));

            Assert.AreEqual(0, EditDistance.GetDistance(a, a));
            Assert.AreEqual(0, EditDistance.GetDistance(b, b));
            Assert.AreEqual(0, EditDistance.GetDistance(c, c));
            Assert.AreEqual(0, EditDistance.GetDistance(d, d));
        }
    }
}
