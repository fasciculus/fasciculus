using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface INamespaceSymbol : ISymbol
    {
        public IEnumerable<IEnumSymbol> Enums { get; }

        public IEnumerable<IInterfaceSymbol> Interfaces { get; }

        public IEnumerable<IClassSymbol> Classes { get; }
    }

    internal class NamespaceSymbol : Symbol<NamespaceSymbol>, INamespaceSymbol
    {
        public static SymbolModifiers NamespaceModifiers
            => new() { IsPublic = true };

        private readonly EnumList enums = [];
        private readonly InterfaceList interfaces = [];
        private readonly ClassList classes = [];

        public IEnumerable<IEnumSymbol> Enums => enums;
        public IEnumerable<IInterfaceSymbol> Interfaces => interfaces;
        public IEnumerable<IClassSymbol> Classes => classes;

        public virtual bool IsEmpty
            => enums.Count == 0 && interfaces.Count == 0 && classes.Count == 0;

        public override bool IsAccessible
            => enums.HasAccessible || interfaces.HasAccessible || classes.HasAccessible;

        public NamespaceSymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Namespace, framework, package, comment)
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
