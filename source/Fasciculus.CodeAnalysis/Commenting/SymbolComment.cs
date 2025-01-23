using Fasciculus.Xml;
using System.IO;
using System.Xml.Linq;
using static Fasciculus.CodeAnalysis.Commenting.CommentConstants;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolComment
    {
        public static SymbolComment Empty(CommentContext context)
            => new(context, XDocument.Parse(RootXml));

        private readonly CommentContext context;

        private readonly XDocument document;

        public string Summary => Convert(document.Root?.Element(SummaryName));

        public SymbolComment(CommentContext context, XDocument document)
        {
            this.context = context;
            this.document = new(document);
        }

        public SymbolComment Clone()
            => new(context, document);

        public void MergeWith(SymbolComment other)
            => context.Merger.Merge(other.document, document);

        internal void Accept(XWalker visitor)
            => visitor.VisitDocument(document);

        public static SymbolComment FromFile(CommentContext context, FileInfo file)
        {
            if (file.Exists)
            {
                XDocument document = XDocument.Load(file.FullName);
                XElement? root = document.Root;

                if (root is not null && root.Name.LocalName == RootName)
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
