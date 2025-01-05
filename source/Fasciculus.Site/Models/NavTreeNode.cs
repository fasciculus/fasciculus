using Fasciculus.Net;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Models
{
    public class NavTreeNode
    {
        public string Label { get; }
        public UriPath Link { get; }
        public bool IsOpen { get; }

        private readonly List<NavTreeNode> children = [];
        public IEnumerable<NavTreeNode> Children => children;
        public bool HasChildren => children.Any();

        public NavTreeNode(string label, UriPath link)
        {
            Label = label;
            Link = link;
        }
    }
}
