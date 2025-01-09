using Fasciculus.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Fasciculus.Core.Tests.Xml
{
    [TestClass]
    public class XmlTests
    {
        [TestMethod]
        public void Test()
        {
            string xml = "<comment><summary>Test<p>Para with <c>code</c></p></summary></comment>";
            XDocument document = XDocument.Parse(xml);
            XElement summary = document.Root!.Element("summary")!;
            StringBuilder sb = new();

            Walk(summary.Nodes(), sb);

            Assert.AreEqual("Test<p>Para with <c>code</c></p>", sb.ToString());
        }

        private void Walk(IEnumerable<XNode> nodes, StringBuilder sb)
        {
            nodes.Apply(n => { Walk(n, sb); });
        }

        private void Walk(XNode node, StringBuilder sb)
        {
            if (node is XElement element)
            {
                IEnumerable<XNode> nodes = element.Nodes();

                if (nodes.Any())
                {
                    sb.Append('<').Append(element.Name).Append('>');
                    Walk(nodes, sb);
                    sb.Append("</").Append(element.Name).Append('>');
                }
                else
                {
                    sb.Append(element.Name);
                }
            }
            else if (node is XText text)
            {
                sb.Append(text.Value);
            }
        }

    }
}
