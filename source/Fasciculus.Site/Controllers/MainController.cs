using Fasciculus.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Site.Controllers
{
    public class MainController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            SiteDocument document = new()
            {
                Title = "Home"
            };

            return View(document);
        }

        [Route("/privacy.html")]
        public IActionResult Privacy()
        {
            SiteDocument document = new()
            {
                Title = "Privacy"
            };

            return View(document);
        }

        [Route("/about.html")]
        public IActionResult About()
        {
            SiteDocument document = new()
            {
                Title = "About"
            };

            return View(document);
        }
    }
}
