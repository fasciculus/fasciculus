using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Fasciculus.Collections
{
    /// <summary>
    /// Type-safe converter for <see cref="NotifyCollectionChangedEventArgs"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypedNotifyCollectionChangedEventArgs<T> : EventArgs
        where T : notnull
    {
        /// <summary>
        /// Action as in <see cref="NotifyCollectionChangedEventArgs.Action"/>
        /// </summary>
        public NotifyCollectionChangedAction Action { get; }

        /// <summary>
        /// Gets the list of new items involved in the change. Never <c>null</c>.
        /// </summary>
        public List<T> NewItems { get; }

        /// <summary>
        /// Gets the index at which the change occurred.
        /// </summary>
        public int NewStartingIndex { get; }

        /// <summary>
        /// Gets the list of items affected by a <c>Replace</c>, <c>Remove</c> or <c>Move</c> action.
        /// </summary>
        public List<T> OldItems { get; }

        /// <summary>
        /// Gets the index at which a <c>Replace</c>, <c>Remove</c> or <c>Move</c> action occurred.
        /// </summary>
        public int OldStartingIndex { get; }

        /// <summary>
        /// Initializes this event args from the given <paramref name="args"/>.
        /// </summary>
        public TypedNotifyCollectionChangedEventArgs(NotifyCollectionChangedEventArgs args)
        {
            Action = args.Action;
            NewItems = args.NewItems?.OfType<T>().NotNull().ToList() ?? [];
            NewStartingIndex = args.NewStartingIndex;
            OldItems = args.OldItems?.OfType<T>().NotNull().ToList() ?? [];
            OldStartingIndex = args.OldStartingIndex;
        }
    }
}
