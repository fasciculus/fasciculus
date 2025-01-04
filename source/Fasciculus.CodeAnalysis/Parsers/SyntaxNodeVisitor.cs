using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Parsers
{
    /// <summary>
    /// A visitor for syntax nodes.
    /// </summary>
    public class SyntaxNodeVisitor
    {
        /// <summary>
        /// Visits the given <paramref name="nodes"/> and their children unless a handler returns <c>false</c>.
        /// </summary>
        protected virtual bool Visit(IEnumerable<SyntaxNode> nodes)
        {
            foreach (SyntaxNode node in nodes)
            {
                if (!Visit(node))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Visits the given <paramref name="node"/> and its children unless a handler returns <c>false</c>.
        /// </summary>
        protected virtual bool Visit(SyntaxNode node)
        {
            SyntaxKind kind = node.Kind();

            switch (kind)
            {
                case SyntaxKind.IdentifierName: return On<IdentifierNameSyntax>(node, OnIdentifierName);
                case SyntaxKind.NamespaceDeclaration: return On<NamespaceDeclarationSyntax>(node, OnNamespaceDeclaration);
                case SyntaxKind.UsingDirective: return On<UsingDirectiveSyntax>(node, OnUsingDirective);

                default:
                    Debug.WriteLine($"unhandled kind {kind}");
                    return true;
            }
        }

        private static bool On<T>(SyntaxNode node, Func<T, bool> handler)
            where T : notnull, SyntaxNode
        {
            if (node is T t)
            {
                return handler(t);
            }

            return true;
        }

        /// <summary>
        /// Handles a IdentifierName.
        /// </summary>
        protected virtual bool OnIdentifierName(IdentifierNameSyntax node)
            => Visit(node.ChildNodes());

        /// <summary>
        /// Handles a NamespaceDeclaration.
        /// </summary>
        protected virtual bool OnNamespaceDeclaration(NamespaceDeclarationSyntax node)
            => Visit(node.ChildNodes());

        /// <summary>
        /// Handles a UsingDirective.
        /// </summary>
        protected virtual bool OnUsingDirective(UsingDirectiveSyntax node)
            => Visit(node.ChildNodes());
    }
}
