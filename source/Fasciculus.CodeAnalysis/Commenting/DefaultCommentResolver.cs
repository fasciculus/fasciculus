using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.Xml;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class DefaultCommentResolver : XVisitor, ICommentResolver
    {
        private readonly ICommentDebugger debugger;

        public DefaultCommentResolver(ICommentDebugger debugger)
        {
            this.debugger = debugger;
        }

        public XElement? Resolve(XElement? element)
        {
            if (element is null)
            {
                return null;
            }

            VisitNodes(element.Nodes());

            return element;
        }

        public override void VisitElement(XElement element)
        {
            string name = element.Name.LocalName;

            debugger.Used(name);

            switch (name)
            {
                case "para": VisitPara(element); break;
                case "see": VisitSee(element); break;
            }

            base.VisitElement(element);
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
