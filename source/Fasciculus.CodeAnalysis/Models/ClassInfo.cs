using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.Collections;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    /// <summary>
    /// Information of a class.
    /// </summary>
    public class ClassInfo : ElementInfo
    {
        /// <summary>
        /// Name without type parameters.
        /// </summary>
        public string UntypedName { get; }

        private readonly SortedSet<string> packages = [];

        /// <summary>
        /// The packages this namespace occurs in.
        /// </summary>
        public IEnumerable<string> Packages => packages;

        private readonly string[] parameters;

        /// <summary>
        /// Type parameters of this class if generic.
        /// </summary>
        public IEnumerable<string> Parameters => parameters;

        /// <summary>
        /// Initializes a new class.
        /// </summary>
        public ClassInfo(string name, string untypedName, IEnumerable<string> parameters)
            : base(name)
        {
            UntypedName = untypedName;

            this.parameters = [.. parameters];
        }

        /// <summary>
        /// Merges the given <paramref name="class"/> into this namespace.
        /// </summary>
        public void MergeWith(ClassInfo @class)
        {
            Frameworks.Add(@class.Frameworks);

            @class.packages.Apply(p => { packages.Add(p); });
        }

        /// <summary>
        /// Adds the given <paramref name="framework"/> to this element and to contained elements.
        /// </summary>
        public override void AddFramework(TargetFramework framework)
        {
            Frameworks.Add(framework);
        }

        /// <summary>
        /// Adds the given <paramref name="name"/> to the packages this namespace occurs in.
        /// </summary>
        public void AddPackage(string name)
        {
            packages.Add(name);
        }

    }
}
