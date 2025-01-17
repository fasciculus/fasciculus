using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class NamespaceSymbol : Symbol<NamespaceSymbol>
    {
        private readonly ClassList classes;

        public IEnumerable<ClassSymbol> Classes => classes;

        public override bool IsAccessible => classes.HasAccessible;

        public NamespaceSymbol(SymbolName name, UriPath link, TargetFramework framework, string package, IEnumerable<ClassSymbol> classes)
            : base(SymbolKind.Namespace, name, link, framework, package)
        {
            this.classes = new(classes);
        }

        private NamespaceSymbol(NamespaceSymbol other, bool clone)
            : base(other, clone)
        {
            classes = other.classes.Clone();
        }

        public NamespaceSymbol Clone()
            => new(this, true);

        public override void MergeWith(NamespaceSymbol other)
        {
            base.MergeWith(other);

            classes.AddOrMergeWith(other.classes);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);

            classes.ReBase(newBase);
        }
    }
}
