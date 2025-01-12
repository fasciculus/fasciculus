using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Support;
using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Xml.Linq;

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
            TargetFrameworks frameworks = context.Frameworks;
            string package = context.Package;

            link = parentLink.Append(name);
            classes = new();

            DefaultVisit(node);

            return new(name, link, frameworks, package, classes)
            {
                Comment = CreateComment(name)
            };
        }

        private SymbolComment CreateComment(string name)
        {
            FileInfo? file = context.ProjectDirectory?
                .Combine("Properties", "Comments", "Namespaces")
                .File(name + ".xml");

            if (file is not null && file.Exists)
            {
                try
                {
                    return new(XDocument.Load(file.FullName));
                }
                catch { }
            }

            return SymbolComment.Empty;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            classes.AddOrMergeWith(classCompiler.Compile(node, link));
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            SymbolCounters.Instance.Increment("InterfaceDeclarationSyntax");
            //base.VisitInterfaceDeclaration(node);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            SymbolCounters.Instance.Increment("EnumDeclarationSyntax");
            //base.VisitEnumDeclaration(node);
        }

        public override void VisitQualifiedName(QualifiedNameSyntax node) { }
    }
}
