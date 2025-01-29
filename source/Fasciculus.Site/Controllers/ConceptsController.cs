using Fasciculus.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Site.Controllers
{
    public class ConceptsController : Controller
    {
        [Route("/concepts/")]
        public IActionResult Index()
        {
            ViewModel model = new()
            {
                Title = "Concepts",
            };

            return View(model);
        }
    }
}
