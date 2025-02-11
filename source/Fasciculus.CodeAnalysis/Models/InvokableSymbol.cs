using Fasciculus.CodeAnalysis.Frameworking;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IInvokableSymbol : ITypedSymbol
    {
        public string BareName { get; }

        public IEnumerable<IParameterSymbol> Parameters { get; }
    }

    internal class InvokableSymbol<T> : TypedSymbol<T>, IInvokableSymbol
        where T : notnull, InvokableSymbol<T>
    {
        public required string BareName { get; init; }

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
            BareName = other.BareName;

            parameters = other.parameters.Clone();
        }

        public override void MergeWith(T other)
        {
            base.MergeWith(other);
        }

        public void Add(ParameterSymbol parameter)
            => parameters.Add(parameter);
    }
}
