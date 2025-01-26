using System.Collections;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IAccessorList
    {
        public int Count { get; }

        public IEnumerable<IAccessorInfo> Accessors { get; }
    }

    internal class AccessorList : IAccessorList, IEnumerable<AccessorInfo>
    {
        private readonly SortedSet<AccessorInfo> accessors;

        public IEnumerable<IAccessorInfo> Accessors => accessors;

        public int Count => accessors.Count;

        public AccessorList()
        {
            accessors = [];
        }

        private AccessorList(AccessorList other, bool _)
        {
            accessors = new(other.accessors);
        }

        public void Add(AccessorInfo accessor)
            => accessors.Add(accessor);

        public override string? ToString()
        {
            return accessors.Count == 0 ? "{ }" : "{ " + string.Join(" ", accessors) + " }";
        }

        public AccessorList Clone()
            => new(this, true);

        public IEnumerator<AccessorInfo> GetEnumerator()
            => accessors.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => accessors.GetEnumerator();
    }
}
