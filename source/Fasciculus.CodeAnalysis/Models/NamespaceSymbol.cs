using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class NamespaceSymbol : Symbol<NamespaceSymbol>
    {
        public static SymbolModifiers NamespaceModifiers
            => new() { IsPublic = true };

        private readonly EnumList enums = [];
        private readonly InterfaceList interfaces = [];
        private readonly ClassList classes = [];

        public IEnumerable<EnumSymbol> Enums => enums;
        public IEnumerable<InterfaceSymbol> Interfaces => interfaces;
        public IEnumerable<ClassSymbol> Classes => classes;

        public virtual bool IsEmpty
            => enums.Count == 0 && interfaces.Count == 0 && classes.Count == 0;

        public override bool IsAccessible
            => enums.HasAccessible || interfaces.HasAccessible || classes.HasAccessible;

        public NamespaceSymbol(TargetFramework framework, string package)
            : base(SymbolKind.Namespace, framework, package)
        {
        }

        private NamespaceSymbol(NamespaceSymbol other, bool clone)
            : base(other, clone)
        {
            enums = other.enums.Clone();
            interfaces = other.interfaces.Clone();
            classes = other.classes.Clone();
        }

        public NamespaceSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment,
            };
        }

        public void AddOrMergeWith(EnumSymbol @enum)
        {
            enums.AddOrMergeWith(@enum);
        }

        public void AddOrMergeWith(InterfaceSymbol @interface)
        {
            interfaces.AddOrMergeWith(@interface);
        }

        public void AddOrMergeWith(ClassSymbol @class)
        {
            classes.AddOrMergeWith(@class);
        }

        public override void MergeWith(NamespaceSymbol other)
        {
            base.MergeWith(other);

            enums.AddOrMergeWith(other.enums);
            interfaces.AddOrMergeWith(other.interfaces);
            classes.AddOrMergeWith(other.classes);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);

            enums.ReBase(newBase);
            interfaces.ReBase(newBase);
            classes.ReBase(newBase);
        }
    }
}
