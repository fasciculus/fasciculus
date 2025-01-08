using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class ClassCompiler : FilteredCompiler
    {
        private static readonly SyntaxKind[] AcceptedKinds =
        [
            SyntaxKind.AttributeList,
            SyntaxKind.TypeParameterList,
            SyntaxKind.TypeParameterConstraintClause,
            SyntaxKind.BaseList,
            SyntaxKind.FieldDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.IndexerDeclaration,
            SyntaxKind.EventFieldDeclaration,
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.DestructorDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.OperatorDeclaration,
            SyntaxKind.ConversionOperatorDeclaration,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.SingleLineDocumentationCommentTrivia,
            SyntaxKind.MultiLineDocumentationCommentTrivia,
        ];

        private readonly TaskSafeMutex mutex = new();

        private readonly TargetFrameworks frameworks;

        private readonly CommentCompiler commentCompiler = new();

        private UriPath link = new();
        private string comment = string.Empty;

        public ClassCompiler(TargetFramework framework)
            : base(AcceptedKinds, SyntaxWalkerDepth.StructuredTrivia)
        {
            this.frameworks = new(framework);
        }

        public ClassSymbol Compile(ClassDeclarationSyntax node, UriPath parentLink)
        {
            using Locker locker = Locker.Lock(mutex);

            string untyped = node.Identifier.ToString();
            TypeParameterListSyntax? parameterList = node.TypeParameterList;
            IEnumerable<string> parameters = parameterList is null ? [] : parameterList.Parameters.Select(p => p.Identifier.ToString());
            SymbolName name = new(untyped, parameters);

            link = parentLink.Append(name.Mangled);
            comment = string.Empty;

            DefaultVisit(node);

            ClassSymbol result = new(name, link, frameworks);

            result.Comment = comment;

            return result;
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
        }

        public override void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
        }

        public override void VisitBaseList(BaseListSyntax node)
        {
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
        }

        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            comment = commentCompiler.Compile(node);
        }
    }
}
