using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using Fasciculus.Site.Blog.Models;
using Fasciculus.Site.Blog.Services;
using Fasciculus.Site.Rendering.Services;
using Markdig.Syntax;
using System;
using System.IO;

namespace Fasciculus.Site.Blog.Compilers
{
    public class BlogCompiler
    {
        private readonly DirectoryInfo directory;
        private readonly Markup markup;

        public BlogCompiler(BlogDocuments docoments, Markup markup)
        {
            directory = docoments.Directory;
            this.markup = markup;
        }

        public BlogEntry Compile(FileInfo file)
        {
            MarkdownDocument markdown = markup.Parse(file);
            BlogFrontMatter frontMatter = markup.FrontMatter<BlogFrontMatter>(markdown);
            UriPath link = file.RelativeTo(directory, ["blog"]);
            string title = frontMatter.Title;
            DateTime published = frontMatter.Published;
            string content = markup.Render(markdown, frontMatter);

            return new(link, title, published, content);
        }
    }
}
