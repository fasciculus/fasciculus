using Fasciculus.Support.Versioning;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Frameworks
{
    /// <summary>
    /// Target product with versions
    /// </summary>
    public class TargetProduct
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }

        private readonly SortedSet<string> versions = new(VersionComparer.Instance);

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
