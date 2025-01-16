using System.IO;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolComment
    {
        public static SymbolComment Empty => new(XDocument.Parse("<comment />"));

        private static SymbolCommentConverter converter = new();

        public XDocument Document { get; }

        public string Summary => Convert(Document.Root?.Element("summary"));

        public SymbolComment(XDocument document)
        {
            Document = document;
        }

        public static SymbolComment FromFile(FileInfo? file)
        {
            if (file is not null && file.Exists)
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
