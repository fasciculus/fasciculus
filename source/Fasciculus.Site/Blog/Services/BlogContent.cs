using Fasciculus.Net.Navigating;
using Fasciculus.Site.Blog.Compilers;
using Fasciculus.Site.Blog.Models;
using Fasciculus.Site.Rendering.Services;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Blog.Services
{
    public class BlogContent
    {
        private readonly Dictionary<UriPath, BlogEntry> entries;

        public BlogEntry this[UriPath link] => entries[link];

        public BlogContent(BlogDocuments documents, Markup markup)
        {
            entries = Compile(documents, markup).ToDictionary(e => e.Link);
        }

        public BlogEntry GetEntry(UriPath link)
            => entries[link];

        private static IEnumerable<BlogEntry> Compile(BlogDocuments documents, Markup markup)
        {
            BlogCompiler compiler = new(documents.Directory, markup);

            return documents.Select(compiler.Compile);
        }
    }
}
