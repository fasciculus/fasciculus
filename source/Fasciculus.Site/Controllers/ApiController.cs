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

        private readonly ApiContent apiContent;
        private readonly ApiNavigation apiNavigation;

        public ApiController(ApiContent apiContent, ApiNavigation apiNavigation)
        {
            this.apiContent = apiContent;
            this.apiNavigation = apiNavigation;
        }

        [Route("/api/")]
        public IActionResult Packages()
        {
            ApiPackagesViewModel model = new()
            {
                Title = "API Doc",
                Combined = apiContent.Combined,
                Packages = apiContent.Packages
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
        public IActionResult Container3(string p1, string p2, string p3)
            => Container(p1, p2, p3);

        [Route("/api/{p1}/{p2}/{p3}/{p4}")]
        public IActionResult Container4(string p1, string p2, string p3, string p4)
            => Container(p1, p2, p3, p4);

        [Route("/api/{p1}.html")]
        public IActionResult Document1(string p1)
            => Document(p1);

        [Route("/api/{p1}/{p2}.html")]
        public IActionResult Document2(string p1, string p2)
            => Document(p1, p2);

        [Route("/api/{p1}/{p2}/{p3}.html")]
        public IActionResult Document3(string p1, string p2, string p3)
            => Document(p1, p2, p3);

        [Route("/api/{p1}/{p2}/{p3}/{p4}.html")]
        public IActionResult Document4(string p1, string p2, string p3, string p4)
            => Document(p1, p2, p3, p4);

        [Route("/api/{p1}/{p2}/{p3}/{p4}/{p5}.html")]
        public IActionResult Document5(string p1, string p2, string p3, string p4, string p5)
            => Document(p1, p2, p3, p4, p5);

        private IActionResult Container(params string[] parts)
        {
            UriPath path = new(parts);
            Symbol? symbol = apiContent.GetSymbol(path);

            if (symbol is not null)
            {
                return symbol.Kind switch
                {
                    SymbolKind.Package => Package((PackageSymbol)symbol),
                    SymbolKind.Namespace => Namespace((NamespaceSymbol)symbol),
                    SymbolKind.Enum => Enum((EnumSymbol)symbol),
                    SymbolKind.Interface => Interface((InterfaceSymbol)symbol),
                    SymbolKind.Class => Class((ClassSymbol)symbol),
                    _ => NotFound()
                };
            }

            return NotFound();
        }

        private IActionResult Document(params string[] parts)
        {
            UriPath path = new(parts);
            Symbol? symbol = apiContent.GetSymbol(path);

            if (symbol is not null)
            {
                return symbol.Kind switch
                {
                    SymbolKind.Field => Field((FieldSymbol)symbol),
                    SymbolKind.Member => Member((MemberSymbol)symbol),
                    SymbolKind.Event => Event((EventSymbol)symbol),
                    SymbolKind.Property => Property((PropertySymbol)symbol),
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
                Navigation = apiNavigation.Create(package.Link)
            };

            return View("Package", model);
        }

        private ViewResult Namespace(NamespaceSymbol @namespace)
        {
            ApiNamespaceViewModel model = new()
            {
                Title = @namespace.Name + " Namespace",
                Namespace = @namespace,
                Navigation = apiNavigation.Create(@namespace.Link)
            };

            return View("Namespace", model);
        }

        private ViewResult Enum(EnumSymbol @enum)
        {
            ApiEnumViewModel model = new()
            {
                Title = @enum.Name + " Enum",
                Enum = @enum,
                Symbol = @enum,
                SourceUris = [.. GetSourceUris(@enum)],
                Navigation = apiNavigation.Create(@enum.Link)
            };

            return View("Enum", model);
        }

        private ViewResult Interface(InterfaceSymbol @interface)
        {
            ApiInterfaceViewModel model = new()
            {
                Title = @interface.Name + " Interface",
                Interface = @interface,
                Symbol = @interface,
                SourceUris = [.. GetSourceUris(@interface)],
                Navigation = apiNavigation.Create(@interface.Link)
            };

            return View("Interface", model);
        }

        private ViewResult Class(ClassSymbol @class)
        {
            ApiClassViewModel model = new()
            {
                Title = @class.Name + " Class",
                Class = @class,
                Symbol = @class,
                SourceUris = [.. GetSourceUris(@class)],
                Navigation = apiNavigation.Create(@class.Link)
            };

            return View("Class", model);
        }

        private ViewResult Field(FieldSymbol field)
        {
            ApiFieldViewModel model = new()
            {
                Title = field.Name + " Field",
                Field = field,
                Symbol = field,
                SourceUris = [.. GetSourceUris(field)],
                Navigation = apiNavigation.Create(field.Link)
            };

            return View("Field", model);
        }

        private ViewResult Member(MemberSymbol member)
        {
            ApiMemberViewModel model = new()
            {
                Title = member.Name + " Member",
                Member = member,
                Symbol = member,
                SourceUris = [.. GetSourceUris(member)],
                Navigation = apiNavigation.Create(member.Link)
            };

            return View("Member", model);
        }

        private ViewResult Event(EventSymbol @event)
        {
            ApiEventViewModel model = new()
            {
                Title = @event.Name + " Event",
                Event = @event,
                Symbol = @event,
                SourceUris = [.. GetSourceUris(@event)],
                Navigation = apiNavigation.Create(@event.Link)
            };

            return View("Event", model);
        }

        private ViewResult Property(PropertySymbol property)
        {
            ApiPropertyViewModel model = new()
            {
                Title = property.Name + " Property",
                Property = property,
                Symbol = property,
                SourceUris = [.. GetSourceUris(property)],
                Navigation = apiNavigation.Create(property.Link)
            };

            return View("Property", model);
        }

        private IEnumerable<Uri> GetSourceUris<T>(T symbol)
            where T : notnull, SourceSymbol<T>
        {
            if (symbol.Packages.Count() > 0)
            {
                UriPath packageLink = new(symbol.Packages.First());
                PackageSymbol? package = apiContent.GetSymbol(packageLink) as PackageSymbol;

                if (package is not null)
                {
                    foreach (UriPath source in symbol.Sources)
                    {
                        string uri = $"{RepositoryPrefix}/{package.RepositoryDirectory}/{source}";

                        yield return new(uri);
                    }
                }
            }
        }
    }
}
