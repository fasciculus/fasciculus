using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
        private readonly Stack<FieldBuilder> fieldBuilders = [];

        private void PushField(SymbolName name, SymbolModifiers modifiers, string type)
        {
            UriPath link = memberReceivers.Peek().Link.Append(name.Name);

            FieldBuilder builder = new(commentContext)
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Type = type,
            };

            fieldBuilders.Push(builder);
            commentReceivers.Push(builder);

            PushComment();
        }

        private void PopField()
        {
            PopComment();

            commentReceivers.Pop();

            FieldBuilder builder = fieldBuilders.Pop();
            FieldSymbol field = builder.Build();

            field.AddSource(Source);

            memberReceivers.Peek().Add(field);
        }


        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            // HasTrivia: True
            // FieldDeclaration: VariableDeclaration

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Declaration.Variables.First().Identifier.ValueText);
                string type = GetTypeName(node.Declaration.Type);

                PushField(name, modifiers, type);
                base.VisitFieldDeclaration(node);
                PopField();
            }
        }

    }
}