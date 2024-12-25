using System.IO;

namespace Fasciculus.IO.Resources
{
    /// <summary>
    /// Exception thrown by <see cref="EmbeddedResources.Find(string)"/>.
    /// </summary>
    public class ResourceNotFoundException : IOException
    {
        /// <summary>
        /// Initializes a new exception without further information.
        /// </summary>
        public ResourceNotFoundException() { }

        /// <summary>
        /// Initializes a new exception with the given <paramref name="name"/>.
        /// </summary>
        public ResourceNotFoundException(string name)
            : base(name) { }
    }
}
