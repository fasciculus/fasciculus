using Fasciculus.Net.Navigating;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Fasciculus.Site.Blog.Models
{
    public abstract class BlogItem
    {
        public static readonly DateTimeFormatInfo DateTimeFormat
            = CultureInfo.GetCultureInfo("en").DateTimeFormat;

        public BlogItemKind Kind { get; }

        public UriPath Link { get; }

        public virtual string Title { get; }

        public DateTime Published { get; }

        public abstract IEnumerable<UriPath> Children { get; }

        public BlogItem(BlogItemKind kind, UriPath link, string title, DateTime published)
        {
            Kind = kind;
            Link = link;
            Title = title;
            Published = published;
        }
    }
}
