using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

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

        private readonly Stack<ParameterBuilder> parameterBuilders = [];
        private readonly Stack<ConstructorBuilder> constructorBuilders = [];
        private readonly Stack<MethodBuilder> methodBuilders = [];

        private void PushComment()
            => commentBuilders.Push(new(CommentContext));

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

            if (builder is ParameterBuilder parameterBuilder) parameterBuilders.Push(parameterBuilder);
            if (builder is ConstructorBuilder constructorBuilder) constructorBuilders.Push(constructorBuilder);
            if (builder is MethodBuilder methodBuilder) methodBuilders.Push(methodBuilder);

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

        private ParameterBuilder PopParameterBuilder()
            => PopReceiver(parameterBuilders.Pop());

        private ConstructorBuilder PopConstructorBuilder()
            => PopReceiver(constructorBuilders.Pop());

        private MethodBuilder PopMethodBuilder()
            => PopReceiver(methodBuilders.Pop());

        private void PushNamespace(SymbolName name)
        {
            UriPath link = new(Package, name);

            NamespaceBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = Framework,
                Package = Package,
                Source = Repository,
                Modifiers = SymbolModifiers.Public(),
            };

            PushBuilder(builder);
        }

        private NamespaceSymbol PopNamespace()
        {
            NamespaceBuilder builder = PopNamespaceBuilder();
            SymbolComment comment = SymbolComment.Empty(CommentContext);

            return builder.Build(comment);
        }

        private void PushEnum(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = typeReceivers.Peek().Link.Append(name.Mangled);

            EnumBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = Framework,
                Package = Package,
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
                Framework = Framework,
                Package = Package,
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
                Framework = Framework,
                Package = Package,
                Modifiers = modifiers,
                Source = Source,
            };

            PushBuilder(builder);
            PushComment();
        }

        private ClassSymbol PopClass()
            => PopClassBuilder().Build(PopComment());

        private void PushField(SymbolName name, SymbolModifiers modifiers, SymbolName type)
        {
            UriPath link = fieldReceivers.Peek().Link.Append(name.Name);

            FieldBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = Framework,
                Package = Package,
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
                Framework = Framework,
                Package = Package,
                Modifiers = modifiers,
                Source = Source,
                Type = new(string.Empty),
            };

            PushBuilder(builder);
            PushComment();
        }

        private MemberSymbol PopMember()
            => PopMemberBuilder().Build(PopComment());

        private void PushEvent(SymbolName name, SymbolModifiers modifiers, SymbolName type)
        {
            UriPath link = eventReceivers.Peek().Link.Append(name.Name);

            EventBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = Framework,
                Package = Package,
                Modifiers = modifiers,
                Source = Source,
                Type = type,
            };

            PushBuilder(builder);
            PushComment();
        }

        private EventSymbol PopEvent()
            => PopEventBuilder().Build(PopComment());

        private void PushProperty(SymbolName name, SymbolModifiers modifiers, SymbolName type)
        {
            UriPath link = propertyReceivers.Peek().Link.Append(name.Name);

            PropertyBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = Framework,
                Package = Package,
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
                Framework = Framework,
                Package = Package,
                Modifiers = modifiers,
                Source = Source,
            };

            PushBuilder(builder);
            PushComment();
        }

        private AccessorSymbol PopAccessor()
            => PopAccessorBuilder().Build(PopComment());

        private void PushParameter(SymbolName name, SymbolModifiers modifiers, SymbolName type, UriPath link)
        {
            ParameterBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = Framework,
                Package = Package,
                Modifiers = modifiers,
                Source = Source,
                Type = type,
            };

            PushBuilder(builder);
            PushComment();
        }

        private ParameterSymbol PopParameter()
            => PopParameterBuilder().Build(PopComment());

        private void PushConstructor(SymbolName name, SymbolModifiers modifiers)
        {
            UriPath link = constructorReceivers.Peek().Link.Append(name.Mangled);

            ConstructorBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = Framework,
                Package = Package,
                Modifiers = modifiers,
                Source = Source,
                Type = new(string.Empty),
                BareName = name,
            };

            PushBuilder(builder);
            PushComment();
        }

        private ConstructorSymbol PopConstructor()
            => PopConstructorBuilder().Build(PopComment());

        private void PushMethod(SymbolName name, SymbolModifiers modifiers, SymbolName type)
        {
            UriPath link = methodReceivers.Peek().Link.Append(name.Mangled);

            MethodBuilder builder = new()
            {
                Name = name,
                Link = link,
                Framework = Framework,
                Package = Package,
                Modifiers = modifiers,
                Source = Source,
                Type = type,
                BareName = name,
            };

            PushBuilder(builder);
            PushComment();
        }

        private MethodSymbol PopMethod()
            => PopMethodBuilder().Build(PopComment());
    }
}