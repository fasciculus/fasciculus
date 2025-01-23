using Fasciculus.Xml;
using System.IO;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolComment
    {
        public static SymbolComment Empty(SymbolCommentContext context)
            => new(context, XDocument.Parse("<comment />"));

        private readonly SymbolCommentContext context;

        private readonly XDocument document;

        public string Summary => Convert(document.Root?.Element("summary"));

        public SymbolComment(SymbolCommentContext context, XDocument document)
        {
            this.context = context;
            this.document = new(document);
        }

        public SymbolComment Clone()
            => new(context, document);

        public void MergeWith(SymbolComment other)
            => SymbolCommentMerger.Merge(document, other.document);

        internal void Accept(XWalker visitor)
            => visitor.VisitDocument(document);

        public static SymbolComment FromFile(SymbolCommentContext context, FileInfo file)
        {
            if (file.Exists)
            {
                XDocument document = XDocument.Load(file.FullName);
                XElement? root = document.Root;

                if (root is not null && root.Name.LocalName == "comment")
                {
                    return new(context, document);
                }
            }

            return Empty(context);
        }

        private static string Convert(XElement? element)
            => SymbolCommentConverter.Convert(element);
    }
}
