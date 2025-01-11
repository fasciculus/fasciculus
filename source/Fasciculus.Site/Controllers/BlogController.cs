using Fasciculus.IO;
using Fasciculus.Site.Blog.Models;
using Fasciculus.Site.Rendering.Services;
using Markdig.Syntax;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Fasciculus.Site.Controllers
{
    public class BlogController : Controller
    {
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
            string content = markup.Render(markdown, frontMatter);

            BlogViewModel model = new()
            {
                Title = frontMatter.Title,
                Content = content
            };

            return View(model);
        }
    }
}
