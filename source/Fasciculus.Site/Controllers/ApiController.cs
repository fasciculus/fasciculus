using Fasciculus.ApiDoc.Models;
using Fasciculus.GitHub.Models;
using Fasciculus.GitHub.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Fasciculus.GitHub.Controllers
{
    public class ApiController : Controller
    {
        private readonly ApiProvider apiProvider;

        public ApiController(ApiProvider apiProvider)
        {
            this.apiProvider = apiProvider;
        }

        [Route("/api/")]
        public IActionResult Index()
        {
            ApiIndexDocument document = new()
            {
                Title = "API Doc",
                Packages = apiProvider.Packages
            };

            return View(document);
        }

        [Route("/api/{pkg}/")]
        public IActionResult Package(string pkg)
        {
            ApiPackage package = apiProvider.Packages.First(package => package.Name == pkg);

            ApiPackageDocument document = new()
            {
                Title = "Package " + package.Name,
                Package = package
            };

            return View(document);
        }
    }
}
