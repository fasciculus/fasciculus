using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public interface IMemberReceiver
    {
        public UriPath Link { get; }

        public void Add(FieldSymbol field);

        public void Add(PropertySymbol property);
    }
}
