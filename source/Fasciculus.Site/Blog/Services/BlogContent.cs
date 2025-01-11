using Fasciculus.Net.Navigating;
using Fasciculus.Site.Blog.Compilers;
using Fasciculus.Site.Blog.Models;
using Fasciculus.Site.Blog.Support;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Blog.Services
{
    public class BlogContent : IEnumerable<BlogEntry>
    {
        private readonly Dictionary<UriPath, BlogEntry> entries;
        private readonly SortedSet<BlogEntry> sorted;

        public BlogEntry this[UriPath link] => entries[link];

        public BlogContent(BlogDocuments documents, BlogCompiler compiler)
        {
            entries = documents.Select(compiler.Compile).ToDictionary(e => e.Link);
            sorted = new(entries.Values, BlogEntryComparer.Instance);
        }

        public BlogEntry GetEntry(UriPath link)
            => entries[link];

        public IEnumerable<BlogEntry> Newest(int count = 1)
            => sorted.Take(Math.Min(sorted.Count, count));

        public IEnumerator<BlogEntry> GetEnumerator()
            => sorted.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => sorted.GetEnumerator();
    }
}
