using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
        private readonly Stack<PropertyBuilder> propertyBuilders = [];

        private void PushProperty(SymbolName name, SymbolModifiers modifiers, string type, AccessorList accessors)
        {
            UriPath link = memberReceivers.Peek().Link.Append(name.Name);

            PropertyBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Type = type,
                Accessors = accessors
            };

            propertyBuilders.Push(builder);
            commentReceivers.Push(builder);

            PushComment();
        }

        private void PopProperty()
        {
            PopComment();

            commentReceivers.Pop();

            PropertyBuilder builder = propertyBuilders.Pop();
            PropertySymbol property = builder.Build();

            property.AddSource(Source);

            memberReceivers.Peek().Add(property);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            // HasTrivia: True
            // PropertyDeclaration
            // : <return-type> ExplicitInterfaceSpecifier? ((AccessorList EqualsValueClause?) | ArrowExpressionClause)
            //
            // <return-type>
            // : AttributeList? (IdentifierName | GenericName | PredefinedType | NullableType) 

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Identifier.ValueText);
                string type = GetTypeName(node.Type);
                AccessorList accessors = accessorsCompiler.Compile(node);

                PushProperty(name, modifiers, type, accessors);
                base.VisitPropertyDeclaration(node);
                PopProperty();
            }
        }

        public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            // covers GetAccessorDeclaration, SetAccessorDeclaration, InitAccessorDeclaration, AddAccessorDeclaration,
            //  RemoveAccessorDeclaration, UnknownAccessorDeclaration
            //
            // GetAccessorDeclaration: ArrowExpressionClause?
            // SetAccessorDeclaration:

            nodeDebugger.Add(node);

            base.VisitAccessorDeclaration(node);
        }

        public override void VisitAccessorList(AccessorListSyntax node)
        {
            // AccessorList: GetAccessorDeclaration? SetAccessorDeclaration?

            nodeDebugger.Add(node);

            base.VisitAccessorList(node);
        }
    }
}

