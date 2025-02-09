using Fasciculus.Net.Navigating;
using Fasciculus.Site.Blog.Models;
using Fasciculus.Site.Models;
using Fasciculus.Site.Navigation;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Blog.Services
{
    public class BlogNavigation : NavigationFactory
    {
        private readonly BlogSiteContent content;

        public BlogNavigation(BlogSiteContent content)
        {
            this.content = content;
        }

        public SiteNavigation Create(UriPath selected)
        {
            NavigationForest forest = Create(content.Years.Select(y => y.Link), selected);
            SitePath path = [];

            return new() { Forest = forest, Path = path };
        }

        protected override IEnumerable<UriPath> GetChildren(UriPath link)
        {
            return content.GetItem(link).Children;
        }

        protected override int GetKind(UriPath link)
        {
            BlogItem item = content.GetItem(link);

            return item.Kind switch
            {
                BlogItemKind.Year => NavigationKind.BlogYear,
                BlogItemKind.Month => NavigationKind.BlogMonth,
                BlogItemKind.Entry => NavigationKind.BlogEntry,
                _ => NavigationKind.Unknown
            };
        }

        protected override string GetLabel(UriPath link)
        {
            return content.GetItem(link).Title;
        }
    }
}
