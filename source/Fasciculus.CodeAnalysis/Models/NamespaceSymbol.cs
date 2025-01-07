using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.Net;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class NamespaceSymbol : Symbol<NamespaceSymbol>
    {
        private readonly ClassList classes;

        public IEnumerable<ClassSymbol> Classes => classes;

        public NamespaceSymbol(SymbolName name, UriPath link, TargetFramework framework, IEnumerable<ClassSymbol> classes)
            : base(name, link, framework)
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
