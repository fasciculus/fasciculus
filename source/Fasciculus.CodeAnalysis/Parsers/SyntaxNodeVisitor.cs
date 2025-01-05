using Fasciculus.Collections;
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
        /// Visits the given <paramref name="nodes"/> and their children.
        /// </summary>
        protected virtual void Visit(IEnumerable<SyntaxNode> nodes)
            => nodes.Apply(Visit);

        /// <summary>
        /// Visits the given <paramref name="node"/> and its children.
        /// </summary>
        protected virtual void Visit(SyntaxNode node)
        {
            SyntaxKind kind = node.Kind();

            switch (kind)
            {
                case SyntaxKind.AttributeList: On<AttributeListSyntax>(node, OnAttributeList); break;
                case SyntaxKind.ClassDeclaration: On<ClassDeclarationSyntax>(node, OnClassDeclaration); break;
                case SyntaxKind.EnumDeclaration: On<EnumDeclarationSyntax>(node, OnEnumDeclaration); break;
                case SyntaxKind.IdentifierName: On<IdentifierNameSyntax>(node, OnIdentifierName); break;
                case SyntaxKind.InterfaceDeclaration: On<InterfaceDeclarationSyntax>(node, OnInterfaceDeclaration); break;
                case SyntaxKind.NamespaceDeclaration: On<NamespaceDeclarationSyntax>(node, OnNamespaceDeclaration); break;
                case SyntaxKind.QualifiedName: On<QualifiedNameSyntax>(node, OnQualifiedName); break;
                case SyntaxKind.TypeParameter: On<TypeParameterSyntax>(node, OnTypeParameter); break;
                case SyntaxKind.TypeParameterList: On<TypeParameterListSyntax>(node, OnTypeParameterList); break;
                case SyntaxKind.UsingDirective: On<UsingDirectiveSyntax>(node, OnUsingDirective); break;

                default:
                    Debug.WriteLine($"unhandled kind {kind}");
                    Visit(node.ChildNodes());
                    break;
            }
        }

        /// <summary>
        /// Calls the given <paramref name="handler"/> if the given <paramref name="node"/> is of type <typeparamref name="T"/>.
        /// </summary>
        protected static void On<T>(SyntaxNode node, Action<T> handler)
            where T : notnull, SyntaxNode
        {
            if (node is T t)
            {
                handler(t);
            }
        }

        /// <summary>
        /// Handles a AttributeList.
        /// </summary>
        protected virtual void OnAttributeList(AttributeListSyntax node)
            => Visit(node.ChildNodes());

        /// <summary>
        /// Handles a ClassDeclaration.
        /// </summary>
        protected virtual void OnClassDeclaration(ClassDeclarationSyntax node)
            => Visit(node.ChildNodes());

        /// <summary>
        /// Handles a EnumDeclaration.
        /// </summary>
        protected virtual void OnEnumDeclaration(EnumDeclarationSyntax node)
            => Visit(node.ChildNodes());

        /// <summary>
        /// Handles a IdentifierName.
        /// </summary>
        protected virtual void OnIdentifierName(IdentifierNameSyntax node)
            => Visit(node.ChildNodes());

        /// <summary>
        /// Handles a InterfaceDeclaration.
        /// </summary>
        protected virtual void OnInterfaceDeclaration(InterfaceDeclarationSyntax node)
            => Visit(node.ChildNodes());

        /// <summary>
        /// Handles a NamespaceDeclaration.
        /// </summary>
        protected virtual void OnNamespaceDeclaration(NamespaceDeclarationSyntax node)
            => Visit(node.ChildNodes());

        /// <summary>
        /// Handles a QualifiedName.
        /// </summary>
        protected virtual void OnQualifiedName(QualifiedNameSyntax node)
            => Visit(node.ChildNodes());

        /// <summary>
        /// Handles a TypeParameter.
        /// </summary>
        protected virtual void OnTypeParameter(TypeParameterSyntax node)
            => Visit(node.ChildNodes());

        /// <summary>
        /// Handles a TypeParameterList.
        /// </summary>
        protected virtual void OnTypeParameterList(TypeParameterListSyntax node)
            => Visit(node.ChildNodes());

        /// <summary>
        /// Handles a UsingDirective.
        /// </summary>
        protected virtual void OnUsingDirective(UsingDirectiveSyntax node)
            => Visit(node.ChildNodes());
    }
}
