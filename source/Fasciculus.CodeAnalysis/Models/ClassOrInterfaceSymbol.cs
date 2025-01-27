using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IClassOrInterfaceSymbol : ITypeSymbol
    {

    }

    internal class ClassOrInterfaceSymbol<T> : TypeSymbol<T>, IClassOrInterfaceSymbol
        where T : notnull, ClassOrInterfaceSymbol<T>
    {
        public ClassOrInterfaceSymbol(SymbolKind kind, TargetFramework framework, string package, SymbolComment comment)
            : base(kind, framework, package, comment)
        {

        }

        protected ClassOrInterfaceSymbol(T other, bool clone)
            : base(other, clone)
        {
        }

        public override void MergeWith(T other)
        {
            base.MergeWith(other);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);
        }
    }
}
