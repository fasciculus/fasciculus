using Fasciculus.Xml;
using System.IO;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolComment
    {
        public static SymbolComment Empty(CommentContext context)
            => new(context, XDocument.Parse("<comment />"));

        private readonly CommentContext context;

        private readonly XDocument document;

        public string Summary => Convert(document.Root?.Element("summary"));

        public SymbolComment(CommentContext context, XDocument document)
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

        public static SymbolComment FromFile(CommentContext context, FileInfo file)
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
