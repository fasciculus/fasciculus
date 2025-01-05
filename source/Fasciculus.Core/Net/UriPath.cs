using Fasciculus.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Net
{
    /// <summary>
    /// An immutable list of strings representing the path of an URI.
    /// </summary>
    public partial class UriPath : IReadOnlyList<string>, IEquatable<UriPath>, IComparable<UriPath>
    {
        /// <summary>
        /// The comparer used in this class. Set to <see cref="StringComparer.InvariantCultureIgnoreCase"/>
        /// </summary>
        public static readonly StringComparer Comparer = StringComparer.InvariantCultureIgnoreCase;

        private readonly List<string> parts;

        /// <summary>
        /// Returns the number of parts in this collection.
        /// </summary>
        public int Count => parts.Count;

        /// <summary>
        /// Returns the part at the given position.
        /// </summary>
        public string this[int index] => parts[index];

        /// <summary>
        /// Initializes a new list.
        /// </summary>
        public UriPath(IEnumerable<string> parts)
        {
            this.parts = new(parts);
        }

        /// <summary>
        /// Initializes a new list.
        /// </summary>
        public UriPath(params string[] parts)
            : this(parts.AsEnumerable()) { }

        /// <summary>
        /// Returns a new Link with the given part appended.
        /// </summary>
        public UriPath Append(string part)
            => new(parts.Append(part));

        /// <summary>
        /// Whether this link points to a parent of the given <paramref name="other"/> link.
        /// </summary>
        public bool IsParentOf(UriPath other)
            => Count < other.Count && Enumerable.SequenceEqual(parts, other.parts.Take(Count), Comparer);

        /// <summary>
        /// Whether this link points to a child of the given <paramref name="other"/> link.
        /// </summary>
        public bool IsChildOf(UriPath other)
            => other.IsParentOf(this);

        /// <summary>
        /// Returns an enumerator that iterates through this collection.
        /// </summary>
        public IEnumerator<string> GetEnumerator()
            => parts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => parts.GetEnumerator();

        /// <summary>
        /// Returns a hash code for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => parts.ToHashCode();

        /// <summary>
        /// Returns the parts of this link joined by a <c>/</c>.
        /// </summary>
        public override string ToString()
            => string.Join("/", parts);

        /// <summary>
        /// Indicates whether the current link is equal to another link.
        /// <para>
        /// Comparison is done using <see cref="Comparer"/>
        /// </para>
        /// </summary>
        public bool Equals(UriPath? other)
            => other is not null && Enumerable.SequenceEqual(parts, other.parts, Comparer);

        /// <summary>
        /// Indicates whether the current link is equal to another link.
        /// <para>
        /// Comparison is done using <see cref="StringComparer.InvariantCultureIgnoreCase"/>
        /// </para>
        /// </summary>
        public override bool Equals(object? obj)
            => Equals(obj as UriPath);

        /// <summary>
        /// Compares this link to the <paramref name="other"/> link.
        /// <para>
        /// Comparison is done using <see cref="StringComparer.InvariantCultureIgnoreCase"/>
        /// </para>
        /// </summary>
        public int CompareTo(UriPath? other)
            => other is null ? -1 : parts.SequenceCompare(other.parts, StringComparer.InvariantCultureIgnoreCase);
    }
}
