using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public abstract class TypeBuilder<T> : TypeReceiver<T>, ICommentReceiver, IMemberReceiver
        where T : notnull, TypeSymbol<T>
    {
        protected FieldList fields = [];
        protected PropertyList properties = [];

        public void Add(FieldSymbol field)
        {
            fields.AddOrMergeWith(field);
        }

        public void Add(PropertySymbol property)
        {
            properties.AddOrMergeWith(property);
        }

        protected void Populate(T type)
        {
            fields.Apply(type.Add);
            properties.Apply(type.Add);
        }
    }
}
