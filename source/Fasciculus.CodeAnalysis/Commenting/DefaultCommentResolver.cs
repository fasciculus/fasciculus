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
                case "paramref": VisitParamref(element); break;
                case "see": VisitSee(element); break;
                case "typeparamref": VisitTypeparamref(element); break;
            }

            base.VisitElement(element);
        }

        private static void VisitPara(XElement element)
        {
            element.Name = "p";
        }

        private static void VisitParamref(XElement element)
        {
            XAttribute? name = element.Attribute("name");

            if (name is not null)
            {
                string content = name.Value;
                XElement code = new("c", content);

                element.ReplaceWith(code);
            }
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

        private static void VisitTypeparamref(XElement element)
        {
            XAttribute? name = element.Attribute("name");

            if (name is not null)
            {
                string content = name.Value;
                XElement code = new("c", content);

                element.ReplaceWith(code);
            }
        }
    }
}
