using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class ClassBuilder : TypeBuilder<ClassSymbol>
    {
        private ConstructorList constructors = [];

        public ClassBuilder(CommentContext commentContext)
            : base(commentContext) { }

        public override ClassSymbol Build()
        {
            ClassSymbol @class = new(Framework, Package, Comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
            };

            Populate(@class);

            constructors.Apply(@class.Add);

            return @class;
        }

        public void Add(ConstructorSymbol constructor)
            => constructors.AddOrMergeWith(constructor);
    }
}
