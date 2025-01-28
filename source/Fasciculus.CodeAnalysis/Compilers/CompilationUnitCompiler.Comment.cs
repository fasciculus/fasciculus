using Fasciculus.CodeAnalysis.Compilers.Builders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        public override void VisitXmlCDataSection(XmlCDataSectionSyntax node)
        {
            base.VisitXmlCDataSection(node);

            string text = string.Join("", node.TextTokens.Select(x => x.Text));

            commentBuilders.Peek().Add(new XCData(text));
        }

        public override void VisitXmlComment(XmlCommentSyntax node)
        {
            base.VisitXmlComment(node);

            string text = string.Join("", node.TextTokens.Select(x => x.Text));

            commentBuilders.Peek().Add(new XComment(text));
        }

        public override void VisitXmlElement(XmlElementSyntax node)
        {
            CommentBuilder builder = commentBuilders.Peek();
            string name = node.StartTag.Name.LocalName.ValueText;

            builder.Push(name);

            base.VisitXmlElement(node);

            builder.Add(builder.Pop());
        }

        public override void VisitXmlEmptyElement(XmlEmptyElementSyntax node)
        {
            CommentBuilder builder = commentBuilders.Peek();
            string name = node.Name.LocalName.ValueText;

            builder.Push(name);

            base.VisitXmlEmptyElement(node);

            builder.Add(builder.Pop());
        }

        public override void VisitXmlText(XmlTextSyntax node)
        {
            base.VisitXmlText(node);

            string text = string.Join("", node.TextTokens.Select(x => x.Text));

            commentBuilders.Peek().Add(new XText(text));
        }

        public override void VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
        {
            //base.VisitXmlCrefAttribute(node);

            string cref = node.Cref.GetText().ToString();

            commentBuilders.Peek().Add(new XAttribute("cref", cref));
        }

        public override void VisitXmlNameAttribute(XmlNameAttributeSyntax node)
        {
            base.VisitXmlNameAttribute(node);

            string name = node.Name.LocalName.ValueText;
            string value = node.Identifier.ToFullString();

            commentBuilders.Peek().Add(new XAttribute(name, value));
        }

        public override void VisitXmlTextAttribute(XmlTextAttributeSyntax node)
        {
            base.VisitXmlTextAttribute(node);

            string name = node.Name.LocalName.ValueText;
            string value = string.Join("", node.TextTokens.Select(x => x.Text));

            commentBuilders.Peek().Add(new XAttribute(name, value));
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            base.VisitDocumentationCommentTrivia(node);
        }
    }
}