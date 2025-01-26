using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal interface ITypeReceiver
    {
        public UriPath Link { get; }

        public void Add(EnumSymbol @enum);
        public void Add(InterfaceSymbol @interface);
        public void Add(ClassSymbol @class);
    }
}
