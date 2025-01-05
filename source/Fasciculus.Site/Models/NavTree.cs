using System.Collections.Generic;

namespace Fasciculus.Site.Models
{
    public class NavTree
    {
        private readonly List<NavTreeNode> roots = [];

        public IEnumerable<NavTreeNode> Roots => roots;
    }
}
