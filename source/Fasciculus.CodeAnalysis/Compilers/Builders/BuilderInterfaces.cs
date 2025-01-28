using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal interface IClassReceiver : ITypeReceiver
    {
        public void Add(ClassSymbol @class);
    }

    internal interface IConstructorReceiver
    {
        public UriPath Link { get; }

        public void Add(ConstructorSymbol constructor);
    }

    internal interface IEnumReceiver : ITypeReceiver
    {
        public void Add(EnumSymbol @enum);
    }

    internal interface IEventReceiver
    {
        public UriPath Link { get; }

        public void Add(EventSymbol @event);
    }

    internal interface IFieldReceiver
    {
        public UriPath Link { get; }

        public void Add(FieldSymbol field);
    }

    internal interface IInterfaceReceiver : ITypeReceiver
    {
        public void Add(InterfaceSymbol @interface);
    }

    internal interface IMemberReceiver
    {
        public UriPath Link { get; }

        public void Add(MemberSymbol member);
    }

    internal interface IPropertyReceiver
    {
        public UriPath Link { get; }

        public void Add(PropertySymbol property);
    }

    internal interface ITypeReceiver
    {
        public UriPath Link { get; }
    }
}