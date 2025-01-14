using System.Collections.Generic;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Compilers.Models
{
    public class CommentInfo
    {
        public XDocument Document { get; }

        private readonly Stack<XElement> elements = [];

        public XElement Top => elements.Peek();

        public CommentInfo()
        {
            Document = XDocument.Parse("<comment></comment>");
            elements.Push(Document.Root!);
        }

        public void Push(string name)
        {
            elements.Push(new XElement(name));
        }

        public XElement Pop()
        {
            return elements.Pop();
        }
    }
}
