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

    internal class SymbolComment : ISymbolComment
    {
        public static SymbolComment Empty(CommentContext context)
            => new(context, XDocument.Parse(RootXml));

        public CommentContext Context { get; }

        private readonly XDocument document;

        public string Summary => ResolveAndFormat(document.Root?.Element(SummaryName));

        public SymbolComment(CommentContext context, XDocument document)
        {
            Context = context;

            this.document = new(document);
        }

        public SymbolComment Clone()
            => new(Context, document);

        public void MergeWith(SymbolComment other)
            => Context.Merger.Merge(other.document, document);

        private string ResolveAndFormat(XElement? element)
            => Context.Formatter.Format(Context.Resolver.Resolve(element));

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
