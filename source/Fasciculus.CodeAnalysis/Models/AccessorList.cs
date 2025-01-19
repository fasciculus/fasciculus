using System.Collections;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class AccessorList : IEnumerable<AccessorInfo>
    {
        private readonly SortedSet<AccessorInfo> accessors = [];

        public IEnumerable<AccessorInfo> Accessors => accessors;

        public void Add(AccessorInfo accessor)
            => accessors.Add(accessor);

        public override string? ToString()
        {
            return accessors.Count == 0 ? "{ }" : "{ " + string.Join(" ", accessors) + " }";
        }

        public IEnumerator<AccessorInfo> GetEnumerator()
            => accessors.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => accessors.GetEnumerator();
    }
}
