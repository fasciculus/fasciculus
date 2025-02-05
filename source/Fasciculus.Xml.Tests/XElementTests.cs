using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Fasciculus.Xml.Tests
{
    [TestClass]
    public class XElementTests
    {
        [TestMethod]
        public void Test()
        {
            XElement root = new("foo");

            root.Add(new XElement("bar"));

            string expected = "<foo>\r\n  <bar />\r\n</foo>";
            string actual = root.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBool()
        {
            XElement element = new("foo");

            Assert.IsFalse(element.GetBool("bar"));

            element.SetBool("bar", true);

            Assert.IsTrue(element.GetBool("bar"));
        }

        [TestMethod]
        public void TestDateTime()
        {
            DateTime zero = DateTime.FromBinary(0);
            DateTime now = DateTime.Now;

            XElement element = new("foo");

            Assert.AreEqual(zero, element.GetDateTime("bar"));

            element.SetDateTime("bar", now);

            Assert.AreEqual(now.ToUniversalTime(), element.GetDateTime("bar"));
        }

        [TestMethod]
        public void TestDateTimeOffset()
        {
            DateTimeOffset zero = DateTimeOffset.FromUnixTimeSeconds(0);
            DateTimeOffset now = DateTimeOffset.Now;

            XElement element = new("foo");

            Assert.AreEqual(zero, element.GetDateTimeOffset("bar"));

            element.SetDateTimeOffset("bar", now);

            Assert.AreEqual(now, element.GetDateTimeOffset("bar"));
        }

        [TestMethod]
        public void TestStrings()
        {
            XElement element = new("foo");
            string[] expected = { "aa", "bb" };

            Assert.AreEqual(0, element.GetStrings("bar").Length);

            element.SetStrings("bar", expected);

            CollectionAssert.AreEqual(expected, element.GetStrings("bar"));
        }

        [TestMethod]
        public void TestBools()
        {
            XElement element = new("foo");
            bool[] expected = { true, false, true };

            Assert.AreEqual(0, element.GetBools("bar").Length);

            element.SetBools("bar", expected);

            CollectionAssert.AreEqual(expected, element.GetBools("bar"));
        }

        [TestMethod]
        public void TestDateTimes()
        {
            XElement element = new("foo");
            DateTime[] expected = { DateTime.Now, DateTime.Now.AddDays(1) };

            Assert.AreEqual(0, element.GetDateTimes("bar").Length);

            element.SetDateTimes("bar", expected);

            DateTime[] actual = [.. element.GetDateTimes("bar").Select(x => x.ToLocalTime())];

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
