using System.Collections;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Frameworks
{
    /// <summary>
    /// Collection of target frameworks.
    /// </summary>
    public class TargetFrameworks : IEnumerable<TargetFramework>
    {
        private readonly HashSet<TargetFramework> frameworks = [];

        /// <summary>
        /// Whether the given <paramref name="targetFramework"/> is already in this collection.
        /// </summary>
        /// <param name="targetFramework"></param>
        /// <returns></returns>
        public bool Contains(TargetFramework targetFramework)
            => frameworks.Contains(targetFramework);

        /// <summary>
        /// Adds the given <paramref name="targetFramework"/> to this collection.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the framework is added, <c>false</c> if the framework is already present.
        /// </returns>
        public bool Add(TargetFramework targetFramework)
            => frameworks.Add(targetFramework);

        /// <summary>
        /// Adds the given <paramref name="targetFrameworks"/> to this collection.
        /// </summary>
        /// <returns>This collection.</returns>
        public TargetFrameworks Add(IEnumerable<TargetFramework> targetFrameworks)
        {
            targetFrameworks.Apply(f => { Add(f); });

            return this;
        }

        /// <summary>
        /// Returns an enumerator that iterates through this collection.
        /// </summary>
        public IEnumerator<TargetFramework> GetEnumerator()
            => frameworks.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => frameworks.GetEnumerator();
    }
}
