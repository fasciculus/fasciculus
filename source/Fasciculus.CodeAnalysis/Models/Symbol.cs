using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Models
{
    public enum SymbolKind
    {
        Package,
        Namespace,
        Class
    }

    [DebuggerDisplay("{Name}")]
    public class Symbol
    {
        public SymbolKind Kind { get; }

        public SymbolName Name { get; }

        public UriPath Link { get; private set; }

        private string? comment;

        public string Comment
        {
            get => comment ?? string.Empty;
            set => comment = value;
        }

        public bool HasComment => comment is not null;

        private readonly TargetFrameworks frameworks;

        public IEnumerable<TargetFramework> Frameworks => frameworks;

        public Symbol(SymbolKind kind, SymbolName name, UriPath link, TargetFrameworks frameworks)
        {
            Kind = kind;
            Name = name;
            Link = link;

            this.frameworks = new(frameworks);
        }

        protected Symbol(Symbol other, bool _)
        {
            Kind = other.Kind;
            Name = other.Name;
            Link = other.Link;
            Comment = other.Comment;

            frameworks = new(other.frameworks);
        }

        public virtual void ReBase(UriPath newBase)
        {
            Link = Link.Replace(0, 1, newBase);
        }

        public virtual void MergeWith(Symbol other)
        {
            if (string.IsNullOrEmpty(comment))
            {
                comment = other.comment;
            }

            frameworks.Add(other.frameworks);
        }
    }

    public class Symbol<T> : Symbol
        where T : notnull, Symbol<T>
    {
        public Symbol(SymbolKind kind, SymbolName name, UriPath link, TargetFrameworks frameworks)
            : base(kind, name, link, frameworks) { }

        protected Symbol(Symbol<T> other, bool clone)
            : base(other, clone) { }

        public virtual void MergeWith(T other)
        {
            base.MergeWith(other);
        }
    }
}
