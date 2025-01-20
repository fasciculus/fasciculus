using Fasciculus.Net.Navigating;
using Fasciculus.Site.Blog.Models;
using Fasciculus.Site.Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Site.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogContent content;
        private readonly BlogNavigation navigation;

        public BlogController(BlogContent content, BlogNavigation navigation)
        {
            this.content = content;
            this.navigation = navigation;
        }

        [Route("/blog/")]
        public IActionResult Blog()
        {
            BlogViewModel model = new()
            {
                Title = "Blog",
                Navigation = navigation.Create(UriPath.Empty),
                Entries = new(content.Newest(10)),
            };

            return View(model);
        }

        [Route("/blog/{y}/")]
        public IActionResult Year(string y)
        {
            UriPath link = new("blog", y);
            BlogYear year = content.GetYear(link);

            BlogYearViewModel model = new()
            {
                Title = year.Title,
                Navigation = navigation.Create(link),
                Year = year
            };

            return View(model);
        }

        [Route("/blog/{y}/{m}/")]
        public IActionResult Month(string y, string m)
        {
            UriPath link = new("blog", y, m);
            BlogMonth month = content.GetMonth(link);

            BlogMonthViewModel model = new()
            {
                Title = month.Title,
                Navigation = navigation.Create(link),
                Month = month
            };

            return View(model);
        }

        [Route("/blog/{y}/{m}/{e}.html")]
        public IActionResult Entry(string y, string m, string e)
        {
            UriPath link = new("blog", y, m, e);
            BlogEntry entry = content.GetEntry(link);

            BlogEntryViewModel model = new()
            {
                Title = entry.Title,
                Navigation = navigation.Create(link),
                Entry = entry
            };

            return View(model);
        }
    }
}
