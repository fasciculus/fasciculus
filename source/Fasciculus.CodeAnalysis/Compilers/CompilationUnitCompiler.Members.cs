using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        private readonly Stack<IMemberReceiver> memberReceivers = [];

        protected readonly Stack<MemberBuilder> memberBuilders = [];
        private readonly Stack<FieldBuilder> fieldBuilders = [];
        private readonly Stack<EventBuilder> eventBuilders = [];
        private readonly Stack<PropertyBuilder> propertyBuilders = [];

        private void PushEnumMember(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = memberReceivers.Peek().Link.Append(name.Name);

            MemberBuilder builder = new(commentContext)
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Type = string.Empty,
            };

            memberBuilders.Push(builder);
            commentReceivers.Push(builder);

            PushComment();
        }

        private void PopEnumMember()
        {
            PopComment();

            commentReceivers.Pop();

            MemberBuilder builder = memberBuilders.Pop();
            MemberSymbol member = builder.Build();

            member.AddSource(Source);

            memberReceivers.Peek().Add(member);
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

        private void PushProperty(SymbolName name, SymbolModifiers modifiers, string type, AccessorList accessors)
        {
            UriPath link = memberReceivers.Peek().Link.Append(name.Name);

            PropertyBuilder builder = new(commentContext)
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Type = type,
                Accessors = accessors
            };

            propertyBuilders.Push(builder);
            commentReceivers.Push(builder);

            PushComment();
        }

        private void PopProperty()
        {
            PopComment();

            commentReceivers.Pop();

            PropertyBuilder builder = propertyBuilders.Pop();
            PropertySymbol property = builder.Build();

            property.AddSource(Source);

            memberReceivers.Peek().Add(property);
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

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                SymbolName name = new(node.Identifier.ValueText);
                string type = GetTypeName(node.Type);
                AccessorList accessors = accessorsCompiler.Compile(node);

                PushProperty(name, modifiers, type, accessors);
                base.VisitPropertyDeclaration(node);
                PopProperty();
            }
        }
    }
}