using Fasciculus.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Core.Tests.Algorithms
{
    [TestClass]
    public class RomanNumbersTests
    {
        [TestMethod]
        public void TestFormat()
        {
            Assert.AreEqual("I", RomanNumbers.Format(1));
            Assert.AreEqual("II", RomanNumbers.Format(2));
            Assert.AreEqual("III", RomanNumbers.Format(3));
            Assert.AreEqual("IV", RomanNumbers.Format(4));
            Assert.AreEqual("V", RomanNumbers.Format(5));
            Assert.AreEqual("VI", RomanNumbers.Format(6));
            Assert.AreEqual("VII", RomanNumbers.Format(7));
            Assert.AreEqual("VIII", RomanNumbers.Format(8));
            Assert.AreEqual("IX", RomanNumbers.Format(9));
            Assert.AreEqual("X", RomanNumbers.Format(10));
        }
    }
}
