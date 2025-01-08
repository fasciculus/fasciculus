using System.Xml;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolComment
    {
        public XmlDocument Document { get; }

        private XmlElement? summary;

        public bool HasSummary => summary is not null;

        public string Summary => summary?.InnerXml ?? string.Empty;

        public SymbolComment(XmlDocument document)
        {
            Document = document;

            summary = FindElement(document, "summary");
        }

        private static XmlElement? FindElement(XmlDocument document, string name)
        {
            XmlNodeList nodes = document.GetElementsByTagName(name);

            if (nodes.Count > 0)
            {
                XmlNode? node = nodes[0];

                if (node is not null && node is XmlElement element)
                {
                    return element;
                }
            }

            return null;
        }

        public static readonly SymbolComment Empty = SymbolCommentFactory.CreateEmpty();
    }
}
