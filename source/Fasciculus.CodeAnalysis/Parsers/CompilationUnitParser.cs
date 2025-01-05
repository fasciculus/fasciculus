using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Parsers
{
    /// <summary>
    /// CompilationUnit parser.
    /// </summary>
    public class CompilationUnitParser : SyntaxNodeVisitor
    {
        private readonly CompilationUnitSyntax compilationUnit;

        private readonly Stack<NamespaceCollection> namespacesStack = [];
        private readonly Stack<ClassCollection> classesStack = [];

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
            namespacesStack.Clear();
            classesStack.Clear();
        }

        /// <summary>
        /// Parses the compilation unit.
        /// </summary>
        public NamespaceCollection Parse()
        {
            Clear();

            namespacesStack.Push([]);
            Visit(compilationUnit.ChildNodes());
            return namespacesStack.Pop();
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
        /// Handles a ClassDeclaration.
        /// </summary>
        protected override void OnClassDeclaration(ClassDeclarationSyntax node)
        {
            string untypedName = node.Identifier.ToString();
            IEnumerable<string> parameters = [];
            TypeParameterListSyntax? typeParameterList = node.TypeParameterList;

            if (typeParameterList is not null)
            {
                parameters = typeParameterList.Parameters.Select(p => p.Identifier.ToString());
            }

            string name = parameters.Any() ? $"{untypedName}<{string.Join(',', parameters)}>" : untypedName;

            Modifiers modifiers = new(node.Modifiers.Select(m => m.ToString()));
            ClassCollection classes = classesStack.Peek();
            ClassInfo @class = classes.Add(name, untypedName, parameters, modifiers);
        }

        /// <summary>
        /// Handles a EnumDeclaration.
        /// </summary>
        protected override void OnEnumDeclaration(EnumDeclarationSyntax node)
        {

        }

        /// <summary>
        /// Handles a InterfaceDeclaration.
        /// </summary>
        protected override void OnInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {

        }

        /// <summary>
        /// Handles a NamespaceDeclaration.
        /// </summary>
        protected override void OnNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            NamespaceCollection namespaces = namespacesStack.Peek();
            string name = node.Name.ToString();
            NamespaceInfo @namespace = namespaces.Add(name);

            classesStack.Push(@namespace.Classes);
            base.OnNamespaceDeclaration(node);
            classesStack.Pop();
        }

        /// <summary>
        /// Handles a UsingDirective.
        /// </summary>
        protected override void OnUsingDirective(UsingDirectiveSyntax node) { }
    }
}
