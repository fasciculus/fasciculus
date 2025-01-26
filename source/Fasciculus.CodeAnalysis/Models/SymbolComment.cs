using Fasciculus.CodeAnalysis.Commenting;
using System.IO;
using System.Xml.Linq;
using static Fasciculus.CodeAnalysis.Commenting.CommentConstants;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface ISymbolComment
    {
        public string Summary { get; }
    }

    public class SymbolComment : ISymbolComment
    {
        public static SymbolComment Empty(CommentContext context)
            => new(context, XDocument.Parse(RootXml));

        private readonly CommentContext context;

        private readonly XDocument document;

        public string Summary => ResolveAndFormat(document.Root?.Element(SummaryName));

        public SymbolComment(CommentContext context, XDocument document)
        {
            this.context = context;
            this.document = new(document);
        }

        public SymbolComment Clone()
            => new(context, document);

        public void MergeWith(SymbolComment other)
            => context.Merger.Merge(other.document, document);

        private string ResolveAndFormat(XElement? element)
            => context.Formatter.Format(context.Resolver.Resolve(element));

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
    }
}
