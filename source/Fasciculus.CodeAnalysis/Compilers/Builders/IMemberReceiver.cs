using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public interface IMemberReceiver
    {
        public void Add(PropertySymbol property);
    }
}
