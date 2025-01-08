using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.Net;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Models
{
    [DebuggerDisplay("{Name}")]
    public class Symbol
    {
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

        public Symbol(SymbolName name, UriPath link, TargetFramework framework)
        {
            Name = name;
            Link = link;

            frameworks = new([framework]);
        }

        protected Symbol(Symbol other, bool _)
        {
            Name = other.Name;
            Link = other.Link;
            Comment = other.Comment;

            frameworks = new(other.Frameworks);
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

            frameworks.Add(other.Frameworks);
        }
    }

    public class Symbol<T> : Symbol
        where T : notnull, Symbol<T>
    {
        public Symbol(SymbolName name, UriPath link, TargetFramework framework)
            : base(name, link, framework) { }

        protected Symbol(Symbol<T> other, bool clone)
            : base(other, clone) { }

        public virtual void MergeWith(T other)
        {
            base.MergeWith(other);
        }
    }
}
