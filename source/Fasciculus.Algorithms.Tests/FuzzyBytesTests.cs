using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Algorithms.Tests
{
    [TestClass]
    public class FuzzyBytesTests
    {
        [TestMethod]
        public void TestEmpty()
        {
            byte[] start = { };
            FuzzyBytes[] fuzzyBytes = [.. FuzzyBytes.Generate(start, 2)];
            SortedSet<byte[]> set = new(fuzzyBytes.Select(x => x.Bytes), ByteArrayComparer.Instance);
            int expected = 65792;

            Assert.AreEqual(expected, fuzzyBytes.Length);
            Assert.AreEqual(fuzzyBytes.Length, set.Count);

            foreach (FuzzyBytes entry in fuzzyBytes)
            {
                Assert.IsTrue(entry.Bytes.Length > 0);
                Assert.IsTrue(entry.Bytes.Length <= 2);
            }

            Assert.AreEqual(256, fuzzyBytes.Where(x => x.Distance == 1).Count());
            Assert.AreEqual(expected - 256, fuzzyBytes.Where(x => x.Distance == 2).Count());
        }
    }
}
