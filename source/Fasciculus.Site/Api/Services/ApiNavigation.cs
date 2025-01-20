using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Fasciculus.Site.Models;
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

        protected override int GetKind(UriPath link)
        {
            Symbol? symbol = content.GetSymbol(ToSymbolLink(link));

            if (symbol is not null)
            {
                return symbol.Kind switch
                {
                    SymbolKind.Package => NavigationKind.ApiPackage,
                    SymbolKind.Namespace => NavigationKind.ApiNamespace,
                    SymbolKind.Enum => NavigationKind.ApiEnum,
                    SymbolKind.Interface => NavigationKind.ApiInterface,
                    SymbolKind.Class => NavigationKind.ApiClass,
                    SymbolKind.Property => NavigationKind.ApiProperty,
                    _ => NavigationKind.Unknown
                };
            }

            return NavigationKind.Unknown;
        }

        protected override string GetLabel(UriPath link)
        {
            Symbol? symbol = content.GetSymbol(ToSymbolLink(link));

            if (symbol is not null)
            {
                return symbol.Name;
            }

            if (link.Last() == "Properties")
            {
                return "Properties";
            }

            return string.Empty;
        }

        protected override bool IsOpen(UriPath link, UriPath selected)
        {
            if (link.Count > 0)
            {
                return link[link.Count - 1] switch
                {
                    "Properties" => base.IsOpen(link.Parent, selected),
                    _ => base.IsOpen(link, selected),
                };
            }

            return base.IsOpen(link, selected);
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
                    SymbolKind.Class => GetChildren((ClassSymbol)symbol),
                    _ => []
                };
            }

            if (link.Last() == "Properties")
            {
                return Properties(link);
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

        private static IEnumerable<UriPath> GetChildren(ClassSymbol @class)
        {
            if (@class.Properties.Any())
            {
                yield return ToApiLink(@class.Link).Append("Properties");
            }
        }

        private IEnumerable<UriPath> Properties(UriPath link)
        {
            Symbol? symbol = content.GetSymbol(ToSymbolLink(link.Parent));

            if (symbol is not null)
            {
                return symbol.Kind switch
                {
                    SymbolKind.Class => Properties((ClassSymbol)symbol),
                    _ => []
                };
            }

            return [];
        }

        private static IEnumerable<UriPath> Properties(ClassSymbol @class)
            => @class.Properties.Select(p => ToApiLink(p.Link));

        private static UriPath ToSymbolLink(UriPath apiLink)
            => new(apiLink.Skip(1));

        private static UriPath ToApiLink(UriPath symbolLink)
            => new(symbolLink.Prepend("api"));
    }
}
