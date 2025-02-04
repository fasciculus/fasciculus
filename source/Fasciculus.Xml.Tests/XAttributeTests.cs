using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace Fasciculus.Xml.Tests
{
    [TestClass]
    public class XAttributeTests
    {
        [TestMethod]
        public void TestString()
        {
            XAttribute attribute = new("foo", "bar");

            string expected = "bar";
            string actual = (string)attribute;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBoolean()
        {
            XAttribute attribute = new("foo", "true");

            bool expected = true;
            bool actual = (bool)attribute;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestToBool()
        {
            XAttribute t = new("foo", true);
            XAttribute f = new("foo", false);

            Assert.IsTrue(t.ToBool());
            Assert.IsFalse(f.ToBool());
        }

        [TestMethod]
        public void TestToByte()
        {
            XAttribute attribute = new("foo", 1);
            byte expected = 1;
            byte actual = attribute.ToByte();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestToDateTime()
        {
            DateTime now = DateTime.Now.ToUniversalTime();
            XAttribute attribute = new("foo", now);
            DateTime expected = now;
            DateTime actual = attribute.ToDateTime();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestToStrings()
        {
            XAttribute attribute = new("foobar", "foo   bar");
            string[] expected = ["foo", "bar"];
            string[] actual = attribute.ToStrings();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestToBools()
        {
            XAttribute attribute = new("foo", "true false true");
            bool[] expected = { true, false, true };
            bool[] actual = attribute.ToBools();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestToBytes()
        {
            XAttribute attribute = new("foo", "2 3 5");
            byte[] expected = { 2, 3, 5 };
            byte[] actual = attribute.ToBytes();

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
