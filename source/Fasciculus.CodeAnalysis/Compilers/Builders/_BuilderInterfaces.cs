using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal interface IReceiver
    {
        public UriPath Link { get; }
    }

    internal interface IAccessorReceiver : IReceiver
    {
        public void Add(AccessorSymbol accessor);
    }

    internal interface IClassReceiver : ITypeReceiver
    {
        public void Add(ClassSymbol @class);
    }

    internal interface IConstructorReceiver : IReceiver
    {
        public void Add(ConstructorSymbol constructor);
    }

    internal interface IEnumReceiver : ITypeReceiver
    {
        public void Add(EnumSymbol @enum);
    }

    internal interface IEventReceiver : IReceiver
    {
        public void Add(EventSymbol @event);
    }

    internal interface IFieldReceiver : IReceiver
    {
        public void Add(FieldSymbol field);
    }

    internal interface IInterfaceReceiver : ITypeReceiver
    {
        public void Add(InterfaceSymbol @interface);
    }

    internal interface IMemberReceiver : IReceiver
    {
        public void Add(MemberSymbol member);
    }

    internal interface IMethodReceiver : IReceiver
    {
        public void Add(MethodSymbol method);
    }

    internal interface IParameterReceiver : IReceiver
    {
        public void Add(ParameterSymbol parameter);
    }

    internal interface IPropertyReceiver : IReceiver
    {
        public void Add(PropertySymbol property);
    }

    internal interface ITypeReceiver : IReceiver
    {
    }
}