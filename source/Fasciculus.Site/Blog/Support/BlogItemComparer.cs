using Fasciculus.Site.Blog.Models;
using System.Collections.Generic;

namespace Fasciculus.Site.Blog.Support
{
    public class BlogItemComparer : IComparer<BlogItem>
    {
        public static readonly BlogItemComparer Instance = new();

        private BlogItemComparer() { }

        public int Compare(BlogItem? x, BlogItem? y)
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
