using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public interface ITypeReceiver
    {
        public void Add(ClassSymbol @class);
    }
}
