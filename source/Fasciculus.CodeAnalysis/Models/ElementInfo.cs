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
    }
}
