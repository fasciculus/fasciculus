using Fasciculus.ApiDoc.Models;
using Fasciculus.GitHub.Models;
using Fasciculus.GitHub.Services;
using Microsoft.AspNetCore.Mvc;

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

        [Route("/api/{packageName}/")]
        public IActionResult Package(string packageName)
        {
            ApiLink link = new(packageName);
            ApiPackage package = apiProvider.GetPackage(link);

            ApiPackageDocument document = new()
            {
                Title = "Package " + package.Name,
                Package = package
            };

            return View(document);
        }

        [Route("/api/{packageName}/{namespaceName}/")]
        public IActionResult Namespace(string packageName, string namespaceName)
        {
            ApiLink link = new(packageName, namespaceName);
            ApiNamespace @namespace = apiProvider.GetNamespace(link);

            ApiNamespaceDocument document = new()
            {
                Title = "Namespace " + @namespace.Name,
                Namespace = @namespace,
                Classes = @namespace.Classes
            };

            return View(document);
        }

        [Route("/api/{packageName}/{namespaceName}/{className}/")]
        public IActionResult Class(string packageName, string namespaceName, string className)
        {
            ApiLink link = new(packageName, namespaceName, className);
            ApiClass @class = apiProvider.GetClass(link);

            ApiClassDocument document = new()
            {
                Title = "Class: " + @class.Name,
                Class = @class,
            };

            return View(document);
        }
    }
}
