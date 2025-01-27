using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class ClassBuilder : TypeBuilder<ClassSymbol>
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

            return @class;
        }

        protected override void Populate(ClassSymbol @class)
        {
            base.Populate(@class);

            fields.Apply(@class.Add);
            constructors.Apply(@class.Add);
        }

        public void Add(ConstructorSymbol constructor)
            => constructors.AddOrMergeWith(constructor);
    }
}
