using Fasciculus.CodeAnalysis.Frameworking;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IClassSymbol : IClassOrInterfaceSymbol
    {
        public IEnumerable<IFieldSymbol> Fields { get; }

        public IEnumerable<IConstructorSymbol> Constructors { get; }
    }

    internal class ClassSymbol : ClassOrInterfaceSymbol<ClassSymbol>, IClassSymbol
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

        public void Add(FieldSymbol field)
            => fields.AddOrMergeWith(field);
    }
}
