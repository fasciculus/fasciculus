using Fasciculus.Collections;
using Fasciculus.Net.Navigating;
using Fasciculus.Site.Blog.Models;
using Fasciculus.Site.Rendering.Services;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Blog.Compilers
{
    public class BlogCompiler
    {
        private readonly Markup markup;

        public BlogCompiler(Markup markup)
        {
            this.markup = markup;
        }

        public BlogYears Compile(IEnumerable<FileInfo> files)
        {
            BlogYears years = [];

            files.Select(Compile).NotNull().Apply(years.Add);

            return years;
        }

        private BlogEntry? Compile(FileInfo file)
        {
            MarkdownDocument markdown = markup.Parse(file);
            BlogFrontMatter frontMatter = markup.FrontMatter<BlogFrontMatter>(markdown);
            DateTime published = frontMatter.Published;

            if (published == DateTime.MinValue)
            {
                return null;
            }

            UriPath link = CreateLink(published, file);
            string title = frontMatter.Title;
            string summary = frontMatter.Summary;
            string content = markup.Render(markdown, frontMatter);

            return new(link, title, published, summary, content);
        }

        private static UriPath CreateLink(DateTime published, FileInfo file)
        {
            string[] parts =
            [
                "blog",
                published.Year.ToString(),
                published.Month.ToString("00"),
                Path.GetFileNameWithoutExtension(file.FullName) + ".html"
            ];

            return new(parts);
        }
    }
}
