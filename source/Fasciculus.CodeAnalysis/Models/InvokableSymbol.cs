using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IInvokableSymbol : ITypedSymbol
    {
    }

    internal class InvokableSymbol<T> : TypedSymbol<T>, IInvokableSymbol
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
