using Fasciculus.Site.Licenses.Models;
using Fasciculus.Site.Licenses.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Site.Controllers
{
    public class LicensesController : Controller
    {
        private readonly LicensesContent content;

        public LicensesController(LicensesContent content)
        {
            this.content = content;
        }

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
