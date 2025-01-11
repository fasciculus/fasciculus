using Fasciculus.Net.Navigating;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Blog.Models
{
    public class BlogMonth : BlogItem, IEnumerable<BlogEntry>
    {
        private readonly string name;
        private readonly BlogItems<BlogEntry> entries = [];

        public override string Title => $"{name} ({entries.Count()})";

        public override IEnumerable<UriPath> Children
            => entries.Select(e => e.Link);

        public BlogMonth(UriPath link, DateTime published)
            : base(BlogItemKind.Month, link, string.Empty, published)
        {
            name = published.ToString("MMMM", DateTimeFormat);
        }

        public void Add(BlogEntry entry)
            => entries.Add(entry);

        public IEnumerator<BlogEntry> GetEnumerator()
            => entries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => entries.GetEnumerator();
    }
}
