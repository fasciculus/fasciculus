using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Api.Services
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
            UriPath packageLink = new(selected.Take(1));
            PackageSymbol package = (PackageSymbol)content.GetSymbol(packageLink)!;

            return Create(GetChildren(package), ToApiLink(selected));
        }

        protected override string GetLabel(UriPath link)
        {
            return content.GetSymbol(ToSymbolLink(link))?.Name ?? string.Empty;
        }

        protected override IEnumerable<UriPath> GetChildren(UriPath link)
        {
            Symbol? symbol = content.GetSymbol(ToSymbolLink(link));

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
            => package.Namespaces.Select(x => ToApiLink(x.Link));

        private static IEnumerable<UriPath> GetChildren(NamespaceSymbol @namespace)
        {
            IEnumerable<Symbol> symbols = Enumerable.Empty<Symbol>()
                .Concat(@namespace.Enums)
                .Concat(@namespace.Interfaces)
                .Concat(@namespace.Classes);

            return symbols.OrderBy(s => s.Name).Select(s => ToApiLink(s.Link));
        }

        private static UriPath ToSymbolLink(UriPath apiLink)
            => new(apiLink.Skip(1));

        private static UriPath ToApiLink(UriPath symbolLink)
            => new(symbolLink.Prepend("api"));
    }
}
