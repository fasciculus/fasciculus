using Fasciculus.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fasciculus.Net.Navigating
{
    /// <summary>
    /// An immutable list of strings representing the path of an URI.
    /// </summary>
    [DebuggerDisplay("{string.Join(\"/\", parts)}")]
    public partial class UriPath : IReadOnlyList<string>, IEquatable<UriPath>, IComparable<UriPath>
    {
        /// <summary>
        /// The empty path.
        /// </summary>
        public static readonly UriPath Empty = new();

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
        /// Returns the parent of this link. The parent of <see cref="Empty"/> is <see cref="Empty"/>.
        /// </summary>
        public UriPath Parent
            => Count > 0 ? new(parts.Take(Count - 1)) : Empty;

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
        /// Replaces <paramref name="count"/> parts of this path starting from <paramref name="start"/> with the given
        /// <paramref name="replacement"/>.
        /// </summary>
        public UriPath Replace(int start, int count, IEnumerable<string> replacement)
        {
            IEnumerable<string> prefix = parts.Take(start);
            IEnumerable<string> suffix = parts.Skip(start + count);

            return new(prefix.Concat(replacement).Concat(suffix));
        }

        /// <summary>
        /// Whether this link points to an ancestor of the given <paramref name="other"/> link.
        /// </summary>
        public bool IsAncestorOf(UriPath other)
            => Count < other.Count && Enumerable.SequenceEqual(parts, other.parts.Take(Count), Comparer);

        /// <summary>
        /// Whether this link points to a descendant of the given <paramref name="other"/> link.
        /// </summary>
        public bool IsDescendantOf(UriPath other)
            => other.IsAncestorOf(this);

        /// <summary>
        /// Whether this link points to itself or an ancestor of the given <paramref name="other"/> link.
        /// </summary>
        public bool IsSelfOrAncestorOf(UriPath other)
            => Equals(other) || IsAncestorOf(other);

        /// <summary>
        /// Whether this link points to itself or a descendant of the given <paramref name="other"/> link.
        /// </summary>
        public bool IsSelfOrDescendantOf(UriPath other)
            => other.IsSelfOrAncestorOf(this);

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
        /// Converts this path to an <see cref="Uri"/> prefixed with the given <paramref name="prefix"/>.
        /// </summary>
        public Uri ToUri(string prefix)
            => new(prefix + ToString());

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
