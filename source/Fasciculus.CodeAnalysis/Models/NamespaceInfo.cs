using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Collections;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    /// <summary>
    /// Information of a namespace.
    /// </summary>
    public class NamespaceInfo : ElementInfo
    {
        private readonly SortedSet<string> packages = [];

        /// <summary>
        /// The packages this namespace occurs in.
        /// </summary>
        public IEnumerable<string> Packages => packages;

        /// <summary>
        /// The classes in this namespace.
        /// </summary>
        public ClassCollection Classes = [];

        /// <summary>
        /// Initiaizes a new namespace.
        /// </summary>
        /// <param name="name"></param>
        public NamespaceInfo(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Merges the given <paramref name="namespace"/> into this namespace.
        /// </summary>
        public void MergeWith(NamespaceInfo @namespace)
        {
            Frameworks.Add(@namespace.Frameworks);

            @namespace.packages.Apply(p => { packages.Add(p); });
            @namespace.Classes.Apply(c => { Classes.MergeWith(c); });
        }

        /// <summary>
        /// Adds the given <paramref name="framework"/> to this element and to contained elements.
        /// </summary>
        public override void AddFramework(TargetFramework framework)
        {
            Frameworks.Add(framework);
            Classes.AddFramework(framework);
        }

        /// <summary>
        /// Adds the given <paramref name="name"/> to the packages this namespace occurs in.
        /// </summary>
        public void AddPackage(string name)
        {
            packages.Add(name);
            Classes.AddPackage(name);
        }
    }
}
