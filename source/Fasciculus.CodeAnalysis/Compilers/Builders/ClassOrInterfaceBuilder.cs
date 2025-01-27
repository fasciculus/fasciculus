using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal abstract class ClassOrInterfaceBuilder<T> : TypeBuilder<T>
        where T : notnull, ClassOrInterfaceSymbol<T>
    {
        public ClassOrInterfaceBuilder(CommentContext commentContext)
            : base(commentContext) { }

        protected override void Populate(T classOrInterface)
        {
            base.Populate(classOrInterface);

            events.Apply(classOrInterface.Add);
            properties.Apply(classOrInterface.Add);
        }
    }
}
