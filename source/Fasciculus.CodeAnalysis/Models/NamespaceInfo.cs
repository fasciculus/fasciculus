using Fasciculus.CodeAnalysis.Frameworks;

namespace Fasciculus.CodeAnalysis.Models
{
    /// <summary>
    /// Information of a namespace.
    /// </summary>
    public class NamespaceInfo : ElementInfo
    {
        /// <summary>
        /// Adds the given <paramref name="framework"/> to this element and to contained elements.
        /// </summary>
        public override void Add(TargetFramework framework)
        {
            Frameworks.Add(framework);
        }

        /// <summary>
        /// Merges the given <paramref name="namespace"/> into this namespace.
        /// </summary>
        public void Add(NamespaceInfo @namespace)
        {
            Frameworks.Add(@namespace.Frameworks);
        }
    }
}
