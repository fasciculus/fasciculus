using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IInvokableSymbol : ITypedSymbol
    {
        public IEnumerable<IParameterSymbol> Parameters { get; }
    }

    internal class InvokableSymbol<T> : TypedSymbol<T>, IInvokableSymbol
        where T : notnull, InvokableSymbol<T>
    {
        private readonly ParameterList parameters;

        public IEnumerable<IParameterSymbol> Parameters => parameters;

        public InvokableSymbol(SymbolKind kind, TargetFramework framework, string package, SymbolComment comment)
            : base(kind, framework, package, comment)
        {
            parameters = [];
        }

        protected InvokableSymbol(T other, bool clone)
            : base(other, clone)
        {
            parameters = other.parameters.Clone();
        }

        public override void MergeWith(T other)
        {
            base.MergeWith(other);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);

            parameters.ReBase(newBase);
        }

        public void Add(ParameterSymbol parameter)
            => parameters.Add(parameter);
    }
}
