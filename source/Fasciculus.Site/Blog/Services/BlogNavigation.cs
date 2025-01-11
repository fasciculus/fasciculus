using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Blog.Services
{
    public class BlogNavigation : NavigationFactory
    {
        private readonly BlogContent content;

        public BlogNavigation(BlogContent content)
        {
            this.content = content;
        }

        public NavigationForest Create(UriPath selected)
        {
            return Create(content.Years.Select(y => y.Link), selected);
        }

        protected override IEnumerable<UriPath> GetChildren(UriPath link)
        {
            return content.GetItem(link).Children;
        }

        protected override string GetLabel(UriPath link)
        {
            return content.GetItem(link).Title;
        }
    }
}
