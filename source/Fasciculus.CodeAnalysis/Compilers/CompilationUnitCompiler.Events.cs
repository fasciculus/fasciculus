using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
        private readonly Stack<EventBuilder> eventBuilders = [];

        private void PushEvent(SymbolName name, SymbolModifiers modifiers, string type)
        {
            UriPath link = memberReceivers.Peek().Link.Append(name.Name);

            EventBuilder builder = new(commentContext)
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Type = type,
            };

            eventBuilders.Push(builder);
            commentReceivers.Push(builder);

            PushComment();
        }

        private void PopEvent()
        {
            PopComment();

            commentReceivers.Pop();

            EventBuilder builder = eventBuilders.Pop();
            EventSymbol @event = builder.Build();

            @event.AddSource(Source);

            memberReceivers.Peek().Add(@event);
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            // HasTrivia: True
            // EventFieldDeclaration: VariableDeclaration

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Declaration.Variables.First().Identifier.ValueText);
                string type = GetTypeName(node.Declaration.Type);

                PushEvent(name, modifiers, type);
                base.VisitEventFieldDeclaration(node);
                PopEvent();
            }
        }
    }
}