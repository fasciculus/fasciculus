using Fasciculus.IO;
using Fasciculus.Site.Blog.Models;
using Fasciculus.Site.Services;
using Markdig.Syntax;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Fasciculus.Site.Controllers
{
    public class BlogController : Controller
    {
        public class BlogFrontMatter
        {
            public string Title { get; set; } = string.Empty;
        }

        private readonly Yaml yaml;
        private readonly Markup markup;

        public BlogController(Yaml yaml, Markup markup)
        {
            this.yaml = yaml;
            this.markup = markup;
        }

        [Route("/blog/")]
        public IActionResult Blog()
        {
            FileInfo file = SpecialDirectories.WorkingDirectory
                .Combine("Blog", "Documents", "2025", "01").File("010_Test.md");

            MarkdownDocument markdown = markup.Parse(file);
            BlogFrontMatter frontMatter = markup.FrontMatter<BlogFrontMatter>(markdown);
            string content = markup.Render(markdown);

            BlogDocument document = new()
            {
                Title = frontMatter.Title,
                Content = content
            };

            return View(document);
        }
    }
}
