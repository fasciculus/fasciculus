using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class CommentBuilder
    {
        private readonly CommentContext commentContext;

        private readonly XDocument document;

        private readonly Stack<XElement> elements = [];

        public XElement Top => elements.Peek();

        public CommentBuilder(CommentContext commentContext)
        {
            this.commentContext = commentContext;

            document = XDocument.Parse("<comment/>");
            elements.Push(document.Root!);
        }

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
            => new(commentContext, document);
    }
}
