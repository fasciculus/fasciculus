using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Fasciculus.Site.Api.Models;
using Fasciculus.Site.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Site.Controllers
{
    public class ApiController : Controller
    {
        private readonly ApiContent content;
        private readonly ApiNavigation navigation;

        public ApiController(ApiContent content, ApiNavigation navigation)
        {
            this.content = content;
            this.navigation = navigation;
        }

        [Route("/api/")]
        public IActionResult Packages()
        {
            ApiIndexDocument document = new()
            {
                Title = "API Doc",
                Packages = content.Packages
            };

            return View(document);
        }

        [Route("/api/{p1}/")]
        public IActionResult Container1(string p1)
            => Container(p1);

        [Route("/api/{p1}/{p2}/")]
        public IActionResult Container2(string p1, string p2)
            => Container(p1, p2);

        [Route("/api/{p1}/{p2}/{p3}/")]
        public IActionResult Container2(string p1, string p2, string p3)
            => Container(p1, p2, p3);

        private IActionResult Container(params string[] parts)
        {
            UriPath path = new(parts);
            Symbol? symbol = content.GetSymbol(path);

            if (symbol is not null)
            {
                return symbol.Kind switch
                {
                    SymbolKind.Package => Package((PackageSymbol)symbol),
                    SymbolKind.Namespace => Namespace((NamespaceSymbol)symbol),
                    SymbolKind.Class => Class((ClassSymbol)symbol),
                    _ => NotFound()
                };
            }

            return NotFound();
        }

        private ViewResult Package(PackageSymbol package)
        {
            ApiPackageDocument document = new()
            {
                Title = package.Name + " Package",
                Package = package,
                Navigation = navigation.Create(package.Link)
            };

            return View("Package", document);
        }

        private ViewResult Namespace(NamespaceSymbol @namespace)
        {
            ApiNamespaceDocument document = new()
            {
                Title = @namespace.Name + " Namespace",
                Namespace = @namespace,
                Navigation = navigation.Create(@namespace.Link)
            };

            return View("Namespace", document);
        }

        private ViewResult Class(ClassSymbol @class)
        {
            ApiClassViewModel model = new()
            {
                Title = @class.Name + " Class",
                Class = @class,
                Navigation = navigation.Create(@class.Link)
            };

            return View("Class", model);
        }
    }
}
