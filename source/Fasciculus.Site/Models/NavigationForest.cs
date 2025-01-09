using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.Site.Models
{
    public class NavigationForest
    {
        public UriPath Prefix { get; }
        private readonly List<NavigationNode> roots = [];
        public IEnumerable<NavigationNode> Roots => roots;

        public NavigationForest(UriPath prefix)
        {
            Prefix = prefix;
        }

        public void Add(NavigationNode root)
        {
            roots.Add(root);
        }
    }
}
