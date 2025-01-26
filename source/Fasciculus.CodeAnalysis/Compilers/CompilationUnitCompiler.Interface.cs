using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        protected readonly Stack<InterfaceBuilder> interfaceBuilders = [];

        protected virtual void PushInterface(SymbolName name, SymbolModifiers modifiers)
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

        protected virtual void PopInterface()
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
    }
}