using System.Collections.Generic;

namespace Fasciculus.Net.Navigating
{
    /// <summary>
    /// A forest of <see cref="NavigationNode"/> trees.
    /// </summary>
    public class NavigationForest
    {
        private readonly List<NavigationNode> trees;

        /// <summary>
        /// The trees in this forest.
        /// </summary>
        public IEnumerable<NavigationNode> Trees => trees;

        /// <summary>
        /// Initializes a new forest with the given <paramref name="trees"/>.
        /// </summary>
        public NavigationForest(IEnumerable<NavigationNode> trees)
        {
            this.trees = new(trees);
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
        }
    }
}
