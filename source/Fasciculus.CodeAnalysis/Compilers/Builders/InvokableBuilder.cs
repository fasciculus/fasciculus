using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal abstract class InvokableBuilder<T> : TypedSymbolBuilder<T>
        where T : notnull, InvokableSymbol<T>
    {
    }
}
