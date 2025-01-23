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
        protected readonly Stack<EnumMemberBuilder> enumMemberBuilders = [];

        protected virtual void PushEnum(SymbolName name, SymbolModifiers modifiers)
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

        protected virtual void PushEnumMember(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = memberReceivers.Peek().Link.Append(name.Name);

            EnumMemberBuilder builder = new(commentContext)
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Type = string.Empty,
            };

            enumMemberBuilders.Push(builder);
            commentReceivers.Push(builder);

            PushComment();
        }

        protected virtual void PopEnumMember()
        {
            PopComment();

            commentReceivers.Pop();

            EnumMemberBuilder builder = enumMemberBuilders.Pop();
            MemberSymbol member = builder.Build();

            member.AddSource(Source);

            memberReceivers.Peek().Add(member);
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
                SymbolName name = new(node.Identifier.ValueText);

                PushEnumMember(name, modifiers);

                base.VisitEnumMemberDeclaration(node);

                PopEnumMember();
            }
        }
    }
}