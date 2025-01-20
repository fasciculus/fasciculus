using Fasciculus.CodeAnalysis.Compilers.Builders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
        protected readonly Stack<CommentBuilder> commentBuilders = [];
        protected readonly Stack<ICommentReceiver> commentReceivers = [];

        public void PushComment()
        {
            commentBuilders.Push(CommentBuilder.Create());
        }

        public void PopComment()
        {
            CommentBuilder builder = commentBuilders.Pop();

            if (commentReceivers.Count > 0)
            {
                commentReceivers.Peek().Comment = builder.Build();
            }
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            //if (commentInfos.Count == 0)
            //{
            //    SyntaxNode? failingNode = ancestors.FirstOrDefault(n => n.HasStructuredTrivia);
            //    SyntaxKind? failingKind = failingNode?.Kind();

            //    throw Ex.InvalidOperation($"missing push in '{failingKind}'");
            //}

            base.VisitDocumentationCommentTrivia(node);
        }

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

        public override void VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
        {
            base.VisitXmlCrefAttribute(node);

            string cref = node.Cref.GetText().ToString();

            commentBuilders.Peek().Add(new XAttribute("cref", cref));
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

        public override void VisitXmlTextAttribute(XmlTextAttributeSyntax node)
        {
            base.VisitXmlTextAttribute(node);

            string name = node.Name.LocalName.ValueText;
            string value = string.Join("", node.TextTokens.Select(x => x.Text));

            commentBuilders.Peek().Add(new XAttribute(name, value));
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
            // HasTrivia
            // AttributeList
            // : AttributeTargetSpecifier Attribute
            // | Attribute

            nodeDebugger.Add(node);

            //PushComment();

            base.VisitAttributeList(node);

            //PopComment();
        }

    }
}