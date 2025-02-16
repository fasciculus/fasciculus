using Fasciculus.Site.Specifications.Models;
using Fasciculus.Site.Specifications.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Site.Controllers
{
    public class SpecificationsController : Controller
    {
        private readonly SpecificationContent content;

        public SpecificationsController(SpecificationContent content)
        {
            this.content = content;
        }

        [Route("/specifications/")]
        public IActionResult Index()
        {
            SpecificationIndexViewModel model = new()
            {
                Title = "Specifications",
                Packages = content.GetPackages(),
            };

            return View(model);
        }

        [Route("/specifications/{name}/")]
        public IActionResult Package(string name)
        {
            SpecificationPackage package = content.GetPackage(name);

            SpecificationPackageViewModel model = new()
            {
                Title = package.Name + " Specifications",
                Entries = [.. package.GetEntries()],
            };

            return View("Package", model);
        }

        [Route("/specifications/{package}/{id}.html")]
        public IActionResult Entry(string package, string id)
        {
            SpecificationEntry entry = content.GetPackage(package).GetEntry(id);

            SpecificationEntryViewModel model = new()
            {
                Title = entry.Title,
                Entry = entry,
                UseKaTeX = true,
            };

            return View("Entry", model);
        }
    }
}
