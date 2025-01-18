using Fasciculus.CodeAnalysis.Commenting;
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

        public SymbolComment Comment { get; set; } = SymbolComment.Empty;

        private readonly TargetFrameworks frameworks = [];

        public IEnumerable<TargetFramework> Frameworks => frameworks;

        private readonly SortedSet<string> packages;

        public IEnumerable<string> Packages => packages;

        public Symbol(SymbolKind kind, TargetFramework framework, string package)
        {
            Kind = kind;

            frameworks.Add(framework);
            packages = [package];
        }

        protected Symbol(Symbol other, bool _)
        {
            Kind = other.Kind;

            link = other.link;
            modifiers = other.modifiers;
            Comment = other.Comment;

            frameworks.Add(other.frameworks);
            packages = new(other.packages);
        }

        public virtual void ReBase(UriPath newBase)
        {
            link = link.Replace(0, 1, newBase);
        }

        public virtual void MergeWith(Symbol other)
        {
            frameworks.Add(other.frameworks);
            packages.UnionWith(other.packages);
        }
    }

    public class Symbol<T> : Symbol
        where T : notnull, Symbol<T>
    {
        public Symbol(SymbolKind kind, TargetFramework framework, string package)
            : base(kind, framework, package) { }

        protected Symbol(Symbol<T> other, bool clone)
            : base(other, clone) { }

        public virtual void MergeWith(T other)
        {
            base.MergeWith(other);
        }
    }
}
