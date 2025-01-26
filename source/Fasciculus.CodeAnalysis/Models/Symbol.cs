using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface ISymbol
    {
        public SymbolKind Kind { get; }

        public SymbolName Name { get; }

        public UriPath Link { get; }

        public ISymbolModifiers Modifiers { get; }

        public bool IsAccessible { get; }

        public ITargetFrameworks Frameworks { get; }

        public IEnumerable<string> Packages { get; }

        public ISymbolComment Comment { get; }
    }

    [DebuggerDisplay("{Name}")]
    internal class Symbol : ISymbol
    {
        public SymbolKind Kind { get; }

        public required SymbolName Name { get; init; }

        private UriPath link = new();

        public required UriPath Link
        {
            get => link;
            init => link = value;
        }

        public required ISymbolModifiers Modifiers { get; init; }

        public virtual bool IsAccessible => Modifiers.IsAccessible;

        private readonly TargetFrameworks frameworks = [];

        public ITargetFrameworks Frameworks => frameworks;

        private readonly SortedSet<string> packages;

        public IEnumerable<string> Packages => packages;

        private readonly SymbolComment comment;

        public ISymbolComment Comment => comment;

        public Symbol(SymbolKind kind, TargetFramework framework, string package, SymbolComment comment)
        {
            Kind = kind;

            frameworks.Add(framework);
            packages = [package];

            this.comment = comment;
        }

        protected Symbol(Symbol other, bool _)
        {
            Kind = other.Kind;

            link = other.link;

            Modifiers = other.Modifiers;

            frameworks.Add(other.frameworks);
            packages = new(other.packages);

            comment = other.comment.Clone();
        }

        public virtual void ReBase(UriPath newBase)
        {
            link = link.Replace(0, 1, newBase);
        }

        public virtual void MergeWith(Symbol other)
        {
            frameworks.Add(other.frameworks);
            packages.UnionWith(other.packages);
            comment.MergeWith(other.comment);
        }
    }

    internal class Symbol<T> : Symbol
        where T : notnull, Symbol<T>
    {
        public Symbol(SymbolKind kind, TargetFramework framework, string package, SymbolComment comment)
            : base(kind, framework, package, comment) { }

        protected Symbol(Symbol<T> other, bool clone)
            : base(other, clone) { }

        public virtual void MergeWith(T other)
        {
            base.MergeWith(other);
        }
    }
}
