using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IClassSymbol : ITypeSymbol
    {
        public IEnumerable<IFieldSymbol> Fields { get; }

        public IEnumerable<IConstructorSymbol> Constructors { get; }
    }

    internal class ClassSymbol : TypeSymbol<ClassSymbol>, IClassSymbol
    {
        private readonly FieldList fields;

        public IEnumerable<IFieldSymbol> Fields => fields;

        private readonly ConstructorList constructors;

        public IEnumerable<IConstructorSymbol> Constructors => constructors;

        public ClassSymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Class, framework, package, comment)
        {
            fields = [];
            constructors = [];
        }

        private ClassSymbol(ClassSymbol other, bool clone)
            : base(other, clone)
        {
            fields = other.fields.Clone();
            constructors = other.constructors.Clone();
        }

        public ClassSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
            };
        }

        public void Add(ConstructorSymbol constructor)
            => constructors.AddOrMergeWith(constructor);

        public override void MergeWith(ClassSymbol other)
        {
            base.MergeWith(other);

            fields.AddOrMergeWith(other.fields);
            constructors.AddOrMergeWith(other.constructors);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);

            fields.ReBase(newBase);
            constructors.ReBase(newBase);
        }

        public void Add(FieldSymbol field)
            => fields.AddOrMergeWith(field);
    }
}
