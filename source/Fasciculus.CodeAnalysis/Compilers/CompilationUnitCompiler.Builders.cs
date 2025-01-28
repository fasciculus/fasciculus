using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        private readonly Stack<CommentBuilder> commentBuilders = [];

        private readonly Stack<NamespaceBuilder> namespaceBuilders = [];

        private readonly Stack<EnumBuilder> enumBuilders = [];
        private readonly Stack<InterfaceBuilder> interfaceBuilders = [];
        private readonly Stack<ClassBuilder> classBuilders = [];

        private readonly Stack<MemberBuilder> memberBuilders = [];
        private readonly Stack<FieldBuilder> fieldBuilders = [];
        private readonly Stack<EventBuilder> eventBuilders = [];
        private readonly Stack<PropertyBuilder> propertyBuilders = [];
        private readonly Stack<AccessorBuilder> accessorBuilders = [];

        private readonly Stack<ConstructorBuilder> constructorBuilders = [];

        private void PushComment()
            => commentBuilders.Push(new(commentContext));

        private SymbolComment PopComment()
            => commentBuilders.Pop().Build();

        private void PushBuilder(SymbolBuilder builder)
        {
            if (builder is NamespaceBuilder namespaceBuilder) namespaceBuilders.Push(namespaceBuilder);

            if (builder is EnumBuilder enumBuilder) enumBuilders.Push(enumBuilder);
            if (builder is InterfaceBuilder interfaceBuilder) interfaceBuilders.Push(interfaceBuilder);
            if (builder is ClassBuilder classBuilder) classBuilders.Push(classBuilder);

            if (builder is MemberBuilder memberBuilder) memberBuilders.Push(memberBuilder);
            if (builder is FieldBuilder fieldBuilder) fieldBuilders.Push(fieldBuilder);
            if (builder is EventBuilder eventBuilder) eventBuilders.Push(eventBuilder);
            if (builder is PropertyBuilder propertyBuilder) propertyBuilders.Push(propertyBuilder);
            if (builder is AccessorBuilder accessorBuilder) accessorBuilders.Push(accessorBuilder);

            if (builder is ConstructorBuilder constructorBuilder) constructorBuilders.Push(constructorBuilder);

            PushReceiver(builder);
        }

        private NamespaceBuilder PopNamespaceBuilder()
            => PopReceiver(namespaceBuilders.Pop());

        private EnumBuilder PopEnumBuilder()
            => PopReceiver(enumBuilders.Pop());

        private InterfaceBuilder PopInterfaceBuilder()
            => PopReceiver(interfaceBuilders.Pop());

        private ClassBuilder PopClassBuilder()
            => PopReceiver(classBuilders.Pop());

        private MemberBuilder PopMemberBuilder()
            => PopReceiver(memberBuilders.Pop());

        private FieldBuilder PopFieldBuilder()
            => PopReceiver(fieldBuilders.Pop());

        private EventBuilder PopEventBuilder()
            => PopReceiver(eventBuilders.Pop());

        private PropertyBuilder PopPropertyBuilder()
            => PopReceiver(propertyBuilders.Pop());

        private AccessorBuilder PopAccessorBuilder()
            => PopReceiver(accessorBuilders.Pop());

        private ConstructorBuilder PopConstructorBuilder()
            => PopReceiver(constructorBuilders.Pop());

        private void PushNamespace(SymbolName name)
        {
            UriPath link = new(package, name);

            NamespaceBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Source = UriPath.Empty,
                Modifiers = SymbolModifiers.Public(),
            };

            PushBuilder(builder);
        }

        private NamespaceSymbol PopNamespace()
        {
            NamespaceBuilder builder = PopNamespaceBuilder();
            FileInfo commentFile = namespaceCommentsDirectory.File($"{builder.Name}.xml");
            SymbolComment comment = SymbolComment.FromFile(commentContext, commentFile);

            return builder.Build(comment);
        }

        private void PushEnum(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = typeReceivers.Peek().Link.Append(name.Mangled);

            EnumBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Source = Source,
            };

            PushBuilder(builder);
            PushComment();
        }

        private EnumSymbol PopEnum()
            => PopEnumBuilder().Build(PopComment());

        private void PushInterface(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = typeReceivers.Peek().Link.Append(name.Mangled);

            InterfaceBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Source = Source,
            };

            PushBuilder(builder);
            PushComment();
        }

        private InterfaceSymbol PopInterface()
            => PopInterfaceBuilder().Build(PopComment());

        private void PushClass(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = typeReceivers.Peek().Link.Append(name.Mangled);

            ClassBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Source = Source,
            };

            PushBuilder(builder);
            PushComment();
        }

        private ClassSymbol PopClass()
            => PopClassBuilder().Build(PopComment());

        private void PushField(SymbolName name, SymbolModifiers modifiers, string type)
        {
            UriPath link = fieldReceivers.Peek().Link.Append(name.Name);

            FieldBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Source = Source,
                Type = type,
            };

            PushBuilder(builder);
            PushComment();
        }

        private FieldSymbol PopField()
            => PopFieldBuilder().Build(PopComment());

        private void PushMember(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = memberReceivers.Peek().Link.Append(name.Name);

            MemberBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Source = Source,
                Type = string.Empty,
            };

            PushBuilder(builder);
            PushComment();
        }

        private MemberSymbol PopMember()
            => PopMemberBuilder().Build(PopComment());

        private void PushEvent(SymbolName name, SymbolModifiers modifiers, string type)
        {
            UriPath link = eventReceivers.Peek().Link.Append(name.Name);

            EventBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Source = Source,
                Type = type,
            };

            PushBuilder(builder);
            PushComment();
        }

        private EventSymbol PopEvent()
            => PopEventBuilder().Build(PopComment());

        private void PushProperty(SymbolName name, SymbolModifiers modifiers, string type)
        {
            UriPath link = propertyReceivers.Peek().Link.Append(name.Name);

            PropertyBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Type = type,
                Source = Source,
            };

            PushBuilder(builder);
            PushComment();
        }

        private PropertySymbol PopProperty()
            => PopPropertyBuilder().Build(PopComment());

        private void PushAccessor(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = accessorReceivers.Peek().Link.Append(name.Name);

            AccessorBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Source = Source,
            };

            PushBuilder(builder);
            PushComment();
        }

        private AccessorSymbol PopAccessor()
            => PopAccessorBuilder().Build(PopComment());

        private void PushConstructor(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = constructorReceivers.Peek().Link.Append(name.Mangled);

            ConstructorBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = framework,
                Package = package,
                Modifiers = modifiers,
                Source = Source,
                Type = string.Empty,
            };

            PushBuilder(builder);
            PushComment();
        }

        private ConstructorSymbol PopConstructor()
            => PopConstructorBuilder().Build(PopComment());
    }
}