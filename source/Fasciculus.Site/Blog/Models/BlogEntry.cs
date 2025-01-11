using Fasciculus.Net.Navigating;
using System;

namespace Fasciculus.Site.Blog.Models
{
    public class BlogEntry
    {
        public UriPath Link { get; }

        public string Title { get; }

        public DateTime Published { get; }

        public string Content { get; }

        public BlogEntry(UriPath link, string title, DateTime published, string content)
        {
            Link = link;
            Title = title;
            Published = published;
            Content = content;
        }
    }
}
