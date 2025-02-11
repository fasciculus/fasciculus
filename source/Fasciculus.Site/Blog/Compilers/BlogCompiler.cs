using Fasciculus.Collections;
using Fasciculus.IO;
using Fasciculus.Markdown.Yaml;
using Fasciculus.Net.Navigating;
using Fasciculus.Site.Blog.Models;
using Markdig;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Blog.Compilers
{
    public class BlogCompiler
    {
        private readonly MarkdownPipeline pipeline;

        public BlogCompiler(MarkdownPipeline pipeline)
        {
            this.pipeline = pipeline;
        }

        public BlogYears Compile(IEnumerable<FileInfo> files)
        {
            BlogYears years = [];

            files.Select(Compile).NotNull().Apply(years.Add);

            return years;
        }

        private BlogEntry? Compile(FileInfo file)
        {
            MarkdownDocument document = Markdig.Markdown.Parse(file.ReadAllText(), pipeline);
            BlogFrontMatter frontMatter = document.FrontMatterObject<BlogFrontMatter>();
            DateTime published = frontMatter.Published;

            if (published == DateTime.MinValue)
            {
                return null;
            }

            UriPath link = CreateLink(published, file);
            string title = frontMatter.Title;
            string summary = frontMatter.Summary;
            string html = document.ToHtml(pipeline);

            return new(link, title, published, summary, html);
        }

        private static UriPath CreateLink(DateTime published, FileInfo file)
        {
            string[] parts =
            [
                "blog",
                published.Year.ToString(),
                published.Month.ToString("00"),
                Path.GetFileNameWithoutExtension(file.FullName)
            ];

            return new(parts);
        }
    }
}
