using System.Collections.Generic;

namespace Fasciculus.Net.Navigating
{
    /// <summary>
    /// A forest of <see cref="NavigationNode"/> trees.
    /// </summary>
    public class NavigationForest
    {
        /// <summary>
        /// The prefix to use when displaying the forest.
        /// </summary>
        public UriPath Prefix { get; }

        private readonly List<NavigationNode> trees;

        /// <summary>
        /// The trees in this forest.
        /// </summary>
        public IEnumerable<NavigationNode> Trees => trees;

        /// <summary>
        /// Initializes a new forest with the given <paramref name="trees"/> and the optionally given <paramref name="prefix"/>.
        /// </summary>
        public NavigationForest(IEnumerable<NavigationNode> trees, UriPath? prefix = null)
        {
            Prefix = prefix ?? UriPath.Empty;

            this.trees = new(trees);
        }

        /// <summary>
        /// Initializes a new forest with the optionally given <paramref name="prefix"/>.
        /// </summary>
        public NavigationForest(UriPath? prefix = null)
            : this([], prefix) { }

        /// <summary>
        /// Adds the given <paramref name="tree"/> to this forest.
        /// </summary>
        public void Add(NavigationNode tree)
        {
            trees.Add(tree);
        }
    }
}
