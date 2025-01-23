using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Net.Navigating;
using Fasciculus.Site.Models;
using Fasciculus.Site.Navigation;
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

        public SiteNavigation Create(UriPath selected)
        {
            UriPath packageLink = new(selected.Take(1));
            PackageSymbol package = (PackageSymbol)content.GetSymbol(packageLink)!;
            NavigationForest forest = Create(GetChildren(package), ToApiLink(selected));
            SitePath path = CreateSitePath(forest, selected);

            return new() { Forest = forest, Path = path };
        }

        private SitePath CreateSitePath(NavigationForest navigation, UriPath link)
        {
            SitePath result = [];

            if (link.Count > 1)
            {
                List<NavigationNode> pathTo = navigation.PathTo(ToApiLink(link.Parent));
                Symbol[] symbols = [.. pathTo.Select(n => content.GetSymbol(ToSymbolLink(n.Link))).NotNull()];
                UriPath packageLink = new(link.Take(1));
                PackageSymbol package = (PackageSymbol)content.GetSymbol(packageLink)!;

                result.Add(NavigationKind.ApiPackage, package.Name, ToApiLink(packageLink));
                symbols.Apply(s => { result.Add(GetKind(s), s.Name, ToApiLink(s.Link)); });
            }

            return result;
        }

        protected override int GetKind(UriPath link)
        {
            Symbol? symbol = content.GetSymbol(ToSymbolLink(link));

            return symbol is null ? NavigationKind.Unknown : GetKind(symbol);
        }

        private static int GetKind(Symbol symbol)
        {
            return symbol.Kind switch
            {
                SymbolKind.Package => NavigationKind.ApiPackage,
                SymbolKind.Namespace => NavigationKind.ApiNamespace,
                SymbolKind.Enum => NavigationKind.ApiEnum,
                SymbolKind.Interface => NavigationKind.ApiInterface,
                SymbolKind.Class => NavigationKind.ApiClass,
                SymbolKind.Field => NavigationKind.ApiField,
                SymbolKind.Member => NavigationKind.ApiEnumMember,
                SymbolKind.Event => NavigationKind.ApiEvent,
                SymbolKind.Property => NavigationKind.ApiProperty,
                _ => NavigationKind.Unknown
            };
        }

        protected override string GetLabel(UriPath link)
        {
            Symbol? symbol = content.GetSymbol(ToSymbolLink(link));

            if (symbol is not null)
            {
                return symbol.Name;
            }

            if (link[^1] == "Fields")
            {
                return "Fields";
            }

            if (link[^1] == "Members")
            {
                return "Members";
            }

            if (link[^1] == "Events")
            {
                return "Events";
            }

            if (link[^1] == "Properties")
            {
                return "Properties";
            }

            return string.Empty;
        }

        protected override bool IsOpen(UriPath link, UriPath selected)
        {
            if (link.Count > 0)
            {
                return link[^1] switch
                {
                    "Fields" => base.IsOpen(link.Parent, selected),
                    "Members" => base.IsOpen(link.Parent, selected),
                    "Events" => base.IsOpen(link.Parent, selected),
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
                    SymbolKind.Enum => GetChildren((EnumSymbol)symbol),
                    SymbolKind.Interface => GetChildren((InterfaceSymbol)symbol),
                    SymbolKind.Class => GetChildren((ClassSymbol)symbol),
                    _ => []
                };
            }

            if (link[^1] == "Fields")
            {
                return Fields(link);
            }

            if (link[^1] == "Members")
            {
                return Members(link);
            }

            if (link[^1] == "Events")
            {
                return Events(link);
            }

            if (link[^1] == "Properties")
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

        private static IEnumerable<UriPath> GetChildren(EnumSymbol @enum)
        {
            if (@enum.Members.Any())
            {
                yield return ToApiLink(@enum.Link).Append("Members");
            }
        }

        private static IEnumerable<UriPath> GetChildren(InterfaceSymbol @interface)
        {
            if (@interface.Events.Any())
            {
                yield return ToApiLink(@interface.Link).Append("Events");
            }

            if (@interface.Properties.Any())
            {
                yield return ToApiLink(@interface.Link).Append("Properties");
            }
        }

        private static IEnumerable<UriPath> GetChildren(ClassSymbol @class)
        {
            if (@class.Fields.Any())
            {
                yield return ToApiLink(@class.Link).Append("Fields");
            }

            if (@class.Events.Any())
            {
                yield return ToApiLink(@class.Link).Append("Events");
            }

            if (@class.Properties.Any())
            {
                yield return ToApiLink(@class.Link).Append("Properties");
            }
        }

        private IEnumerable<UriPath> Fields(UriPath link)
        {
            Symbol? symbol = content.GetSymbol(ToSymbolLink(link.Parent));

            if (symbol is not null)
            {
                return symbol.Kind switch
                {
                    SymbolKind.Class => Fields((ClassSymbol)symbol),
                    _ => []
                };
            }

            return [];
        }

        private static IEnumerable<UriPath> Fields(ClassSymbol @class)
            => @class.Fields.Select(p => ToApiLink(p.Link));

        private IEnumerable<UriPath> Members(UriPath link)
        {
            Symbol? symbol = content.GetSymbol(ToSymbolLink(link.Parent));

            if (symbol is not null)
            {
                return symbol.Kind switch
                {
                    SymbolKind.Enum => Members((EnumSymbol)symbol),
                    _ => []
                };
            }

            return [];
        }

        private static IEnumerable<UriPath> Members(EnumSymbol @enum)
            => @enum.Members.Select(p => ToApiLink(p.Link));

        private IEnumerable<UriPath> Events(UriPath link)
        {
            Symbol? symbol = content.GetSymbol(ToSymbolLink(link.Parent));

            if (symbol is not null)
            {
                return symbol.Kind switch
                {
                    SymbolKind.Interface => Events((InterfaceSymbol)symbol),
                    SymbolKind.Class => Events((ClassSymbol)symbol),
                    _ => []
                };
            }

            return [];
        }

        private static IEnumerable<UriPath> Events(InterfaceSymbol @interface)
            => @interface.Events.Select(p => ToApiLink(p.Link));

        private static IEnumerable<UriPath> Events(ClassSymbol @class)
            => @class.Events.Select(p => ToApiLink(p.Link));

        private IEnumerable<UriPath> Properties(UriPath link)
        {
            Symbol? symbol = content.GetSymbol(ToSymbolLink(link.Parent));

            if (symbol is not null)
            {
                return symbol.Kind switch
                {
                    SymbolKind.Interface => Properties((InterfaceSymbol)symbol),
                    SymbolKind.Class => Properties((ClassSymbol)symbol),
                    _ => []
                };
            }

            return [];
        }

        private static IEnumerable<UriPath> Properties(InterfaceSymbol @interface)
            => @interface.Properties.Select(p => ToApiLink(p.Link));

        private static IEnumerable<UriPath> Properties(ClassSymbol @class)
            => @class.Properties.Select(p => ToApiLink(p.Link));

        private static UriPath ToSymbolLink(UriPath apiLink)
            => new(apiLink.Skip(1));

        private static UriPath ToApiLink(UriPath symbolLink)
            => new(symbolLink.Prepend("api"));
    }
}
