using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.Site.Models
{
    public class NavigationNode
    {
        public string Label { get; }
        public UriPath Link { get; }
        public bool IsOpen { get; }

        private readonly List<NavigationNode> children = [];
        public IEnumerable<NavigationNode> Children => children;
        public bool HasChildren => children.Count > 0;

        public NavigationNode(string label, UriPath link, bool isOpen = false)
        {
            Label = label;
            Link = link;
            IsOpen = isOpen;
        }

        public void Add(NavigationNode node)
        {
            children.Add(node);
        }
    }
}
