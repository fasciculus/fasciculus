using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Services
{
    public class ApiNavigation : NavigationFactory
    {
        private readonly ApiContent content;

        public ApiNavigation(ApiContent content)
        {
            this.content = content;
        }

        public NavigationForest Create(UriPath selected)
        {
            UriPath package = new(selected.Take(1));
            UriPath prefix = new("api");

            return Create(GetChildren(package), selected, prefix);
        }

        protected override string GetLabel(UriPath link)
        {
            return content.GetSymbol(link)?.Name ?? string.Empty;
        }

        protected override IEnumerable<UriPath> GetChildren(UriPath link)
        {
            Symbol? symbol = content.GetSymbol(link);

            if (symbol is not null)
            {
                return symbol.Kind switch
                {
                    SymbolKind.Package => GetChildren((PackageSymbol)symbol),
                    SymbolKind.Namespace => GetChildren((NamespaceSymbol)symbol),
                    _ => []
                };
            }

            return [];
        }

        private static IEnumerable<UriPath> GetChildren(PackageSymbol package)
            => package.Namespaces.Select(n => n.Link);

        private static IEnumerable<UriPath> GetChildren(NamespaceSymbol @namespace)
            => @namespace.Classes.Select(n => n.Link);
    }
}
