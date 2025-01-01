using Fasciculus.GitHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.GitHub.Controllers
{
    public class MainController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            Document document = new()
            {
                Title = "Home"
            };

            return View(document);
        }

        [Route("/privacy.html")]
        public IActionResult Privacy()
        {
            Document document = new()
            {
                Title = "Privacy"
            };

            return View(document);
        }
    }
}
