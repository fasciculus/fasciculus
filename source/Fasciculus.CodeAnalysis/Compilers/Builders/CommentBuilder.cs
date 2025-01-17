using Fasciculus.CodeAnalysis.Commenting;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class CommentBuilder
    {
        public XDocument Document { get; }

        private readonly Stack<XElement> elements = [];

        public XElement Top => elements.Peek();

        public CommentBuilder()
        {
            Document = XDocument.Parse("<comment>\r\n</comment>");
            elements.Push(Document.Root!);
        }

        public static CommentBuilder Create()
            => new();

        public void Add(XObject content)
            => Top.Add(content);

        public void Push(string name)
        {
            elements.Push(new XElement(name));
        }

        public XElement Pop()
        {
            return elements.Pop();
        }

        public SymbolComment Build()
            => new(Document);
    }
}
