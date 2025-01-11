using Fasciculus.Net.Navigating;
using Fasciculus.Site.Blog.Compilers;
using Fasciculus.Site.Blog.Models;
using Fasciculus.Site.Blog.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Blog.Services
{
    public class BlogContent
    {
        private readonly SortedSet<BlogYear> years;
        private readonly SortedSet<BlogMonth> months;
        private readonly SortedSet<BlogEntry> entries;

        private readonly Dictionary<UriPath, BlogItem> items;

        public BlogContent(BlogDocuments documents, BlogCompiler compiler)
        {
            BlogItemComparer comparer = BlogItemComparer.Instance;

            years = new(compiler.Compile(documents), comparer);
            months = new(years.SelectMany(y => y), comparer);
            entries = new(months.SelectMany(m => m), comparer);
            items = years.Cast<BlogItem>().Concat(months).Concat(entries).ToDictionary(i => i.Link);
        }

        public BlogItem GetItem(UriPath link)
            => items[link];

        public IEnumerable<BlogEntry> Newest(int count = 1)
            => entries.Take(Math.Min(entries.Count, count));
    }
}
