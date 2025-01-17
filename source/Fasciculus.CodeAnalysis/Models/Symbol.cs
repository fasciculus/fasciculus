using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Collections;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Models
{
    [DebuggerDisplay("{Name}")]
    public class Symbol
    {
        public SymbolKind Kind { get; }

        public SymbolName Name { get; }

        public UriPath Link { get; private set; }

        public SymbolModifiers Modifiers { get; set; } = new();

        public virtual bool IsAccessible => Modifiers.IsAccessible;

        public SymbolComment Comment { get; set; } = SymbolComment.Empty;

        private readonly TargetFrameworks frameworks = new();

        public IEnumerable<TargetFramework> Frameworks => frameworks;

        private readonly SortedSet<string> packages;

        public IEnumerable<string> Packages => packages;

        public Symbol(SymbolKind kind, SymbolName name, UriPath link, TargetFramework framework, string package)
        {
            Kind = kind;
            Name = name;
            Link = link;

            frameworks.Add(framework);
            packages = [package];
        }

        protected Symbol(Symbol other, bool _)
        {
            Kind = other.Kind;
            Name = other.Name;
            Link = other.Link;
            Modifiers = other.Modifiers;
            Comment = other.Comment;

            frameworks.Add(other.frameworks);
            packages = new(other.packages);
        }

        public virtual void ReBase(UriPath newBase)
        {
            Link = Link.Replace(0, 1, newBase);
        }

        public virtual void MergeWith(Symbol other)
        {
            frameworks.Add(other.frameworks);
            other.packages.Apply(p => { packages.Add(p); });
        }
    }

    public class Symbol<T> : Symbol
        where T : notnull, Symbol<T>
    {
        public Symbol(SymbolKind kind, SymbolName name, UriPath link, TargetFramework framework, string package)
            : base(kind, name, link, framework, package) { }

        protected Symbol(Symbol<T> other, bool clone)
            : base(other, clone) { }

        public virtual void MergeWith(T other)
        {
            base.MergeWith(other);
        }
    }
}
