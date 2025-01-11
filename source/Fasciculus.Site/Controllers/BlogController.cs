using Fasciculus.IO;
using Fasciculus.Site.Blog.Models;
using Fasciculus.Site.Rendering.Services;
using Markdig.Syntax;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Fasciculus.Site.Controllers
{
    public class BlogController : Controller
    {
        public class BlogFrontMatter
        {
            public string Title { get; set; } = string.Empty;
            public DateTime Published { get; set; } = DateTime.MinValue;
            public string Author { get; set; } = string.Empty;
        }

        private readonly Markup markup;

        public BlogController(Markup markup)
        {
            this.markup = markup;
        }

        [Route("/blog/")]
        public IActionResult Blog()
        {
            FileInfo file = SpecialDirectories.WorkingDirectory
                .Combine("Blog", "Documents", "2025", "01").File("010_Test.md");

            MarkdownDocument markdown = markup.Parse(file);
            BlogFrontMatter frontMatter = markup.FrontMatter<BlogFrontMatter>(markdown);
            string published = $"Published: {frontMatter.Published:yyyy-MM-dd}";
            string author = $"Author: {frontMatter.Author}";
            string content = markup.Render(markdown, published, author);

            BlogDocument document = new()
            {
                Title = frontMatter.Title,
                Content = content
            };

            return View(document);
        }
    }
}
