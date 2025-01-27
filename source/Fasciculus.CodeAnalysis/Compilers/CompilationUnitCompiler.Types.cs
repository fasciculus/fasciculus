using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        private readonly Stack<ITypeReceiver> typeReceivers = [];

        protected readonly Stack<EnumBuilder> enumBuilders = [];

        protected readonly Stack<InterfaceBuilder> interfaceBuilders = [];

        private readonly Stack<ClassBuilder> classBuilders = [];

        private void PushEnum(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = typeReceivers.Peek().Link.Append(name.Mangled);

            EnumBuilder builder = new(commentContext)
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
            };

            enumBuilders.Push(builder);
            typeReceivers.Push(builder);
            memberReceivers.Push(builder);
            commentReceivers.Push(builder);

            PushComment();
        }

        private void PopEnum()
        {
            PopComment();
            commentReceivers.Pop();
            memberReceivers.Pop();
            typeReceivers.Pop();

            EnumBuilder builder = enumBuilders.Pop();
            EnumSymbol @enum = builder.Build();

            @enum.AddSource(Source);

            typeReceivers.Peek().Add(@enum);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            // HasTrivia: True
            // EnumDeclaration
            // : EnumMemberDeclaration*

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Identifier.ValueText);

                PushEnum(name, modifiers);

                base.VisitEnumDeclaration(node);

                PopEnum();
            }
        }

        private void PushInterface(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = typeReceivers.Peek().Link.Append(name.Mangled);

            InterfaceBuilder builder = new(commentContext)
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
            };

            interfaceBuilders.Push(builder);
            typeReceivers.Push(builder);
            memberReceivers.Push(builder);
            commentReceivers.Push(builder);

            PushComment();
        }

        private void PopInterface()
        {
            PopComment();
            commentReceivers.Pop();
            memberReceivers.Pop();
            typeReceivers.Pop();

            InterfaceBuilder builder = interfaceBuilders.Pop();
            InterfaceSymbol @interface = builder.Build();

            @interface.AddSource(Source);

            typeReceivers.Peek().Add(@interface);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            // HasTrivia: True
            // InterfaceDeclaration
            // : TypeParameterList? BaseList? TypeParameterConstraintClause? MethodDeclaration*

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = GetName(node.Identifier, node.TypeParameterList);

                PushInterface(name, modifiers);

                base.VisitInterfaceDeclaration(node);

                PopInterface();
            }
        }

        private void PushClass(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = typeReceivers.Peek().Link.Append(name.Mangled);

            ClassBuilder builder = new(commentContext)
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
            };

            classBuilders.Push(builder);
            typeReceivers.Push(builder);
            memberReceivers.Push(builder);
            commentReceivers.Push(builder);

            PushComment();
        }

        private void PopClass()
        {
            PopComment();

            commentReceivers.Pop();
            memberReceivers.Pop();
            typeReceivers.Pop();

            ClassBuilder builder = classBuilders.Pop();
            ClassSymbol @class = builder.Build();

            @class.AddSource(Source);

            typeReceivers.Peek().Add(@class);
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

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = GetName(node.Identifier, node.TypeParameterList);

                PushClass(name, modifiers);

                base.VisitClassDeclaration(node);

                PopClass();
            }
        }

        public override void VisitBaseList(BaseListSyntax node)
        {
            // BaseList
            // : SimpleBaseType+

            nodeDebugger.Add(node);

            base.VisitBaseList(node);
        }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            // SimpleBaseType
            // : IdentifierName
            // | GenericName

            nodeDebugger.Add(node);

            base.VisitSimpleBaseType(node);
        }

        private string GetTypeName(TypeSyntax type)
        {
            if (type is PredefinedTypeSyntax predefined) return GetPredefinedTypeName(predefined);
            if (type is IdentifierNameSyntax identifier) return GetIdentifierTypeName(identifier);
            if (type is GenericNameSyntax generic) return GetGenericTypeName(generic);
            if (type is NullableTypeSyntax nullable) return GetNullableTypeName(nullable);

            return string.Empty;
        }

        private static string GetPredefinedTypeName(PredefinedTypeSyntax type)
        {
            return type.Keyword.ToString();
        }

        private static string GetIdentifierTypeName(IdentifierNameSyntax identifier)
        {
            return identifier.Identifier.ValueText;
        }

        private string GetGenericTypeName(GenericNameSyntax type)
        {
            string name = type.Identifier.ValueText;
            TypeSyntax[] args = [.. type.TypeArgumentList.Arguments];
            string[] names = [.. args.Select(GetTypeName)];

            return $"{name}<{string.Join(",", names)}>";
        }

        private string GetNullableTypeName(NullableTypeSyntax nullable)
        {
            return GetTypeName(nullable.ElementType) + "?";
        }
    }
}