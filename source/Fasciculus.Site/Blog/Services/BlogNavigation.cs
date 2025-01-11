using Fasciculus.Net.Navigating;
using System.Collections.Generic;

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
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<UriPath> GetChildren(UriPath link)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetLabel(UriPath link)
        {
            throw new System.NotImplementedException();
        }
    }
}
