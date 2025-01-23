using Fasciculus.Xml;
using System.Text;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class DefaultCommentFormatter : ICommentFormatter
    {
        private class Visitor : XVisitor
        {
            public StringBuilder Result { get; } = new();

            public override void VisitElement(XElement element)
            {
                if (element.IsEmpty)
                {
                    Result.Append('<').Append(element.Name).Append(" />");
                }
                else
                {
                    Result.Append('<').Append(element.Name).Append('>');
                    base.VisitElement(element);
                    Result.Append("</").Append(element.Name).Append('>');
                }
            }

            public override void VisitText(XText text)
            {
                Result.Append(text.Value);
            }
        }

        public string Format(XElement? element)
        {
            if (element is null)
            {
                return string.Empty;
            }

            Visitor visitor = new();

            visitor.VisitNodes(element.Nodes());

            return visitor.Result.ToString();
        }
    }
}
