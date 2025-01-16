using Fasciculus.Xml;
using System.Text;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolCommentConverter
    {
        private class Walker : XWalker
        {
            public StringBuilder Result { get; } = new();

            public override void VisitElement(XElement element)
            {
                if (element.IsEmpty)
                {
                    Result.Append('<').Append(element.Name).Append("/>");
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

        public static string Convert(XElement? element)
        {
            if (element is not null)
            {
                Walker walker = new();

                walker.VisitNodes(element.Nodes());

                return walker.Result.ToString();
            }

            return string.Empty;
        }
    }
}
