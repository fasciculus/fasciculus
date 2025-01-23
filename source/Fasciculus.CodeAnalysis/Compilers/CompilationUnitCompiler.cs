using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Extensions;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler : CompilerBase
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly DirectoryInfo namespaceCommentsDirectory;

        private readonly CommentContext commentContext;

        private readonly AccessorsCompiler accessorsCompiler;

        private UriPath Source { get; set; } = UriPath.Empty;

        private CompilationUnitInfo compilationUnit = new();

        public CompilationUnitCompiler(CompilerContext context)
            : base(context, SyntaxWalkerDepth.StructuredTrivia)
        {
            namespaceCommentsDirectory = context.CommentsDirectory.Combine("Namespaces");
            commentContext = context.CommentContext;

            accessorsCompiler = new(context);
        }

        public virtual CompilationUnitInfo Compile(CompilationUnitSyntax node)
        {
            using Locker locker = Locker.Lock(mutex);

            Source = node.GetSource(projectDirectory);

            compilationUnit = new();

            node.Accept(this);

            return compilationUnit;
        }

        protected virtual SymbolName GetName(SyntaxToken identifier, TypeParameterListSyntax? typeParameters)
        {
            string name = identifier.Text;

            if (typeParameters is null || typeParameters.Parameters.Count == 0)
            {
                return new(name);
            }

            string[] parameters = [.. typeParameters.Parameters.Select(p => p.Identifier.Text)];

            return new(name, parameters);
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            // compilation_unit
            //   : extern_alias_directive* using_directive* global_attributes? namespace_member_declaration*
            //
            // CompilationUnit: UsingDirective* AttributeList* NamespaceDeclaration*

            nodeDebugger.Add(node);

            base.VisitCompilationUnit(node);
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            // UsingDirective
            // : IdentifierName
            // | QualifiedName

            nodeDebugger.Add(node);

            base.VisitUsingDirective(node);
        }

    }
}
