﻿using System.Collections;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    /// <summary>
    /// Collection of packages.
    /// </summary>
    public class Packages : IEnumerable<PackageInfo>
    {
        private readonly Dictionary<string, PackageInfo> packages = [];

        /// <summary>
        /// Creates a new collection for the given <paramref name="packages"/>.
        /// </summary>
        public Packages(IEnumerable<PackageInfo> packages)
        {
            Add(packages);
        }

        /// <summary>
        /// Adds the given <paramref name="package"/> to this collection, merging it into an existing package of the
        /// same name if such one exists.
        /// </summary>
        public void Add(PackageInfo package)
        {
            if (packages.TryGetValue(package.Name, out PackageInfo? existing))
            {
                existing.Add(package);
            }
            else
            {
                packages.Add(package.Name, package);
            }
        }

        /// <summary>
        /// Adds the given <paramref name="packages"/> to this collection, merging them with existing packages of the
        /// same name if such exists.
        /// </summary>
        public void Add(IEnumerable<PackageInfo> packages)
            => packages.Apply(Add);

        /// <summary>
        /// Returns an enumerator that iterates through this collection.
        /// </summary>
        public IEnumerator<PackageInfo> GetEnumerator()
            => packages.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => packages.Values.GetEnumerator();
    }
}
