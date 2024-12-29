using Fasciculus.Collections;
using Fasciculus.Maui.Threading;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Fasciculus.Maui.Collections
{
    /// <summary>
    /// An observable sorted set propagating events on the main thread.
    /// </summary>
    public class MainThreadSortedSet<T> : ObservableSortedSet<T>
        where T : notnull
    {
        /// <summary>
        /// Initializes a set with the given <paramref name="collection"/> and the given <paramref name="comparer"/>.
        /// </summary>
        public MainThreadSortedSet(IEnumerable<T> collection, IComparer<T> comparer)
            : base(collection, comparer) { }

        /// <summary>
        /// Initializes a set with the given <paramref name="collection"/>
        /// </summary>
        public MainThreadSortedSet(IEnumerable<T> collection)
            : base(collection) { }

        /// <summary>
        /// Initializes a set with the given <paramref name="comparer"/>.
        /// </summary>
        public MainThreadSortedSet(IComparer<T> comparer)
            : base(comparer) { }

        /// <summary>
        /// Initializes an empty set.
        /// </summary>
        public MainThreadSortedSet() { }

        /// <summary>
        /// Called when a property of this set changed.
        /// </summary>
        protected override void OnPropertyChanged(string name)
            => Threads.RunInMainThread(() => { base.OnPropertyChanged(name); });

        /// <summary>
        /// Called when the contents of this set changed.
        /// </summary>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
            => Threads.RunInMainThread(() => { base.OnCollectionChanged(args); });
    }
}
