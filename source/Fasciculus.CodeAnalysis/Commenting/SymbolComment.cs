using System.Xml;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolComment
    {
        public XDocument Document { get; }

        private XElement? summary;

        public bool HasSummary => summary is not null;

        public string Summary => InnerXml(summary);

        public SymbolComment(XDocument document)
        {
            Document = document;

            summary = FindElement(document, "summary");
        }

        private static XElement? FindElement(XDocument document, string name)
        {
            return document.Root?.Element(name);
        }

        private static string InnerXml(XElement? element)
        {
            if (element is not null)
            {
                using XmlReader reader = element.CreateReader();

                reader.MoveToContent();

                return reader.ReadInnerXml();
            }

            return string.Empty;
        }

        public static readonly SymbolComment Empty = SymbolCommentFactory.CreateEmpty();
    }
}
