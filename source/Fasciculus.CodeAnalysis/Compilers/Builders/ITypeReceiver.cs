using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public interface ITypeReceiver
    {
        public UriPath Link { get; }

        public void Add(ClassSymbol @class);
    }
}
