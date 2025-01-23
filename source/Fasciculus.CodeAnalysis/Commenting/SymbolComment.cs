using Fasciculus.Xml;
using System.IO;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolComment
    {
        public static SymbolComment Empty => new(XDocument.Parse("<comment />"));

        private readonly XDocument document;

        public string Summary => Convert(document.Root?.Element("summary"));

        public SymbolComment(XDocument document)
        {
            this.document = document;
        }

        public SymbolComment Clone()
            => new(document);

        public void MergeWith(SymbolComment other)
            => SymbolCommentMerger.Merge(document, other.document);

        internal void Accept(XWalker visitor)
            => visitor.VisitDocument(document);

        public static SymbolComment FromFile(FileInfo file)
        {
            if (file.Exists)
            {
                XDocument document = XDocument.Load(file.FullName);
                XElement? root = document.Root;

                if (root is not null && root.Name.LocalName == "comment")
                {
                    return new(document);
                }
            }

            return Empty;
        }

        private static string Convert(XElement? element)
            => SymbolCommentConverter.Convert(element);
    }
}
