using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
