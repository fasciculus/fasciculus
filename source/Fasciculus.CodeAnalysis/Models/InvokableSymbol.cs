using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IInvokableSymbol : ITypedSymbol
    {
        public string Id { get; }
    }

    internal class InvokableSymbol<T> : TypedSymbol<T>, IInvokableSymbol
        where T : notnull, InvokableSymbol<T>
    {
        public string Id => GetId();

        public InvokableSymbol(SymbolKind kind, TargetFramework framework, string package, SymbolComment comment)
            : base(kind, framework, package, comment) { }

        protected InvokableSymbol(T other, bool clone)
            : base(other, clone)
        {
        }

        protected virtual string GetId()
        {
            return $"id{Name.GetHashCode()}";
        }
    }
}
