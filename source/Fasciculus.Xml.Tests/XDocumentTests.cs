using Fasciculus.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Fasciculus.Xml.Tests
{
    [TestClass]
    public class XDocumentTests : TestsBase
    {
        [TestMethod]
        public void Test()
        {
            FileInfo file = GetProjectDirectory().Combine("Resources").File("XDocumentTests.xml");
            XDocument document = XDocument.Load(file.FullName);
            using XStringWriter writer = new();

            document.Save(writer);

            string expected = file.ReadAllText();
            string actual = writer.ToString();

            Assert.AreEqual(expected, actual);

            string[] lines = file.ReadAllLines();

            expected = string.Join("\r\n", lines.Skip(1));
            actual = document.ToString();

            Assert.AreEqual(expected, actual);
        }
    }
}
