using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Fasciculus.Site.Api.Models;
using Fasciculus.Site.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Controllers
{
    public class ApiController : Controller
    {
        private static readonly string RepositoryPrefix = "https://github.com/fasciculus/fasciculus";

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
            ApiPackagesViewModel model = new()
            {
                Title = "API Doc",
                Combined = content.Combined,
                Packages = content.Packages
            };

            return View(model);
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
                    SymbolKind.Enum => Enum((EnumSymbol)symbol),
                    _ => NotFound()
                };
            }

            return NotFound();
        }

        private ViewResult Package(PackageSymbol package)
        {
            ApiPackageViewModel model = new()
            {
                Title = package.Name + " Package",
                Package = package,
                PackageUri = new($"{RepositoryPrefix}/{package.RepositoryDirectory}/"),
                Navigation = navigation.Create(package.Link)
            };

            return View("Package", model);
        }

        private ViewResult Namespace(NamespaceSymbol @namespace)
        {
            ApiNamespaceViewModel model = new()
            {
                Title = @namespace.Name + " Namespace",
                Namespace = @namespace,
                Navigation = navigation.Create(@namespace.Link)
            };

            return View("Namespace", model);
        }

        private ViewResult Class(ClassSymbol @class)
        {
            ApiClassViewModel model = new()
            {
                Title = @class.Name + " Class",
                Class = @class,
                Symbol = @class,
                SourceUris = [.. GetSourceUris(@class)],
                Navigation = navigation.Create(@class.Link)
            };

            return View("Class", model);
        }

        private ViewResult Enum(EnumSymbol @enum)
        {
            ApiEnumViewModel model = new()
            {
                Title = @enum.Name + " Enum",
                Enum = @enum,
                Symbol = @enum,
                SourceUris = [.. GetSourceUris(@enum)],
                Navigation = navigation.Create(@enum.Link)
            };

            return View("Enum", model);
        }

        private IEnumerable<Uri> GetSourceUris<T>(T type)
            where T : notnull, TypeSymbol<T>
        {
            if (type.Packages.Count() > 0)
            {
                UriPath packageLink = new(type.Packages.First());
                PackageSymbol? package = content.GetSymbol(packageLink) as PackageSymbol;

                if (package is not null)
                {
                    foreach (UriPath source in type.Sources)
                    {
                        string uri = $"{RepositoryPrefix}/{package.RepositoryDirectory}/{source}";

                        yield return new(uri);
                    }
                }
            }
        }
    }
}
