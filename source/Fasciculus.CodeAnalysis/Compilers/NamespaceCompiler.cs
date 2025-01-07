using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class NamespaceCompiler : FilteredCompiler
    {
        private static readonly SyntaxKind[] AcceptedKinds =
        [
            SyntaxKind.ClassDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.QualifiedName,
        ];

        private readonly TaskSafeMutex mutex = new();

        private readonly TargetFramework framework;

        private UriPath link = new();

        public NamespaceCompiler(TargetFramework framework)
            : base(AcceptedKinds)
        {
            this.framework = framework;
        }

        public NamespaceSymbol Compile(NamespaceDeclarationSyntax node, UriPath parentLink)
        {
            using Locker locker = Locker.Lock(mutex);

            SymbolName name = new(node.Name.ToString());

            link = parentLink.Append(name.Name);

            NamespaceSymbol @namespace = new(name, link, framework);

            DefaultVisit(node);

            return @namespace;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            //base.VisitClassDeclaration(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            //base.VisitInterfaceDeclaration(node);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            //base.VisitEnumDeclaration(node);
        }

        public override void VisitQualifiedName(QualifiedNameSyntax node) { }
    }
}
