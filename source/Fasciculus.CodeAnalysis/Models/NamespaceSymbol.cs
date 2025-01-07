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

        public override void MergeWith(NamespaceSymbol other)
        {
            base.MergeWith(other);

            classes.AddOrMergeWith(other.classes);
        }
    }
}
