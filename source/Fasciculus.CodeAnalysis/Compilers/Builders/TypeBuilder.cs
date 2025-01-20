using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public abstract class TypeBuilder<T> : TypeReceiver<T>, ICommentReceiver, IMemberReceiver
        where T : notnull, TypeSymbol<T>
    {
        protected PropertyList properties = [];

        public void Add(PropertySymbol property)
        {
            properties.AddOrMergeWith(property);
        }

        protected void Populate(T type)
        {
            properties.Apply(type.Add);
        }
    }
}
