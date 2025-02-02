using System.Collections.Generic;
using System.Collections.Specialized;

namespace Fasciculus.Collections
{
    /// <summary>
    /// An enumerable implementing both <see cref="IEnumerable{T}"/> and <see cref="INotifyCollectionChanged"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INotifyingEnumerable<T> : IEnumerable<T>, INotifyCollectionChanged
        where T : notnull
    {
    }
}
