using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Fasciculus.Algorithms.Tests
{
    [TestClass]
    public class SpanTests
    {
        [TestMethod]
        public void Test()
        {
            byte[]? bytes = null;
            Span<byte> span = bytes.AsSpan();

            Assert.AreEqual(0, span.Length);
        }
    }
}
