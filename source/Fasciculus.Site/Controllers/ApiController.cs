using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Fasciculus.Site.Api.Models;
using Fasciculus.Site.Api.Services;
using Microsoft.AspNetCore.Mvc;
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
                case "-Members": return Members(path);
                case "-Fields": return Fields(path);
                case "-Events": return Events(path);
                case "-Properties": return Properties(path);
                case "-Constructors": return Constructors(path);
            }

            ISymbol? symbol = apiContent.GetSymbol(path);

            if (symbol is not null)
            {
                return symbol.Kind switch
                {
                    _ => NotFound()
                };
            }

            return NotFound();
        }

        private ViewResult Package(IPackageSymbol package)
        {
            ApiSymbolViewModel<IPackageSymbol> model = new()
            {
                Title = package.Name + " Package",
                Symbol = package,
                Navigation = apiNavigation.Create(package.Link)
            };

            return View("Package", model);
        }

        private ViewResult Namespace(INamespaceSymbol @namespace)
        {
            ApiSymbolViewModel<INamespaceSymbol> model = new()
            {
                Title = @namespace.Name + " Namespace",
                Symbol = @namespace,
                Navigation = apiNavigation.Create(@namespace.Link)
            };

            return View("Namespace", model);
        }

        private ViewResult Enum(IEnumSymbol @enum)
        {
            ApiSymbolViewModel<IEnumSymbol> model = new()
            {
                Title = @enum.Name + " Enum",
                Symbol = @enum,
                Navigation = apiNavigation.Create(@enum.Link)
            };

            return View("Enum", model);
        }

        private ViewResult Interface(IInterfaceSymbol @interface)
        {
            ApiSymbolViewModel<IInterfaceSymbol> model = new()
            {
                Title = @interface.Name + " Interface",
                Symbol = @interface,
                Navigation = apiNavigation.Create(@interface.Link)
            };

            return View("Interface", model);
        }

        private ViewResult Class(IClassSymbol @class)
        {
            ApiSymbolViewModel<IClassSymbol> model = new()
            {
                Title = @class.Name + " Class",
                Symbol = @class,
                Navigation = apiNavigation.Create(@class.Link)
            };

            return View("Class", model);
        }

        private IActionResult Members(UriPath path)
        {
            IEnumSymbol? @enum = apiContent.GetSymbol(path.Parent) as IEnumSymbol;

            if (@enum is not null)
            {
                ApiSymbolsViewModel<IMemberSymbol> model = new()
                {
                    Title = $"{@enum.Name} Members",
                    Symbols = [.. @enum.Members],
                    Navigation = apiNavigation.Create(path)
                };

                return View("Members", model);
            }

            return NotFound();
        }

        private IActionResult Fields(UriPath path)
        {
            IClassSymbol? @class = apiContent.GetSymbol(path.Parent) as IClassSymbol;

            if (@class is not null)
            {
                ApiSymbolsViewModel<IFieldSymbol> model = new()
                {
                    Title = $"{@class.Name} Fields",
                    Symbols = [.. @class.Fields],
                    Navigation = apiNavigation.Create(path)
                };

                return View("Fields", model);
            }

            return NotFound();
        }

        private IActionResult Events(UriPath path)
        {
            IClassOrInterfaceSymbol? cori = apiContent.GetSymbol(path.Parent) as IClassOrInterfaceSymbol;

            if (cori is not null)
            {
                ApiSymbolsViewModel<IEventSymbol> model = new()
                {
                    Title = $"{cori.Name} Events",
                    Symbols = [.. cori.Events],
                    Navigation = apiNavigation.Create(path)
                };

                return View("Events", model);
            }

            return NotFound();
        }

        private IActionResult Properties(UriPath path)
        {
            IClassOrInterfaceSymbol? cori = apiContent.GetSymbol(path.Parent) as IClassOrInterfaceSymbol;

            if (cori is not null)
            {
                ApiSymbolsViewModel<IPropertySymbol> model = new()
                {
                    Title = $"{cori.Name} Properties",
                    Symbols = [.. cori.Properties],
                    Navigation = apiNavigation.Create(path)
                };

                return View("Properties", model);
            }

            return NotFound();
        }

        private IActionResult Constructors(UriPath path)
        {
            IClassSymbol? @class = apiContent.GetSymbol(path.Parent) as IClassSymbol;

            if (@class is not null)
            {
                ApiSymbolsViewModel<IConstructorSymbol> model = new()
                {
                    Title = $"{@class.Name} Constructors",
                    Symbols = [.. @class.Constructors],
                    Navigation = apiNavigation.Create(path)
                };

                return View("Constructors", model);
            }

            return NotFound();
        }
    }
}
