using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.GitHub.Controllers
{
    public class MainController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
