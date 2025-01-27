using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Extensions;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler : CompilerBase
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly DirectoryInfo namespaceCommentsDirectory;

        private readonly CommentContext commentContext;

        private readonly AccessorsCompiler accessorsCompiler;

        private UriPath Source { get; set; } = UriPath.Empty;

        protected readonly Stack<NamespaceBuilder> namespaceBuilders = [];

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

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            // compilation_unit
            //   : extern_alias_directive* using_directive* global_attributes? namespace_member_declaration*
            //
            // CompilationUnit: UsingDirective* AttributeList* NamespaceDeclaration*

            nodeDebugger.Add(node);

            base.VisitCompilationUnit(node);
        }

        private UriPath CreateNamespaceLink(SymbolName name)
        {
            if (namespaceBuilders.Count > 0)
            {
                return namespaceBuilders.Peek().Link.Append(name);
            }

            return new(package, name);
        }

        private void PushNamespace(SymbolName name)
        {
            UriPath link = CreateNamespaceLink(name);

            NamespaceBuilder builder = new(commentContext, namespaceCommentsDirectory)
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = new()
            };

            namespaceBuilders.Push(builder);
            typeReceivers.Push(builder);
        }

        private void PopNamespace()
        {
            typeReceivers.Pop();

            NamespaceBuilder builder = namespaceBuilders.Pop();
            NamespaceSymbol @namespace = builder.Build();

            compilationUnit.AddOrMergeWith(@namespace);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            // HasTrivia: True
            // NamespaceDeclaration
            // : QualifiedName ClassDeclaration* InterfaceDeclaration* EnumDeclaration*

            nodeDebugger.Add(node);

            SymbolName name = new(node.Name.ToString());

            PushNamespace(name);

            base.VisitNamespaceDeclaration(node);

            PopNamespace();
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
