using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class NamespaceCompiler : FilteredCompiler
    {
        private static readonly SyntaxKind[] HandledSymbols =
        [
            SyntaxKind.ClassDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.QualifiedName,
        ];

        private readonly TaskSafeMutex mutex = new();

        private readonly CompilerContext context;

        private readonly ClassCompiler classCompiler;

        private UriPath link = new();
        private ClassList classes = new();

        public NamespaceCompiler(CompilerContext context)
            : base(HandledSymbols)
        {
            this.context = context;

            classCompiler = new(context);
        }

        public NamespaceSymbol Compile(NamespaceDeclarationSyntax node, UriPath parentLink)
        {
            using Locker locker = Locker.Lock(mutex);

            SymbolName name = new(node.Name.ToString());

            link = parentLink.Append(name);
            classes = new();

            DefaultVisit(node);

            return new(name, link, context.Frameworks, classes);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            classes.AddOrMergeWith(classCompiler.Compile(node, link));
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
