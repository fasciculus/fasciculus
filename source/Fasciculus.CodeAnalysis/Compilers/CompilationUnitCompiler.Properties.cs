using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
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
                //AccessorListSyntax? accessorList = node.AccessorList;

                //if (accessorList is not null)
                //{
                //    AccessorDeclarationSyntax[] accessors = [.. accessorList.Accessors];

                //    foreach (AccessorDeclarationSyntax accessor in accessors)
                //    {
                //        SyntaxTokenList accessorModifierList = accessor.Modifiers;

                //        if (accessorModifierList.Count > 0)
                //        {
                //            SymbolModifiers accessorModifiers = ModifiersCompiler.Compile(accessorModifierList);
                //        }
                //    }
                //}

                PushComment();

                base.VisitPropertyDeclaration(node);

                PopComment();
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

