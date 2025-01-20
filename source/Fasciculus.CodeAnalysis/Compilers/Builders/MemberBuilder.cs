using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public abstract class MemberBuilder<T> : SymbolBuilder<T>
        where T : notnull, Symbol<T>
    {
        public required string Type { get; init; }
    }
}
