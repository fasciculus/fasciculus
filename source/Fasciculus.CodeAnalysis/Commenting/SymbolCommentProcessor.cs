using Fasciculus.CodeAnalysis.Compilers;
using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Xml;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolCommentProcessor : XDocumentWalker
    {
        private static string[] HandledElements
            = ["c", "code", "comment", "p", "para", "see", "summary", "typeparam"];

        private readonly SymbolIndices indices;

        public SymbolCommentProcessor(SymbolIndices indices)
        {
            this.indices = indices;

            UnhandledCommentElements.Instance.Handled(HandledElements);
        }

        public void Process()
        {
            indices.Symbols.Values.Apply(Process);
        }

        private void Process(Symbol symbol)
        {
            XDocument document = symbol.Comment.Document;

            Visit(document);
        }

        public override void Visit(XElement element)
        {
            string name = element.Name.LocalName;

            UnhandledCommentElements.Instance.Used(name);

            switch (name)
            {
                case "para": VisitPara(element); break;
                case "see": VisitSee(element); break;
            }

            base.Visit(element);
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
                string content = cref.Value.Replace("{", "&lt;").Replace("}", "&gt;");
                XElement code = new("c", content);

                element.ReplaceWith(code);
            }
        }
    }
}
