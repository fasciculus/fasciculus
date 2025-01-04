using System;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Frameworks
{
    /// <summary>
    /// Target product with versions
    /// </summary>
    public class TargetProduct
    {
        /// <summary>
        /// Comparer to compare version strings
        /// </summary>
        public class VersionComparer : IComparer<string>
        {
            /// <summary>
            /// Compares the given versions
            /// </summary>
            public int Compare(string? x, string? y)
            {
                Version a = VersionFactory.Create(x ?? string.Empty);
                Version b = VersionFactory.Create(y ?? string.Empty);

                return a.CompareTo(b);
            }
        }

        private static readonly VersionComparer versionComparer = new();

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }

        private readonly SortedSet<string> versions = new(versionComparer);

        /// <summary>
        /// Versions
        /// </summary>
        public IEnumerable<string> Versions => versions;

        /// <summary>
        /// Initializes a target product.
        /// </summary>
        public TargetProduct(string name, IEnumerable<string> versions)
        {
            Name = name;

            Add(versions);
        }

        /// <summary>
        /// Initializes a target product.
        /// </summary>
        public TargetProduct(string name)
            : this(name, []) { }

        /// <summary>
        /// Adds the given <paramref name="versions"/> to this product.
        /// </summary>
        public void Add(IEnumerable<string> versions)
            => versions.Apply(Add);

        /// <summary>
        /// Adds the given <paramref name="version"/> to this product.
        /// </summary>
        public void Add(string version)
            => this.versions.Add(version);
    }
}
