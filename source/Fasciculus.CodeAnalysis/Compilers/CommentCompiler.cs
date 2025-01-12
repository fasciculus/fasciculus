using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CommentCompiler : FilteredCompiler
    {
        private static readonly SyntaxKind[] HandledSymbols =
        [
            SyntaxKind.XmlElement,
            SyntaxKind.XmlElementStartTag,
            SyntaxKind.XmlElementEndTag,
            SyntaxKind.XmlEmptyElement,

            SyntaxKind.XmlNameAttribute,
            SyntaxKind.XmlName,

            SyntaxKind.XmlCrefAttribute,
            SyntaxKind.QualifiedCref,
            SyntaxKind.TypeCref,
            SyntaxKind.NameMemberCref,
            SyntaxKind.CrefParameterList,
            SyntaxKind.CrefParameter,

            SyntaxKind.GenericName,
            SyntaxKind.PredefinedType,
            SyntaxKind.NullableType,
            SyntaxKind.ArrayType,
            SyntaxKind.ArrayRankSpecifier,
            SyntaxKind.OmittedArraySizeExpression,
            SyntaxKind.TypeArgumentList,
            SyntaxKind.IdentifierName,

            SyntaxKind.XmlText,
            SyntaxKind.XmlTextAttribute,
        ];

        private readonly TaskSafeMutex mutex = new();

        private readonly SymbolCommentFactory commentFactory = new();

        private StringBuilder stringBuilder = new();

        public CommentCompiler()
            : base(HandledSymbols, SyntaxWalkerDepth.StructuredTrivia)
        {
        }

        public SymbolComment Compile(DocumentationCommentTriviaSyntax node)
        {
            using Locker locker = Locker.Lock(mutex);

            stringBuilder = new();

            DefaultVisit(node);

            string xml = "<comment>\r\n" + stringBuilder.ToString() + "</comment>";

            return commentFactory.Create(xml);
        }

        public override void VisitXmlTextAttribute(XmlTextAttributeSyntax node)
        {
            stringBuilder.Append(' ');

            base.VisitXmlTextAttribute(node);
        }

        public override void VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
        {
            stringBuilder.Append(' ');

            base.VisitXmlCrefAttribute(node);
        }

        public override void VisitXmlNameAttribute(XmlNameAttributeSyntax node)
        {
            stringBuilder.Append(' ');

            base.VisitXmlNameAttribute(node);
        }

        public override void VisitToken(SyntaxToken token)
        {
            stringBuilder.Append(token.Text);

            base.VisitToken(token);
        }
    }
}
