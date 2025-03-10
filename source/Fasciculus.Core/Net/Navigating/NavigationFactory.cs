using Fasciculus.Threading.Synchronization;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Net.Navigating
{
    /// <summary>
    /// Factory to create <see cref="NavigationForest"/>s.
    /// </summary>
    public abstract class NavigationFactory
    {
        private readonly TaskSafeMutex mutex = new();

        private UriPath selected = UriPath.Empty;

        /// <summary>
        /// Creates a new forest with the given <paramref name="roots"/>.
        /// </summary>
        public NavigationForest Create(IEnumerable<UriPath> roots, UriPath selected)
        {
            using Locker locker = Locker.Lock(mutex);

            this.selected = selected;

            IEnumerable<NavigationNode> trees = Visit(roots);

            return new(trees);
        }

        /// <summary>
        /// Returns the kind for the node with the given <paramref name="link"/>.
        /// </summary>
        protected abstract int GetKind(UriPath link);

        /// <summary>
        /// Returns the label for the node with the given <paramref name="link"/>.
        /// </summary>
        protected abstract string GetLabel(UriPath link);

        /// <summary>
        /// Whether the given <paramref name="link"/> is "open".
        /// </summary>
        protected virtual bool IsOpen(UriPath link, UriPath selected)
            => link.IsSelfOrAncestorOf(selected);

        /// <summary>
        /// Returns the children for the node with the given <paramref name="link"/>.
        /// </summary>
        protected abstract IEnumerable<UriPath> GetChildren(UriPath link);

        private NavigationNode Visit(UriPath link)
        {
            int kind = GetKind(link);
            string label = GetLabel(link);
            bool isOpen = IsOpen(link, selected);
            IEnumerable<NavigationNode> children = Visit(GetChildren(link));

            return new(kind, label, link, children, isOpen);
        }

        private IEnumerable<NavigationNode> Visit(IEnumerable<UriPath> links)
        {
            return links.Select(Visit);
        }
    }
}
