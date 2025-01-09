﻿using Fasciculus.CodeAnalysis.Commenting;
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

        public SymbolModifiers Modifiers { get; set; }

        public SymbolComment Comment { get; set; } = SymbolComment.Empty;

        private readonly TargetFrameworks frameworks;

        public IEnumerable<TargetFramework> Frameworks => frameworks;

        public TargetProducts Products => new(frameworks);

        public virtual bool IsAccessible => Modifiers.IsAccessible;

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
            Modifiers = other.Modifiers;
            Comment = other.Comment;

            frameworks = new(other.frameworks);
        }

        public virtual void ReBase(UriPath newBase)
        {
            Link = Link.Replace(0, 1, newBase);
        }

        public virtual void MergeWith(Symbol other)
        {
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
