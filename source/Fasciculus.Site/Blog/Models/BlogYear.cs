using Fasciculus.Net.Navigating;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Blog.Models
{
    public class BlogYear : BlogItem, IEnumerable<BlogMonth>
    {
        private readonly string name;
        private readonly BlogItems<BlogMonth> months = [];

        public override string Title
            => $"{name} ({months.Sum(m => m.Count())})";

        public override IEnumerable<UriPath> Children
            => months.Select(m => m.Link);

        public BlogYear(UriPath link, DateTime published)
            : base(link, string.Empty, published)
        {
            name = published.ToString("yyyy");
        }

        public void Add(BlogEntry entry)
        {
            UriPath monthLink = entry.Link.Parent;

            if (!months.TryGetItem(monthLink, out BlogMonth? month))
            {
                DateTime published = new(entry.Published.Year, entry.Published.Month, 1);

                month = new(monthLink, published);
                months.Add(month);
            }

            month.Add(entry);
        }

        public IEnumerator<BlogMonth> GetEnumerator()
            => months.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => months.GetEnumerator();
    }
}
