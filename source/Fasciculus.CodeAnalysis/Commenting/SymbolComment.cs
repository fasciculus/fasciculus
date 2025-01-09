using Fasciculus.Xml;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolComment
    {
        public XDocument Document { get; }

        private readonly XElement? summary;

        public bool HasSummary => summary is not null;

        public string Summary => summary.InnerXml().Trim();

        public SymbolComment(XDocument document)
        {
            Document = document;

            summary = document.Root?.Element("summary");
        }

        public static readonly SymbolComment Empty = SymbolCommentFactory.CreateEmpty();
    }
}
