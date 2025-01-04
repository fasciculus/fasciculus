using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Parsers
{
    /// <summary>
    /// CompilationUnit parser.
    /// </summary>
    public class CompilationUnitParser : SyntaxNodeVisitor
    {
        private readonly CompilationUnitSyntax compilationUnit;

        private NamespaceCollection namespaces = [];

        /// <summary>
        /// Initializes a new instance of this parser.
        /// </summary>
        /// <param name="compilationUnit"></param>
        public CompilationUnitParser(CompilationUnitSyntax compilationUnit)
        {
            this.compilationUnit = compilationUnit;
        }

        private void Clear()
        {
            namespaces = [];
        }

        /// <summary>
        /// Parses the compilation unit.
        /// </summary>
        public NamespaceCollection Parse()
        {
            Clear();
            Visit(compilationUnit.ChildNodes());

            return namespaces;
        }

        /// <summary>
        /// Handles a AttributeList.
        /// </summary>
        protected override void OnAttributeList(AttributeListSyntax node) { }

        /// <summary>
        /// Handles a IdentifierName.
        /// </summary>
        protected override void OnIdentifierName(IdentifierNameSyntax node) { }

        /// <summary>
        /// Handles a NamespaceDeclaration.
        /// </summary>
        protected override void OnNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            namespaces.Add(node.Name.ToString());
        }

        /// <summary>
        /// Handles a UsingDirective.
        /// </summary>
        protected override void OnUsingDirective(UsingDirectiveSyntax node) { }
    }
}
