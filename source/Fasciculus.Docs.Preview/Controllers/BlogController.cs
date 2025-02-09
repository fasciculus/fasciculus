using Fasciculus.Docs.Preview.Models;
using Fasciculus.Docs.Preview.Services;
using Fasciculus.IO;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Docs.Preview.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogClient blog;
        private readonly Renderer renderer;

        public BlogController(BlogClient blog, Renderer renderer)
        {
            this.blog = blog;
            this.renderer = renderer;
        }

        [Route("/Blog/")]
        public IActionResult Index()
        {
            BlogIndexViewModel model = new()
            {
                Title = "Blog",
                Version = blog.GetVersion(),
                Keys = blog.GetKeys(),
            };

            return View("Index", model);
        }

        [Route("/Blog/{key}")]
        public IActionResult Entry(string key)
        {
            string text = blog.GetFile(key).ReadAllText();

            BlogEntryViewModel model = new()
            {
                Title = key,
                Version = blog.GetVersion(),
                Content = renderer.Render(text)
            };

            return View("Entry", model);
        }
    }
}
