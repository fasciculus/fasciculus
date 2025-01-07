using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.CodeAnalysis.Models;
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

        public NamespaceCompiler(TargetFramework framework)
            : base(AcceptedKinds)
        {
            this.framework = framework;
        }

        public NamespaceList Compile(NamespaceDeclarationSyntax node)
        {
            using Locker locker = Locker.Lock(mutex);

            NamespaceList namespaces = new();
            SymbolName name = new(node.Name.ToString());
            NamespaceSymbol @namespace = new(name, framework);

            DefaultVisit(node);

            return new([@namespace]);
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
