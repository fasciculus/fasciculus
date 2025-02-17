using Fasciculus.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Net.Navigating
{
    /// <summary>
    /// A forest of <see cref="NavigationNode"/> trees.
    /// </summary>
    public class NavigationForest
    {
        private readonly List<NavigationNode> trees = [];

        /// <summary>
        /// The trees in this forest.
        /// </summary>
        public IEnumerable<NavigationNode> Trees => trees;

        private readonly Dictionary<UriPath, NavigationNode> byLink = [];

        /// <summary>
        /// Returns the node with the given <paramref name="link"/> if such exists.
        /// </summary>
        public NavigationNode? this[UriPath link]
            => byLink.TryGetValue(link, out NavigationNode? node) ? node : null;

        /// <summary>
        /// Initializes a new forest with the given <paramref name="trees"/>.
        /// </summary>
        public NavigationForest(IEnumerable<NavigationNode> trees)
        {
            trees.Apply(Add);
        }

        /// <summary>
        /// Initializes a new forest.
        /// </summary>
        public NavigationForest()
            : this([]) { }

        /// <summary>
        /// Adds the given <paramref name="tree"/> to this forest.
        /// </summary>
        public void Add(NavigationNode tree)
        {
            trees.Add(tree);
            AddNode(tree);
        }

        private void AddNode(NavigationNode node)
        {
            byLink.TryAdd(node.Link, node);

            node.Children.Apply(AddNode);
        }

        /// <summary>
        /// Returns the nodes leading to the given <paramref name="link"/>.
        /// </summary>
        public virtual List<NavigationNode> PathTo(UriPath link)
        {
            IEnumerable<NavigationNode> result = [];

            for (NavigationNode? n = this[link]; n is not null; n = n.Parent)
            {
                result = result.Prepend(n);
            }

            return result.ToList();
        }
    }
}
