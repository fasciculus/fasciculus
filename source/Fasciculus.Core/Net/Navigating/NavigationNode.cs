using System.Collections.Generic;

namespace Fasciculus.Net.Navigating
{
    /// <summary>
    /// A node within a navigation tree.
    /// </summary>
    public class NavigationNode
    {
        /// <summary>
        /// The label to display
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// The link
        /// </summary>
        public UriPath Link { get; }

        /// <summary>
        /// Whether this node is open and displays its children
        /// </summary>
        public bool IsOpen { get; }

        private readonly List<NavigationNode> children;

        /// <summary>
        /// The children of this node, ordered by Label.
        /// </summary>
        public IEnumerable<NavigationNode> Children => children;

        /// <summary>
        /// Whether this node has children
        /// </summary>
        public bool HasChildren => children.Count > 0;

        /// <summary>
        /// Initializes a new node.
        /// </summary>
        public NavigationNode(string label, UriPath link, IEnumerable<NavigationNode> children, bool isOpen = false)
        {
            Label = label;
            Link = link;
            IsOpen = isOpen;

            this.children = new(children);
        }

        /// <summary>
        /// Initializes a new node.
        /// </summary>
        public NavigationNode(string label, UriPath link, bool isOpen = false)
            : this(label, link, [], isOpen) { }

        /// <summary>
        /// Adds the given <paramref name="node"/> as child of this node.
        /// </summary>
        public void Add(NavigationNode node)
        {
            children.Add(node);
        }
    }
}
