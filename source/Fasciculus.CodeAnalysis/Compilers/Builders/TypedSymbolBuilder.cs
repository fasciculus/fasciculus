using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal abstract class TypedSymbolBuilder<T> : SymbolBuilder<T>
        where T : notnull, TypedSymbol<T>
    {
        public required string Type { get; init; }
    }
}
