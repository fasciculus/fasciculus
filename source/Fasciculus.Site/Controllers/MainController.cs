using Fasciculus.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Site.Controllers
{
    public class MainController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            ViewModel model = new()
            {
                Title = "Home"
            };

            return View(model);
        }

        [Route("/privacy.html")]
        public IActionResult Privacy()
        {
            ViewModel model = new()
            {
                Title = "Privacy"
            };

            return View(model);
        }

        [Route("/releases.html")]
        public IActionResult Releases()
        {
            ViewModel model = new()
            {
                Title = "Releases"
            };

            return View(model);
        }

        [Route("/about.html")]
        public IActionResult About()
        {
            ViewModel model = new()
            {
                Title = "About"
            };

            return View(model);
        }
    }
}
