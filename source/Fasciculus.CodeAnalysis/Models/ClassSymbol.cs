using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class ClassSymbol : TypeSymbol<ClassSymbol>
    {
        private readonly ConstructorList constructors;

        public IEnumerable<ConstructorSymbol> Constructors => constructors;

        public ClassSymbol(TargetFramework framework, string package)
            : base(SymbolKind.Class, framework, package)
        {
            constructors = [];
        }

        private ClassSymbol(ClassSymbol other, bool clone)
            : base(other, clone)
        {
            constructors = other.constructors.Clone();
        }

        public ClassSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment,
            };
        }

        public void Add(ConstructorSymbol constructor)
            => constructors.AddOrMergeWith(constructor);

        public override void MergeWith(ClassSymbol other)
        {
            base.MergeWith(other);

            constructors.AddOrMergeWith(other.constructors);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);

            constructors.ReBase(newBase);
        }
    }
}
