using Fasciculus.Site.Blog.Models;
using System.Collections.Generic;

namespace Fasciculus.Site.Blog.Support
{
    public class BlogEntryComparer : IComparer<BlogEntry>
    {
        public static readonly BlogEntryComparer Instance = new();

        private BlogEntryComparer() { }

        public int Compare(BlogEntry? x, BlogEntry? y)
        {
            if (x is null)
            {
                return y is null ? 1 : 0;
            }

            if (y is null)
            {
                return 1;
            }

            int result = y.Published.CompareTo(x.Published);

            if (result == 0)
            {
                result = x.Link.CompareTo(y.Link);
            }

            return result;
        }
    }
}
