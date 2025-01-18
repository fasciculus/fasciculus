using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class NamespaceSymbol : Symbol<NamespaceSymbol>
    {
        private readonly ClassList classes = [];
        private readonly EnumList enums = [];

        public IEnumerable<ClassSymbol> Classes => classes;
        public IEnumerable<EnumSymbol> Enums => enums;

        public override bool IsAccessible => classes.HasAccessible;

        public virtual bool IsEmpty
            => classes.Count == 0 && enums.Count == 0;

        public NamespaceSymbol(UriPath link, TargetFramework framework, string package)
            : base(SymbolKind.Namespace, link, framework, package)
        {
        }

        private NamespaceSymbol(NamespaceSymbol other, bool clone)
            : base(other, clone)
        {
            classes = other.classes.Clone();
            enums = other.enums.Clone();
        }

        public NamespaceSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name
            };
        }

        public void AddOrMergeWith(ClassSymbol @class)
        {
            classes.AddOrMergeWith(@class);
        }

        public void AddOrMergeWith(EnumSymbol @enum)
        {
            enums.AddOrMergeWith(@enum);
        }

        public override void MergeWith(NamespaceSymbol other)
        {
            base.MergeWith(other);

            classes.AddOrMergeWith(other.classes);
            enums.AddOrMergeWith(other.enums);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);

            classes.ReBase(newBase);
            enums.ReBase(newBase);
        }
    }
}
