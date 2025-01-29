using Fasciculus.Site.Models;
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
            ViewModel model = new()
            {
                Title = "Specifications"
            };

            return View(model);
        }

        [Route("/specifications/{id}.html")]
        public IActionResult Entry(string id)
        {
            SpecificationEntry entry = content.GetEntry(id);

            SpecificationViewModel model = new()
            {
                Title = entry.Title,
                Entry = entry,
                UseKaTeX = true,
            };

            return View(model);
        }
    }
}
