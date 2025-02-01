using System;
using System.Collections;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    internal class SourceComparer : Comparer<Uri>
    {
        public static SourceComparer Instance = new SourceComparer();

        private SourceComparer() { }

        public override int Compare(Uri? x, Uri? y)
        {
            if (x is null)
            {
                return y is null ? 0 : -1;
            }

            if (y is null)
            {
                return -1;
            }

            return x.ToString().CompareTo(y.ToString());
        }
    }

    internal class SourceList : IEnumerable<Uri>
    {
        private readonly SortedSet<Uri> sources;

        public SourceList(IEnumerable<Uri> sources)
        {
            this.sources = new(sources, SourceComparer.Instance);
        }

        public SourceList()
            : this([]) { }

        public SourceList Clone()
            => new(this);

        public IEnumerator<Uri> GetEnumerator()
            => sources.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => sources.GetEnumerator();
    }
}
