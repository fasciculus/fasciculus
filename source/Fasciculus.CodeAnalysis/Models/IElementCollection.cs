using Fasciculus.CodeAnalysis.Frameworks;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    /// <summary>
    /// Collection of elements.
    /// </summary>
    public interface IElementCollection<T> : IEnumerable<T>
        where T : notnull, ElementInfo
    {
        /// <summary>
        /// Adds the given <paramref name="framework"/>
        /// </summary>
        public void Add(TargetFramework framework);
    }
}
