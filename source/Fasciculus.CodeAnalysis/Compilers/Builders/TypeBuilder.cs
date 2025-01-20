using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public abstract class TypeBuilder<T> : TypeReceiver<T>, ICommentReceiver, IMemberReceiver
        where T : notnull, Symbol<T>
    {
        protected PropertyList properties = [];

        public void Add(PropertySymbol property)
        {
            properties.AddOrMergeWith(property);
        }
    }
}
