using System;
using System.Collections.Generic;

namespace Fasciculus.Support.Versioning
{
    /// <summary>
    /// A comparer to compare versions stored in strings.
    /// </summary>
    public class VersionComparer : IComparer<string>
    {
        private VersionComparer() { }

        /// <summary>
        /// Compares the given string versions converting then to <see cref="Version"/> first.
        /// </summary>
        public int Compare(string? x, string? y)
        {
            Version a = VersionFactory.Create(x ?? string.Empty);
            Version b = VersionFactory.Create(y ?? string.Empty);

            return a.CompareTo(b);
        }

        /// <summary>
        /// The singleton instance of this comparer.
        /// </summary>
        public static VersionComparer Instance { get; } = new();
    }
}
