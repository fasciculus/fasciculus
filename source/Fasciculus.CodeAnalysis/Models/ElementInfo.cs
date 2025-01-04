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
        public required string Name { get; init; }

        /// <summary>
        /// The target frameworks this element is valid for.
        /// </summary>
        public TargetFrameworks Frameworks { get; } = new();

        /// <summary>
        /// Adds the given <paramref name="framework"/> to this element and to contained elements.
        /// </summary>
        public abstract void Add(TargetFramework framework);
    }
}
