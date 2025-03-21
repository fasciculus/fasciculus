using Fasciculus.Net.Navigating;
using System;
using System.Collections.Generic;

namespace Fasciculus.Site.Blog.Models
{
    public class BlogEntry : BlogItem
    {
        public string Summary { get; }
        public string Html { get; }

        public override IEnumerable<UriPath> Children => [];

        public BlogEntry? Prev { get; set; }
        public BlogEntry? Next { get; set; }

        public BlogEntry(UriPath link, string title, DateTime published, string summary, string html)
            : base(BlogItemKind.Entry, link, title, published)
        {
            Summary = summary;
            Html = html;
        }
    }
}
