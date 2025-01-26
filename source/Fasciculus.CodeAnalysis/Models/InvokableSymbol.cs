using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class InvokableSymbol<T> : TypedSymbol<T>
        where T : notnull, InvokableSymbol<T>
    {
        public InvokableSymbol(SymbolKind kind, TargetFramework framework, string package, SymbolComment comment)
            : base(kind, framework, package, comment) { }

        protected InvokableSymbol(T other, bool clone)
            : base(other, clone)
        {
        }
    }
}
