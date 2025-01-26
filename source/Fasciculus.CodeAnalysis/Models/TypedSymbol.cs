using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface ITypedSymbol : ISourceSymbol
    {
        public string Type { get; }
    }

    internal class TypedSymbol<T> : SourceSymbol<T>, ITypedSymbol
        where T : notnull, TypedSymbol<T>
    {
        public required string Type { get; init; }

        public TypedSymbol(SymbolKind kind, TargetFramework framework, string package, SymbolComment comment)
            : base(kind, framework, package, comment) { }

        protected TypedSymbol(T other, bool clone)
            : base(other, clone)
        {
            Type = other.Type;
        }
    }
}
