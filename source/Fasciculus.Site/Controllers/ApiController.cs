using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net;
using Fasciculus.Site.Models;
using Fasciculus.Site.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Site.Controllers
{
    public class ApiController : Controller
    {
        private readonly ApiProvider apiProvider;

        public ApiController(ApiProvider apiProvider)
        {
            this.apiProvider = apiProvider;
        }

        [Route("/api/")]
        public IActionResult Packages()
        {
            ApiIndexDocument document = new()
            {
                Title = "API Doc",
                Packages = apiProvider.Packages
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
            Symbol? symbol = apiProvider.GetSymbol(path);

            if (symbol is not null)
            {
                switch (symbol.Kind)
                {
                    case SymbolKind.Package: return symbol is PackageSymbol package ? Package(package) : ServerError();
                    case SymbolKind.Namespace: return symbol is NamespaceSymbol @namespace ? Namespace(@namespace) : ServerError();
                    case SymbolKind.Class: return symbol is ClassSymbol @class ? Class(@class) : ServerError();
                }
            }

            return NotFound();
        }

        private ViewResult Package(PackageSymbol package)
        {
            ApiPackageDocument document = new()
            {
                Title = package.Name + " Package",
                Package = package
            };

            return View("Package", document);
        }

        private ViewResult Namespace(NamespaceSymbol @namespace)
        {
            ApiNamespaceDocument document = new()
            {
                Title = @namespace.Name + " Namespace",
                Namespace = @namespace
            };

            return View("Namespace", document);
        }

        private ViewResult Class(ClassSymbol @class)
        {
            ApiClassDocument document = new()
            {
                Title = @class.Name + " Namespace",
                Class = @class
            };

            return View("Class", document);
        }

        private StatusCodeResult ServerError()
            => StatusCode(StatusCodes.Status500InternalServerError);
    }
}
