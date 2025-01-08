using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Support;
using Fasciculus.Collections;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolCommentProcessor
    {
        private readonly SymbolIndices indices;

        public SymbolCommentProcessor(SymbolIndices indices)
        {
            this.indices = indices;
        }

        public void Process()
        {
            indices.Symbols.Values.Apply(Process);
        }

        private void Process(Symbol symbol)
        {
            XDocument document = symbol.Comment.Document;

            Visit(document.Root);
        }

        private void Visit(XElement? element)
        {
            if (element is not null)
            {
                CommentElementReporter.Instance.Used(element.Name.LocalName);

                switch (element.Name.LocalName)
                {
                    case "para": VisitPara(element); break;
                    case "see": VisitSee(element); break;
                }

                element.Elements().Apply(Visit);
            }
        }

        private static void VisitPara(XElement element)
        {
            element.Name = "p";
        }

        private static void VisitSee(XElement element)
        {
            XAttribute? cref = element.Attribute("cref");

            if (cref is not null)
            {
                XElement code = new("c", cref.Value);

                element.ReplaceWith(code);
            }
        }
    }
}
