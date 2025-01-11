using Fasciculus.Site.Blog.Models;
using Fasciculus.Site.Blog.Services;
using Fasciculus.Site.Rendering.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Fasciculus.Site.Controllers
{
    public class BlogController : Controller
    {
        private readonly Markup markup;
        private readonly BlogContent content;

        public BlogController(Markup markup, BlogContent content)
        {
            this.markup = markup;
            this.content = content;
        }

        [Route("/blog/")]
        public IActionResult Blog()
        {
            BlogEntry entry = content.Newest().First();

            BlogViewModel model = new()
            {
                Title = entry.Title,
                Content = entry.Content,
            };

            return View(model);
        }
    }
}
