using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            // HasTrivia: True
            // NamespaceDeclaration
            // : QualifiedName ClassDeclaration* InterfaceDeclaration* EnumDeclaration*

            NodeDebugger.Add(node);

            SymbolName name = new(node.Name.ToString());

            PushNamespace(name);
            base.VisitNamespaceDeclaration(node);
            Add(PopNamespace());
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            // HasTrivia: True
            // EnumDeclaration
            // : EnumMemberDeclaration*

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Identifier.ValueText);

                PushEnum(name, modifiers);
                base.VisitEnumDeclaration(node);
                Add(PopEnum());
            }
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            // HasTrivia: True
            // InterfaceDeclaration
            // : TypeParameterList? BaseList? TypeParameterConstraintClause? MethodDeclaration*

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = GetName(node.Identifier, node.TypeParameterList);

                PushInterface(name, modifiers);
                base.VisitInterfaceDeclaration(node);
                Add(PopInterface());
            }
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // HasTrivia: True
            // ClassDeclaration
            // : AttributeList? TypeParameterList? BaseList? TypeParameterConstraintClause?
            //   (
            //       FieldDeclaration
            //     | PropertyDeclaration
            //     | IndexerDeclaration
            //     | EventFieldDeclaration
            //     | ConstructorDeclaration
            //     | DestructorDeclaration
            //     | MethodDeclaration
            //     | OperatorDeclaration
            //     | ConversionOperatorDeclaration
            //     | ClassDeclaration
            //   )*
            //
            // TypeParameterConstraintClause only when TypeParameterList 

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = GetName(node.Identifier, node.TypeParameterList);

                PushClass(name, modifiers);
                base.VisitClassDeclaration(node);
                Add(PopClass());
            }
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            // HasTrivia: True
            // Leaf

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Identifier.ValueText);

                PushMember(name, modifiers);
                base.VisitEnumMemberDeclaration(node);
                Add(PopMember());
            }
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            // HasTrivia: True
            // FieldDeclaration: VariableDeclaration

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Declaration.Variables.First().Identifier.ValueText);
                SymbolName type = GetTypeName(node.Declaration.Type);

                PushField(name, modifiers, type);
                base.VisitFieldDeclaration(node);
                Add(PopField());
            }
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            // HasTrivia: True
            // EventFieldDeclaration: VariableDeclaration

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Declaration.Variables.First().Identifier.ValueText);
                SymbolName type = GetTypeName(node.Declaration.Type);

                PushEvent(name, modifiers, type);
                base.VisitEventFieldDeclaration(node);
                Add(PopEvent());
            }
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            // HasTrivia: True
            // PropertyDeclaration
            // : <return-type> ExplicitInterfaceSpecifier? ((AccessorList EqualsValueClause?) | ArrowExpressionClause)
            //
            // <return-type>
            // : AttributeList? (IdentifierName | GenericName | PredefinedType | NullableType) 

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Identifier.ValueText);
                SymbolName type = GetTypeName(node.Type);

                PushProperty(name, modifiers, type);
                base.VisitPropertyDeclaration(node);
                Add(PopProperty());
            }
        }

        public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            // covers GetAccessorDeclaration, SetAccessorDeclaration, InitAccessorDeclaration, AddAccessorDeclaration,
            //  RemoveAccessorDeclaration, UnknownAccessorDeclaration
            //
            // GetAccessorDeclaration: ArrowExpressionClause?
            // SetAccessorDeclaration:

            NodeDebugger.Add(node);
            AccessorDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Keyword.ValueText);

                PushAccessor(name, modifiers);
                base.VisitAccessorDeclaration(node);
                Add(PopAccessor());
            }
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            // Parameter
            // : IdentifierName EqualsValueClause?
            // | GenericName
            // | PredefinedType EqualsValueClause?
            // | ArrayType
            // | PointerType
            // | NullableType EqualsValueClause?

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = GetName(node.Identifier, null);
                SymbolName type = GetTypeName(node.Type);
                UriPath link = GetParameterLink(node);

                PushParameter(name, modifiers, type, link);
                base.VisitParameter(node);
                Add(PopParameter());
            }
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            // ConstructorDeclaration
            // : ParameterList (BaseConstructorInitializer | ThisConstructorInitializer)? Block

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = GetName(node.Identifier, null);

                PushConstructor(name, modifiers);
                base.VisitConstructorDeclaration(node);
                Add(PopConstructor());
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            // HasTrivia: True
            // MethodDeclaration
            // : AttributeList? <return-type> ExplicitInterfaceSpecifier? ParameterList TypeParameterConstraintClause? (ArrowExpressionClause | Block)
            //
            // return-type
            // : IdentifierName TypeParameterList?
            // | GenericName TypeParameterList?
            // | PredefinedType TypeParameterList?
            // | ArrayType
            // | NullableType

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = GetName(node.Identifier, null);
                SymbolName type = GetTypeName(node.ReturnType);

                PushMethod(name, modifiers, type);
                base.VisitMethodDeclaration(node);
                Add(PopMethod());
            }
        }

    }
}