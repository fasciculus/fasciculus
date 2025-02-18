using Fasciculus.Net.Navigating;
using Fasciculus.Site.Releases.Models;
using Fasciculus.Site.Releases.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Site.Controllers
{
    public class ReleasesController : Controller
    {
        private readonly ReleasesContent releases;
        private readonly ReleasesNavigation navigation;

        public ReleasesController(ReleasesContent releases, ReleasesNavigation navigation)
        {
            this.releases = releases;
            this.navigation = navigation;
        }

        [Route("/releases/")]
        public IActionResult Index()
        {
            ReleasesIndexViewModel model = new()
            {
                Title = "Releases",
                Navigation = navigation.Create(UriPath.Empty),
            };

            return View("Index", model);
        }

        [Route("/releases/Roadmap.html")]
        public IActionResult Roadmap()
        {
            Roadmap roadmap = releases.GetRoadmap();

            RoadmapViewModel model = new()
            {
                Title = "Roadmap",
                Navigation = navigation.Create(UriPath.Empty),
                Html = roadmap.Html
            };

            return View("Roadmap", model);
        }
    }
}
