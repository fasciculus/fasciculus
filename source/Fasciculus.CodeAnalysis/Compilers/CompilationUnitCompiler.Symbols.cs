using Fasciculus.CodeAnalysis.Models;
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

            nodeDebugger.Add(node);

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

            nodeDebugger.Add(node);

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

            nodeDebugger.Add(node);

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

            nodeDebugger.Add(node);

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

            nodeDebugger.Add(node);

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

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Declaration.Variables.First().Identifier.ValueText);
                string type = GetTypeName(node.Declaration.Type);

                PushField(name, modifiers, type);
                base.VisitFieldDeclaration(node);
                Add(PopField());
            }
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            // HasTrivia: True
            // EventFieldDeclaration: VariableDeclaration

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Declaration.Variables.First().Identifier.ValueText);
                string type = GetTypeName(node.Declaration.Type);

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

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Identifier.ValueText);
                string type = GetTypeName(node.Type);
                AccessorList accessors = accessorsCompiler.Compile(node);

                PushProperty(name, modifiers, type, accessors);
                base.VisitPropertyDeclaration(node);
                Add(PopProperty());
            }
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            // ConstructorDeclaration
            // : ParameterList (BaseConstructorInitializer | ThisConstructorInitializer)? Block

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = GetName(node.Identifier, node.ParameterList, null);

                PushConstructor(name, modifiers);
                base.VisitConstructorDeclaration(node);
                Add(PopConstructor());
            }
        }

    }
}