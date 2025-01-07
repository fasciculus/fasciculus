using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Xml;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CommentCompiler : FilteredCompiler
    {
        private static readonly SyntaxKind[] AcceptedKinds =
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
            SyntaxKind.TypeArgumentList,
            SyntaxKind.IdentifierName,

            SyntaxKind.XmlText,
        ];

        private readonly TaskSafeMutex mutex = new();

        private StringBuilder stringBuilder = new();

        public CommentCompiler()
            : base(AcceptedKinds, SyntaxWalkerDepth.StructuredTrivia)
        {
        }

        public string Compile(DocumentationCommentTriviaSyntax node)
        {
            using Locker locker = Locker.Lock(mutex);

            stringBuilder = new();

            DefaultVisit(node);

            string xml = "<comment>\r\n" + stringBuilder.ToString() + "</comment>";

            try
            {
                XmlDocument document = new();

                document.LoadXml(xml);
            }
            catch
            {
                return string.Empty;
            }

            return xml;
        }

        public override void VisitPredefinedType(PredefinedTypeSyntax node)
        {
            base.VisitPredefinedType(node);
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
