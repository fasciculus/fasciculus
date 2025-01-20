using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
        protected readonly Stack<NamespaceBuilder> namespaceBuilders = [];

        protected virtual UriPath CreateNamespaceLink(SymbolName name)
        {
            if (namespaceBuilders.Count > 0)
            {
                return namespaceBuilders.Peek().Link.Append(name);
            }

            return new(Package, name);
        }

        protected virtual void PushNamespace(SymbolName name)
        {
            UriPath link = CreateNamespaceLink(name);
            NamespaceBuilder builder = new(name, link, Framework, Package);

            namespaceBuilders.Push(builder);
            typeReceivers.Push(builder);
        }

        protected virtual void PopNamespace()
        {
            typeReceivers.Pop();

            NamespaceBuilder builder = namespaceBuilders.Pop();
            NamespaceSymbol @namespace = builder.Build();
            FileInfo commentFile = NamespaceCommentsDirectory.File($"{builder.Name}.xml");

            @namespace.Comment = SymbolComment.FromFile(commentFile);

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

    }
}