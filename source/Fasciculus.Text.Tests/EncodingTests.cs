using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Fasciculus.Text.Tests
{
    [TestClass]
    public class EncodingTests
    {
        [TestMethod]
        public void TesFromBytes()
        {
            Encoding utf8 = Encoding.UTF8;

            byte[] a = { };

            Assert.AreEqual(string.Empty, utf8.GetString(a));

            Assert.IsTrue(utf8.TryGetString(a, out string? sa));
            Assert.AreEqual(string.Empty, sa);
        }
    }
}
