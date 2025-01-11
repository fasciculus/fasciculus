using Fasciculus.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Site.Controllers
{
    public class MainController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            ViewModel document = new()
            {
                Title = "Home"
            };

            return View(document);
        }

        [Route("/privacy.html")]
        public IActionResult Privacy()
        {
            ViewModel document = new()
            {
                Title = "Privacy"
            };

            return View(document);
        }

        [Route("/about.html")]
        public IActionResult About()
        {
            ViewModel document = new()
            {
                Title = "About"
            };

            return View(document);
        }
    }
}
