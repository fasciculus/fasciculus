using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolComment
    {
        public static readonly SymbolComment Empty = SymbolCommentFactory.CreateEmpty();

        private static SymbolCommentConverter converter = new();

        public XDocument Document { get; }

        public string Summary => Convert(Document.Root?.Element("summary"));

        public SymbolComment(XDocument document)
        {
            Document = document;
        }

        private static string Convert(XElement? element)
            => converter.Convert(element);
    }
}
