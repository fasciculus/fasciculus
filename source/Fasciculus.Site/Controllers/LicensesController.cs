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

        [Route("/licenses/")]
        public IActionResult Index()
        {
            LicencesViewModel model = new()
            {
                Title = "Licenses",
                LicenseLists = content.GetLicenseLists()
            };

            return View("Index", model);
        }

        [Route("/licenses/{packageName}.html")]
        public IActionResult PackageLicenses(string packageName)
        {
            PackageLicensesViewModel model = new()
            {
                Title = packageName,
                Licenses = content[packageName]
            };

            return View("PackageLicenses", model);
        }
    }
}
