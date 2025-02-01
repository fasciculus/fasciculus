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
                Navigation = apiNavigation.Create(),
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
            ISymbol? symbol = apiContent.GetSymbol(path);

            if (symbol is not null)
            {
                return symbol.Kind switch
                {
                    SymbolKind.Package => Package((IPackageSymbol)symbol),
                    SymbolKind.Namespace => Namespace((INamespaceSymbol)symbol),
                    SymbolKind.Enum => Enum((IEnumSymbol)symbol),
                    SymbolKind.Interface => Interface((IInterfaceSymbol)symbol),
                    SymbolKind.Class => Class((IClassSymbol)symbol),
                    _ => NotFound()
                };
            }

            return NotFound();
        }

        private IActionResult Document(params string[] parts)
        {
            UriPath path = new(parts);
            string last = parts.Last();

            switch (last)
            {
                case "-Constructors": return Constructors(path);
            }

            ISymbol? symbol = apiContent.GetSymbol(path);

            if (symbol is not null)
            {
                return symbol.Kind switch
                {
                    SymbolKind.Field => Field((IFieldSymbol)symbol),
                    SymbolKind.Member => Member((IMemberSymbol)symbol),
                    SymbolKind.Event => Event((IEventSymbol)symbol),
                    SymbolKind.Property => Property((IPropertySymbol)symbol),
                    _ => NotFound()
                };
            }

            return NotFound();
        }

        private ViewResult Package(IPackageSymbol package)
        {
            ApiPackageViewModel model = new()
            {
                Title = package.Name + " Package",
                Package = package,
                PackageUri = package.Repository,
                AppliesTo = new(package.Frameworks),
                Navigation = apiNavigation.Create(package.Link)
            };

            return View("Package", model);
        }

        private ViewResult Namespace(INamespaceSymbol @namespace)
        {
            ApiNamespaceViewModel model = new()
            {
                Title = @namespace.Name + " Namespace",
                Namespace = @namespace,
                AppliesTo = new(@namespace.Frameworks),
                Navigation = apiNavigation.Create(@namespace.Link)
            };

            return View("Namespace", model);
        }

        private ViewResult Enum(IEnumSymbol @enum)
        {
            ApiEnumViewModel model = new()
            {
                Title = @enum.Name + " Enum",
                Enum = @enum,
                Symbol = @enum,
                SourceUris = [.. GetSourceUris(@enum)],
                AppliesTo = new(@enum.Frameworks),
                Navigation = apiNavigation.Create(@enum.Link)
            };

            return View("Enum", model);
        }

        private ViewResult Interface(IInterfaceSymbol @interface)
        {
            ApiInterfaceViewModel model = new()
            {
                Title = @interface.Name + " Interface",
                Interface = @interface,
                Symbol = @interface,
                SourceUris = [.. GetSourceUris(@interface)],
                AppliesTo = new(@interface.Frameworks),
                Navigation = apiNavigation.Create(@interface.Link)
            };

            return View("Interface", model);
        }

        private ViewResult Class(IClassSymbol @class)
        {
            ApiClassViewModel model = new()
            {
                Title = @class.Name + " Class",
                Class = @class,
                Symbol = @class,
                SourceUris = [.. GetSourceUris(@class)],
                AppliesTo = new(@class.Frameworks),
                Navigation = apiNavigation.Create(@class.Link)
            };

            return View("Class", model);
        }

        private ViewResult Field(IFieldSymbol field)
        {
            ApiFieldViewModel model = new()
            {
                Title = field.Name + " Field",
                Field = field,
                Symbol = field,
                SourceUris = [.. GetSourceUris(field)],
                AppliesTo = new(field.Frameworks),
                Navigation = apiNavigation.Create(field.Link)
            };

            return View("Field", model);
        }

        private ViewResult Member(IMemberSymbol member)
        {
            ApiMemberViewModel model = new()
            {
                Title = member.Name + " Member",
                Member = member,
                Symbol = member,
                SourceUris = [.. GetSourceUris(member)],
                AppliesTo = new(member.Frameworks),
                Navigation = apiNavigation.Create(member.Link)
            };

            return View("Member", model);
        }

        private ViewResult Event(IEventSymbol @event)
        {
            ApiEventViewModel model = new()
            {
                Title = @event.Name + " Event",
                Event = @event,
                Symbol = @event,
                SourceUris = [.. GetSourceUris(@event)],
                AppliesTo = new(@event.Frameworks),
                Navigation = apiNavigation.Create(@event.Link)
            };

            return View("Event", model);
        }

        private ViewResult Property(IPropertySymbol property)
        {
            ApiPropertyViewModel model = new()
            {
                Title = property.Name + " Property",
                Property = property,
                Symbol = property,
                SourceUris = [.. GetSourceUris(property)],
                AppliesTo = new(property.Frameworks),
                Navigation = apiNavigation.Create(property.Link)
            };

            return View("Property", model);
        }

        private IActionResult Constructors(UriPath path)
        {
            IClassSymbol? @class = apiContent.GetSymbol(path.Parent) as IClassSymbol;

            if (@class is not null)
            {
                ApiConstructorsViewModel model = new()
                {
                    Title = $"{@class.Name} Constructors",
                    Constructors = @class.Constructors,
                    Navigation = apiNavigation.Create(path)
                };

                return View("Constructors", model);
            }

            return NotFound();

        }

        private IEnumerable<Uri> GetSourceUris<T>(T symbol)
            where T : notnull, ISourceSymbol
        {
            if (symbol.Packages.Count() > 0)
            {
                UriPath packageLink = new(symbol.Packages.First());
                IPackageSymbol? package = apiContent.GetSymbol(packageLink) as IPackageSymbol;

                if (package is not null)
                {
                    foreach (UriPath source in symbol.Sources)
                    {
                        string uri = $"{package.Repository}/{source}";

                        yield return new(uri);
                    }
                }
            }
        }
    }
}
