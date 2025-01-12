using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Support;
using Fasciculus.Net.Navigating;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CompilationCompiler : FilteredCompiler
    {
        private static readonly SyntaxKind[] HandledSymbols =
        [
            SyntaxKind.NamespaceDeclaration,
            SyntaxKind.AttributeList,
            SyntaxKind.UsingDirective
        ];

        private readonly TaskSafeMutex mutex = new();

        private readonly NamespaceCompiler namespaceCompiler;

        private NamespaceList namespaces = new();
        private UriPath link = new();

        public CompilationCompiler(CompilerContext context)
            : base(HandledSymbols)
        {
            namespaceCompiler = new(context);
        }

        public CompilationUnit Compile(CompilationUnitSyntax compilationUnit, UriPath parentLink)
        {
            using Locker locker = Locker.Lock(mutex);

            namespaces = new();
            link = parentLink;

            DefaultVisit(compilationUnit);

            return new(namespaces);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            namespaces.AddOrMergeWith(namespaceCompiler.Compile(node, link));
        }

        public override void VisitAttributeList(AttributeListSyntax node) { }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            SymbolCounters.Instance.Increment("UsingDirectiveSyntax");
        }
    }
}
