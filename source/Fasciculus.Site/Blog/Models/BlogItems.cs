using Fasciculus.Net.Navigating;
using Fasciculus.Site.Blog.Support;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Fasciculus.Site.Blog.Models
{
    public class BlogItems<T> : IEnumerable<T>
        where T : notnull, BlogItem
    {
        private readonly SortedSet<T> items = new(BlogItemComparer.Instance);

        public bool TryGetItem(UriPath link, [NotNullWhen(true)] out T? item)
        {
            item = items.FirstOrDefault(i => i.Link == link);

            return item is not null;
        }

        public void Add(T item)
            => items.Add(item);

        public IEnumerator<T> GetEnumerator()
            => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => items.GetEnumerator();
    }
}
