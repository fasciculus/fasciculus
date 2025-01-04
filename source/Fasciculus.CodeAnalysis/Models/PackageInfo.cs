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
        /// Merges this package with the given <paramref name="other"/> package.
        /// </summary>
        /// <param name="other"></param>
        public void Add(PackageInfo other)
        {
            Frameworks.Add(other.Frameworks);
            Namespaces.Add(other.Namespaces);
        }

        /// <summary>
        /// Adds the given <paramref name="framework"/> to this element and to contained elements.
        /// </summary>
        public override void Add(TargetFramework framework)
        {
            Frameworks.Add(framework);
            Namespaces.Add(framework);
        }
    }
}
