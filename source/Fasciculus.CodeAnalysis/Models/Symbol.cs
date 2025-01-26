using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Models
{
    [DebuggerDisplay("{Name}")]
    public class Symbol
    {
        public SymbolKind Kind { get; }

        public required SymbolName Name { get; init; }

        private UriPath link = new();

        public required UriPath Link
        {
            get => link;
            init => link = value;
        }

        private SymbolModifiers modifiers = new();

        public required SymbolModifiers Modifiers
        {
            get => new(modifiers);
            init => modifiers = value;
        }

        public virtual bool IsAccessible => modifiers.IsAccessible;

        private readonly TargetFrameworks frameworks = [];

        public TargetFrameworks Frameworks => new(frameworks);

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
            modifiers = other.modifiers;

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

    public class Symbol<T> : Symbol
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
