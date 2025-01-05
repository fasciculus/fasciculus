using Fasciculus.CodeAnalysis.Frameworks;

namespace Fasciculus.CodeAnalysis.Models
{
    /// <summary>
    /// Information of a package
    /// </summary>
    public class PackageInfo : ElementInfo
    {
        /// <summary>
        /// The namespaces of this package.
        /// </summary>
        public NamespaceCollection Namespaces { get; } = [];

        /// <summary>
        /// Initializes a new package.
        /// </summary>
        /// <param name="name"></param>
        public PackageInfo(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Merges this package with the given <paramref name="other"/> package.
        /// </summary>
        /// <param name="other"></param>
        public void MergeWith(PackageInfo other)
        {
            Frameworks.Add(other.Frameworks);
            Namespaces.MergeWith(other.Namespaces);
        }

        /// <summary>
        /// Adds the given <paramref name="framework"/> to this element and to contained elements.
        /// </summary>
        public override void AddFramework(TargetFramework framework)
        {
            Frameworks.Add(framework);
            Namespaces.AddFramework(framework);
        }
    }
}
