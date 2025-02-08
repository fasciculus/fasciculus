using Fasciculus.Blog.Preview.Models;
using Fasciculus.Blog.Preview.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Blog.Preview.Controllers
{
    public class BlogController : Controller
    {
        private readonly Entries entries;

        public BlogController(Entries entries)
        {
            this.entries = entries;
        }

        [Route("/")]
        public IActionResult Index()
        {
            IndexViewModel model = new()
            {
                Title = "Index",
                Version = entries.Version,
                Keys = [.. entries.GetKeys()]
            };

            return View("Index", model);
        }

        [Route("/entry/{key}")]
        public IActionResult Entry(string key)
        {
            Entry entry = entries.GetEntry(key);

            EntryViewModel model = new()
            {
                Title = entry.Title,
                Version = entries.Version,
                Entry = entry,
            };

            return View("Entry", model);
        }
    }
}
