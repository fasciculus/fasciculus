using Fasciculus.CodeAnalysis.Frameworks;

namespace Fasciculus.CodeAnalysis.Models
{
    /// <summary>
    /// Basic information of an element.
    /// </summary>
    public abstract class ElementInfo
    {
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The target frameworks this element is valid for.
        /// </summary>
        public TargetFrameworks Frameworks { get; } = [];

        /// <summary>
        /// Initializes a new element
        /// </summary>
        protected ElementInfo(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Adds the given <paramref name="framework"/> to this element and to contained elements.
        /// </summary>
        public abstract void AddFramework(TargetFramework framework);
    }
}
