using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public abstract class TypeBuilder<T> : TypeReceiver<T>, ICommentReceiver, IMemberReceiver
        where T : notnull, TypeSymbol<T>
    {
        protected FieldList fields = [];
        protected MemberList members = [];
        protected EventList events = [];
        protected PropertyList properties = [];

        public TypeBuilder(CommentContext commentContext)
            : base(commentContext) { }

        public void Add(FieldSymbol field)
        {
            fields.AddOrMergeWith(field);
        }

        public void Add(MemberSymbol member)
        {
            members.AddOrMergeWith(member);
        }

        public void Add(EventSymbol @event)
        {
            events.AddOrMergeWith(@event);
        }

        public void Add(PropertySymbol property)
        {
            properties.AddOrMergeWith(property);
        }

        protected void Populate(T type)
        {
            fields.Apply(type.Add);
            members.Apply(type.Add);
            events.Apply(type.Add);
            properties.Apply(type.Add);
        }
    }
}
