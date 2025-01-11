using Fasciculus.Net.Navigating;
using System;

namespace Fasciculus.Site.Blog.Models
{
    public class BlogYears : BlogItems<BlogYear>
    {
        public void Add(BlogEntry entry)
        {
            UriPath yearLink = entry.Link.Parent.Parent;

            if (!TryGetItem(yearLink, out BlogYear? year))
            {
                DateTime published = new(entry.Published.Year, 1, 1);

                year = new(yearLink, published);
                Add(year);
            }

            year.Add(entry);
        }
    }
}
