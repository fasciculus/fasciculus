using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Net.Navigating;
using Fasciculus.Site.Models;
using Fasciculus.Site.Navigation;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Api.Services
{
    public class ApiNavigation2
    {
        private readonly ApiContent content;

        public ApiNavigation2(ApiContent content)
        {
            this.content = content;
        }

        public SiteNavigation Create()
        {
            NavigationForest forest = CreateForest();
            SitePath path = [];

            return new() { Forest = forest, Path = path };
        }

        public SiteNavigation Create(UriPath selected)
        {
            UriPath packageLink = new(selected.Take(1));
            PackageSymbol package = (PackageSymbol)content.GetSymbol(packageLink)!;
            NavigationForest forest = CreateForest(package, selected);
            SitePath path = CreateSitePath(package, forest, selected);

            return new() { Forest = forest, Path = path };
        }

        private SitePath CreateSitePath(PackageSymbol package, NavigationForest navigation, UriPath link)
        {
            SitePath result = [];

            if (link.Count > 1)
            {
                List<NavigationNode> pathTo = navigation.PathTo(ToSiteLink(link.Parent));
                Symbol[] symbols = [.. pathTo.Select(n => content.GetSymbol(ToSymbolLink(n.Link))).NotNull()];

                result.Add(NavigationKind.ApiPackage, package.Name, ToSiteLink(package.Link));
                symbols.Apply(s => { result.Add(GetKind(s), s.Name, ToSiteLink(s.Link)); });
            }

            return result;
        }

        private static NavigationForest CreateForest()
        {
            string[] packageNames = [.. SiteConstants.PackageNames.Prepend("Combined")];
            NavigationNode[] trees = [.. packageNames.Select(CreatePackageNode)];

            return new(trees);
        }

        private static NavigationNode CreatePackageNode(string packageName)
        {
            int kind = NavigationKind.ApiPackage;
            string label = packageName;
            UriPath link = ToSiteLink(new(packageName));

            return new(kind, label, link);
        }

        private static NavigationForest CreateForest(PackageSymbol package, UriPath selected)
        {
            NavigationNode[] trees = CreateNamespaceTrees(package.Namespaces, selected);

            return new(trees);
        }

        private static NavigationNode[] CreateNamespaceTrees(IEnumerable<NamespaceSymbol> namespaces, UriPath selected)
            => [.. namespaces.Select(n => CreateNamespaceNode(n, selected))];

        private static NavigationNode CreateNamespaceNode(NamespaceSymbol @namespace, UriPath selected)
        {
            int kind = NavigationKind.ApiNamespace;
            string label = @namespace.Name;
            UriPath link = ToSiteLink(@namespace.Link);
            NavigationNode overview = CreateOverview(label, link);
            IEnumerable<NavigationNode> children = [overview];
            bool isOpen = selected.IsSelfOrDescendantOf(@namespace.Link);

            if (@namespace.Enums.Any())
            {
                children = children.Append(CreateEnumsNode(@namespace, selected));
            }

            if (@namespace.Interfaces.Any())
            {
                children = children.Append(CreateInterfacesNode(@namespace, selected));
            }

            if (@namespace.Classes.Any())
            {
                children = children.Append(CreateClassesNode(@namespace, selected));
            }

            return new(kind, label, link, children, isOpen);
        }

        private static NavigationNode CreateEnumsNode(NamespaceSymbol @namespace, UriPath selected)
        {
            int kind = NavigationKind.ApiEnums;
            UriPath link = ToSiteLink(@namespace.Link);
            IEnumerable<NavigationNode> children = @namespace.Enums.Select(x => CreateEnumNode(x, selected));
            bool isOpen = children.Any(x => x.IsOpen);

            return new(kind, "Enums", link, children, isOpen);
        }

        private static NavigationNode CreateInterfacesNode(NamespaceSymbol @namespace, UriPath selected)
        {
            int kind = NavigationKind.ApiInterfaces;
            UriPath link = ToSiteLink(@namespace.Link);
            IEnumerable<NavigationNode> children = @namespace.Interfaces.Select(x => CreateInterfaceNode(x, selected));
            bool isOpen = children.Any(x => x.IsOpen);

            return new(kind, "Interfaces", link, children, isOpen);
        }

        private static NavigationNode CreateClassesNode(NamespaceSymbol @namespace, UriPath selected)
        {
            int kind = NavigationKind.ApiClasses;
            UriPath link = ToSiteLink(@namespace.Link);
            IEnumerable<NavigationNode> children = @namespace.Classes.Select(x => CreateClassNode(x, selected));
            bool isOpen = children.Any(x => x.IsOpen);

            return new(kind, "Classes", link, children, isOpen);
        }

        private static NavigationNode CreateMembersNode(EnumSymbol @enum, UriPath selected)
        {
            int kind = NavigationKind.ApiMembers;
            UriPath link = ToSiteLink(@enum.Link);
            IEnumerable<NavigationNode> children = @enum.Members.Select(x => CreateMemberNode(x, selected));
            bool isOpen = children.Any(x => x.IsOpen);

            return new(kind, "Members", link, children, isOpen);
        }

        private static NavigationNode CreateFieldsNode(ClassSymbol @class, UriPath selected)
        {
            int kind = NavigationKind.ApiFields;
            UriPath link = ToSiteLink(@class.Link);
            IEnumerable<NavigationNode> children = @class.Fields.Select(x => CreateFieldNode(x, selected));
            bool isOpen = children.Any(x => x.IsOpen);

            return new(kind, "Fields", link, children, isOpen);
        }

        private static NavigationNode CreateEventsNode(ClassSymbol @class, UriPath selected)
        {
            int kind = NavigationKind.ApiEvents;
            UriPath link = ToSiteLink(@class.Link);
            IEnumerable<NavigationNode> children = @class.Events.Select(x => CreateEventNode(x, selected));
            bool isOpen = children.Any(x => x.IsOpen);

            return new(kind, "Events", link, children, isOpen);
        }

        private static NavigationNode CreatePropertiesNode(ClassSymbol @class, UriPath selected)
        {
            int kind = NavigationKind.ApiProperties;
            UriPath link = ToSiteLink(@class.Link);
            IEnumerable<NavigationNode> children = @class.Properties.Select(x => CreatePropertyNode(x, selected));
            bool isOpen = children.Any(x => x.IsOpen);

            return new(kind, "Properties", link, children, isOpen);
        }

        private static NavigationNode CreateEnumNode(EnumSymbol @enum, UriPath selected)
        {
            int kind = NavigationKind.ApiEnum;
            string label = @enum.Name;
            UriPath link = ToSiteLink(@enum.Link);
            NavigationNode overview = CreateOverview(label, link);
            IEnumerable<NavigationNode> children = [overview];
            bool isOpen = selected.IsSelfOrDescendantOf(@enum.Link);

            if (@enum.Members.Any())
            {
                children = children.Append(CreateMembersNode(@enum, selected));
            }

            return new(kind, label, link, children, isOpen);
        }

        private static NavigationNode CreateInterfaceNode(InterfaceSymbol @interface, UriPath selected)
        {
            int kind = NavigationKind.ApiInterface;
            string label = @interface.Name;
            UriPath link = ToSiteLink(@interface.Link);
            NavigationNode overview = CreateOverview(label, link);
            IEnumerable<NavigationNode> children = [overview];
            bool isOpen = selected.IsSelfOrDescendantOf(@interface.Link);

            return new(kind, label, link, children, isOpen);
        }

        private static NavigationNode CreateClassNode(ClassSymbol @class, UriPath selected)
        {
            int kind = NavigationKind.ApiClass;
            string label = @class.Name;
            UriPath link = ToSiteLink(@class.Link);
            NavigationNode overview = CreateOverview(label, link);
            IEnumerable<NavigationNode> children = [overview];
            bool isOpen = selected.IsSelfOrDescendantOf(@class.Link);

            if (@class.Fields.Any())
            {
                children = children.Append(CreateFieldsNode(@class, selected));
            }
            if (@class.Events.Any())
            {
                children = children.Append(CreateEventsNode(@class, selected));
            }
            if (@class.Properties.Any())
            {
                children = children.Append(CreatePropertiesNode(@class, selected));
            }
            if (@class.Constructors.Any())
            {
                children = children.Append(CreateConstructorsNode(@class, selected));
            }

            return new(kind, label, link, children, isOpen);
        }

        private static NavigationNode CreateMemberNode(MemberSymbol member, UriPath selected)
        {
            int kind = NavigationKind.ApiMember;
            string label = member.Name;
            UriPath link = ToSiteLink(member.Link);
            bool isOpen = selected.IsSelfOrDescendantOf(member.Link);

            return new(kind, label, link, isOpen);
        }

        private static NavigationNode CreateFieldNode(FieldSymbol field, UriPath selected)
        {
            int kind = NavigationKind.ApiField;
            string label = field.Name;
            UriPath link = ToSiteLink(field.Link);
            bool isOpen = selected.IsSelfOrDescendantOf(field.Link);

            return new(kind, label, link, isOpen);
        }

        private static NavigationNode CreateEventNode(EventSymbol @event, UriPath selected)
        {
            int kind = NavigationKind.ApiEvent;
            string label = @event.Name;
            UriPath link = ToSiteLink(@event.Link);
            bool isOpen = selected.IsSelfOrDescendantOf(@event.Link);

            return new(kind, label, link, isOpen);
        }

        private static NavigationNode CreatePropertyNode(PropertySymbol property, UriPath selected)
        {
            int kind = NavigationKind.ApiProperty;
            string label = property.Name;
            UriPath link = ToSiteLink(property.Link);
            bool isOpen = selected.IsSelfOrDescendantOf(property.Link);

            return new(kind, label, link, isOpen);
        }

        private static NavigationNode CreateConstructorsNode(ClassSymbol @class, UriPath selected)
        {
            int kind = NavigationKind.ApiConstructors;
            string label = "Constructors";
            UriPath link = ToSiteLink(@class.Link).Append("-Constructors");
            bool isOpen = selected.IsSelfOrDescendantOf(@class.Link);

            return new(kind, label, link, isOpen);
        }

        private static NavigationNode CreateOverview(string label, UriPath link)
        {
            int kind = NavigationKind.Overview;

            return new(kind, label, link);
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
                SymbolKind.Member => NavigationKind.ApiMember,
                SymbolKind.Event => NavigationKind.ApiEvent,
                SymbolKind.Property => NavigationKind.ApiProperty,
                _ => NavigationKind.Unknown
            };
        }

        private static UriPath ToSymbolLink(UriPath apiLink)
            => new(apiLink.Skip(1));

        private static UriPath ToSiteLink(UriPath symbolLink)
            => new(symbolLink.Prepend("api"));
    }
}
