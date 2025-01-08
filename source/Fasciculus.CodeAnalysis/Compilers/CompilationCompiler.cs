using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CompilationCompiler : FilteredCompiler
    {
        private static readonly SyntaxKind[] AcceptedKinds =
        [
            SyntaxKind.NamespaceDeclaration,
            SyntaxKind.UsingDirective
        ];

        private readonly TaskSafeMutex mutex = new();

        private readonly NamespaceCompiler namespaceCompiler;

        private NamespaceList namespaces;
        private UriPath link = new();

        public CompilationCompiler(TargetFramework framework)
            : base(AcceptedKinds)
        {
            namespaceCompiler = new(framework);
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

        public override void VisitUsingDirective(UsingDirectiveSyntax node) { }
    }
}
