using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
        protected readonly Stack<ClassBuilder> classBuilders = [];

        protected virtual void PushClass(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = typeReceivers.Peek().Link.Append(name.Mangled);
            ClassBuilder builder = new(name, link, Framework, Package, modifiers);

            classBuilders.Push(builder);
            typeReceivers.Push(builder);
            commentReceivers.Push(builder);

            PushComment();
        }

        protected virtual void PopClass()
        {
            PopComment();
            commentReceivers.Pop();
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

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = ModifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = GetName(node.Identifier, node.TypeParameterList);

                PushClass(name, modifiers);

                base.VisitClassDeclaration(node);

                PopClass();
            }
        }

    }
}