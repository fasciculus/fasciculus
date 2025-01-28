using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Models;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        private readonly Stack<ITypeReceiver> typeReceivers = [];
        private readonly Stack<IEnumReceiver> enumReceivers = [];
        private readonly Stack<IInterfaceReceiver> interfaceReceivers = [];
        private readonly Stack<IClassReceiver> classReceivers = [];

        private readonly Stack<IMemberReceiver> memberReceivers = [];
        private readonly Stack<IFieldReceiver> fieldReceivers = [];
        private readonly Stack<IEventReceiver> eventReceivers = [];
        private readonly Stack<IPropertyReceiver> propertyReceivers = [];

        private readonly Stack<IConstructorReceiver> constructorReceivers = [];

        private void PushReceiver(SymbolBuilder builder)
        {
            if (builder is ITypeReceiver typeReceiver) typeReceivers.Push(typeReceiver);
            if (builder is IEnumReceiver enumReceiver) enumReceivers.Push(enumReceiver);
            if (builder is IInterfaceReceiver interfaceReceiver) interfaceReceivers.Push(interfaceReceiver);
            if (builder is IClassReceiver classReceiver) classReceivers.Push(classReceiver);

            if (builder is IMemberReceiver memberReceiver) memberReceivers.Push(memberReceiver);
            if (builder is IFieldReceiver fieldReceiver) fieldReceivers.Push(fieldReceiver);
            if (builder is IEventReceiver eventReceiver) eventReceivers.Push(eventReceiver);
            if (builder is IPropertyReceiver propertyReceiver) propertyReceivers.Push(propertyReceiver);

            if (builder is IConstructorReceiver constructorReceiver) constructorReceivers.Push(constructorReceiver);
        }

        private T PopReceiver<T>(T builder)
            where T : notnull, SymbolBuilder
        {
            if (builder is ITypeReceiver) typeReceivers.Pop();
            if (builder is IEnumReceiver) enumReceivers.Pop();
            if (builder is IInterfaceReceiver) interfaceReceivers.Pop();
            if (builder is IClassReceiver) classReceivers.Pop();

            if (builder is IMemberReceiver) memberReceivers.Pop();
            if (builder is IFieldReceiver) fieldReceivers.Pop();
            if (builder is IEventReceiver) eventReceivers.Pop();
            if (builder is IPropertyReceiver) propertyReceivers.Pop();

            if (builder is IConstructorReceiver) constructorReceivers.Pop();

            return builder;
        }

        private void Add(NamespaceSymbol @namespace)
            => compilationUnit.AddOrMergeWith(@namespace);

        private void Add(EnumSymbol @enum)
            => enumReceivers.Peek().Add(@enum);

        private void Add(InterfaceSymbol @interface)
            => interfaceReceivers.Peek().Add(@interface);

        private void Add(ClassSymbol @class)
            => classReceivers.Peek().Add(@class);

        private void Add(MemberSymbol member)
            => memberReceivers.Peek().Add(member);

        private void Add(FieldSymbol field)
            => fieldReceivers.Peek().Add(field);

        private void Add(EventSymbol @event)
            => eventReceivers.Peek().Add(@event);

        private void Add(PropertySymbol property)
            => propertyReceivers.Peek().Add(property);

        private void Add(ConstructorSymbol constructor)
            => constructorReceivers.Peek().Add(constructor);
    }
}