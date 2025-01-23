using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class TypedSymbol<T> : SourceSymbol<T>
        where T : notnull, TypedSymbol<T>
    {
        public required string Type { get; init; }

        public TypedSymbol(SymbolKind kind, TargetFramework framework, string package)
            : base(kind, framework, package) { }

        protected TypedSymbol(T other, bool clone)
            : base(other, clone)
        {
            Type = other.Type;
        }
    }
}
