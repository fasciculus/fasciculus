using Fasciculus.Net.Navigating;
using Fasciculus.Site.Specifications.Models;
using Fasciculus.Site.Specifications.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Site.Controllers
{
    public class SpecificationsController : Controller
    {
        private readonly SpecificationContent content;
        private readonly SpecificationNavigation navigation;

        public SpecificationsController(SpecificationContent content, SpecificationNavigation navigation)
        {
            this.content = content;
            this.navigation = navigation;
        }

        [Route("/specifications/")]
        public IActionResult Index()
        {
            SpecificationIndexViewModel model = new()
            {
                Title = "Specifications",
                Navigation = navigation.Create(UriPath.Empty),
                Packages = content.GetPackages(),
            };

            return View(model);
        }

        [Route("/specifications/{name}/")]
        public IActionResult Package(string name)
        {
            SpecificationPackage package = content.GetPackage(name);
            UriPath link = new("specifications", name);

            SpecificationPackageViewModel model = new()
            {
                Title = package.Name + " Specifications",
                Navigation = navigation.Create(link),
                Entries = [.. package.GetEntries()],
            };

            return View("Package", model);
        }

        [Route("/specifications/{package}/{id}.html")]
        public IActionResult Entry(string package, string id)
        {
            SpecificationEntry entry = content.GetPackage(package).GetEntry(id);
            UriPath link = new("specifications", package, id);

            SpecificationEntryViewModel model = new()
            {
                Title = entry.Title,
                Navigation = navigation.Create(link),
                Entry = entry,
                UseKaTeX = true,
            };

            return View("Entry", model);
        }
    }
}
