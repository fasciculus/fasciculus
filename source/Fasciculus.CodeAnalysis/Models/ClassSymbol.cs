using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IClassSymbol : ITypeSymbol
    {
        public IEnumerable<IConstructorSymbol> Constructors { get; }
    }

    internal class ClassSymbol : TypeSymbol<ClassSymbol>, IClassSymbol
    {
        private readonly ConstructorList constructors;

        public IEnumerable<IConstructorSymbol> Constructors => constructors;

        public ClassSymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Class, framework, package, comment)
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
