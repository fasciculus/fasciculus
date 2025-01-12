using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Support;
using Fasciculus.Net.Navigating;
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
        private static readonly SyntaxKind[] HandledSymbols =
        [
            SyntaxKind.AttributeList,
            SyntaxKind.Attribute,
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
            SyntaxKind.IdentifierName
        ];

        private readonly TaskSafeMutex mutex = new();

        private readonly CompilerContext context;

        private readonly ModifiersFactory modifiersFactory = new();
        private readonly CommentCompiler commentCompiler = new();

        private SymbolComment comment = SymbolComment.Empty;

        public ClassCompiler(CompilerContext context)
            : base(HandledSymbols, SyntaxWalkerDepth.StructuredTrivia)
        {
            this.context = context;
        }

        private void Clear()
        {
            comment = SymbolComment.Empty;
        }

        public ClassSymbol Compile(ClassDeclarationSyntax node, UriPath parentLink)
        {
            using Locker locker = Locker.Lock(mutex);

            string untyped = node.Identifier.ToString();
            TypeParameterListSyntax? parameterList = node.TypeParameterList;
            IEnumerable<string> parameters = parameterList is null ? [] : parameterList.Parameters.Select(p => p.Identifier.ToString());
            SymbolName name = new(untyped, parameters);
            UriPath link = parentLink.Append(name.Mangled);
            TargetFrameworks frameworks = context.Frameworks;
            string package = context.Package;
            SymbolModifiers modifiers = modifiersFactory.Create(node.Modifiers);

            Clear();

            DefaultVisit(node);

            return new(name, link, frameworks, package)
            {
                Modifiers = modifiers,
                Comment = comment
            };
        }

        public override void VisitAttribute(AttributeSyntax node)
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
            SymbolCounters.Instance.Increment("FieldDeclarationSyntax");
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            SymbolCounters.Instance.Increment("PropertyDeclarationSyntax");
        }

        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            SymbolCounters.Instance.Increment("IndexerDeclarationSyntax");
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            SymbolCounters.Instance.Increment("EventFieldDeclarationSyntax");
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            SymbolCounters.Instance.Increment("ConstructorDeclarationSyntax");
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            MethodCompiler compiler = new();

            compiler.Compile(node);
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            SymbolCounters.Instance.Increment("OperatorDeclarationSyntax");
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            SymbolCounters.Instance.Increment("ConversionOperatorDeclarationSyntax");
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            SymbolCounters.Instance.Increment("ClassDeclarationSyntax");
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            comment = commentCompiler.Compile(node);
        }
    }
}
