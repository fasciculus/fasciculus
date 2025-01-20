using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
        protected readonly Stack<EnumBuilder> enumBuilders = [];

        protected virtual void PushEnum(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = typeReceivers.Peek().Link.Append(name.Mangled);
            EnumBuilder builder = new(name, link, Framework, Package, modifiers);

            enumBuilders.Push(builder);
            typeReceivers.Push(builder);
            commentReceivers.Push(builder);
            memberReceivers.Push(builder);

            PushComment();
        }

        protected virtual void PopEnum()
        {
            PopComment();
            commentReceivers.Pop();
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

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            // HasTrivia: True
            // Leaf

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                PushComment();

                base.VisitEnumMemberDeclaration(node);

                PopComment();
            }
        }
    }
}