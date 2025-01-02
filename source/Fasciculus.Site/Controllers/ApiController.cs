using Fasciculus.GitHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.GitHub.Controllers
{
    public class ApiController : Controller
    {
        [Route("/api/")]
        public IActionResult Index()
        {
            Document document = new()
            {
                Title = "API Doc"
            };

            return View(document);
        }
    }
}
