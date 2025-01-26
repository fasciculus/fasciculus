using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        private readonly Stack<ConstructorBuilder> constructorBuilders = [];

        private void PushConstructor(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = typeReceivers.Peek().Link.Append(name.Mangled);

            ConstructorBuilder builder = new(commentContext)
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Type = string.Empty,
            };

            constructorBuilders.Push(builder);
            commentReceivers.Push(builder);

            PushComment();
        }

        private void PopConstructor()
        {
            PopComment();
            commentReceivers.Pop();

            ConstructorBuilder builder = constructorBuilders.Pop();
            ConstructorSymbol constructor = builder.Build();

            constructor.AddSource(Source);

            classBuilders.Peek().Add(constructor);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            // ConstructorDeclaration
            // : ParameterList (BaseConstructorInitializer | ThisConstructorInitializer)? Block

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = GetName(node.Identifier, node.ParameterList, null);

                PushConstructor(name, modifiers);
                base.VisitConstructorDeclaration(node);
                PopConstructor();
            }
        }
    }
}