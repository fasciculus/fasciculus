using Fasciculus.Site.Licenses.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Site.Controllers
{
    public class LicensesController : Controller
    {
        [Route("/licences/")]
        public IActionResult Index()
        {
            LicencesViewModel model = new()
            {
                Title = "Licenses",
            };

            return View("Index", model);
        }
    }
}
