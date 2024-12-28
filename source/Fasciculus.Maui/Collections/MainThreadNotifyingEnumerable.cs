using Fasciculus.Collections;
using Fasciculus.Maui.Threading;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Fasciculus.Maui.Collections
{
    /// <summary>
    /// A <see cref="ObservableNotifyingEnumerable{T}"/> propagating events on the main thread.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MainThreadNotifyingEnumerable<T> : ObservableNotifyingEnumerable<T>
        where T : notnull
    {
        /// <summary>
        /// Initializes a collection with the given <paramref name="source"/>.
        /// <para>
        /// Note: <paramref name="source"/> and <paramref name="notifier"/> must be the same object.
        /// </para>
        /// </summary>
        public MainThreadNotifyingEnumerable(IEnumerable<T> source, INotifyCollectionChanged notifier)
            : base(source, notifier) { }

        /// <summary>
        /// Initializes a collection with the given <paramref name="source"/>.
        /// </summary>
        public MainThreadNotifyingEnumerable(INotifyingEnumerable<T> source)
            : base(source) { }

        /// <summary>
        /// Raise CollectionChanged event within the main thread.
        /// </summary>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            => Threads.RunInMainThread(() => { base.OnCollectionChanged(e); });

        /// <summary>
        /// Raises a PropertyChanged event within the main thread.
        /// </summary>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
            => Threads.RunInMainThread(() => { base.OnPropertyChanged(e); });
    }
}
