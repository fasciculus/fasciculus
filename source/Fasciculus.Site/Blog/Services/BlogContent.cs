using Fasciculus.Docs.Content.Services;
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

        private readonly Dictionary<UriPath, BlogYear> yearMap;
        private readonly Dictionary<UriPath, BlogMonth> monthMap;
        private readonly Dictionary<UriPath, BlogEntry> entryMap;
        private readonly Dictionary<UriPath, BlogItem> itemMap;

        public IEnumerable<BlogYear> Years => years;
        public IEnumerable<BlogMonth> Months => months;
        public IEnumerable<BlogEntry> Entries => entries;

        public BlogContent(BlogFiles files, BlogCompiler compiler)
        {
            BlogItemComparer comparer = BlogItemComparer.Instance;

            years = new(compiler.Compile(files.GetFiles()), comparer);
            months = new(years.SelectMany(y => y), comparer);
            entries = new(months.SelectMany(m => m), comparer);

            LinkEntries();

            yearMap = years.ToDictionary(y => y.Link);
            monthMap = months.ToDictionary(m => m.Link);
            entryMap = entries.ToDictionary(e => e.Link);

            itemMap = years.Cast<BlogItem>().Concat(months).Concat(entries).ToDictionary(i => i.Link);
        }

        public IEnumerable<BlogEntry> Newest(int count = 1)
            => entries.Take(Math.Min(entries.Count, count));

        public BlogYear GetYear(UriPath link)
            => yearMap[link];

        public BlogMonth GetMonth(UriPath link)
            => monthMap[link];

        public BlogEntry GetEntry(UriPath link)
            => entryMap[link];

        public BlogItem GetItem(UriPath link)
            => itemMap[link];

        private void LinkEntries()
        {
            BlogEntry? next = null;

            foreach (BlogEntry entry in entries)
            {
                entry.Next = next;

                if (next is not null)
                {
                    next.Prev = entry;
                }

                next = entry;
            }
        }
    }
}
