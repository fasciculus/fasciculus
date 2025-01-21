using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class MemberSymbol<T> : SourceSymbol<T>
        where T : notnull, MemberSymbol<T>
    {
        public required string Type { get; init; }

        public MemberSymbol(SymbolKind kind, TargetFramework framework, string package)
            : base(kind, framework, package) { }

        protected MemberSymbol(T other, bool clone)
            : base(other, clone)
        {
            Type = other.Type;
        }
    }
}
