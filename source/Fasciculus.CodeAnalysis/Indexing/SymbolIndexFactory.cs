using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Net.Navigating;
using Fasciculus.Threading.Synchronization;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Indexing
{
    public class SymbolIndexFactory
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly bool IncludeNonAccessible;

        private Dictionary<UriPath, Symbol> index = [];

        public SymbolIndexFactory(SymbolIndexOptions options)
        {
            IncludeNonAccessible = options.IncludeNonAccessible;
        }

        public SymbolIndex Create(IEnumerable<PackageSymbol> input)
        {
            using Locker locker = Locker.Lock(mutex);

            index = [];

            AddPackages(input);

            return new(index);
        }

        private void AddPackages(IEnumerable<PackageSymbol> packages)
            => packages.Apply(AddPackage);

        private void AddPackage(PackageSymbol package)
        {
            if (IsIncluded(package))
            {
                index.Add(package.Link, package);

                AddNamespaces(package.Namespaces);
            }
        }

        private void AddNamespaces(IEnumerable<NamespaceSymbol> namespaces)
            => namespaces.Apply(AddNamespace);

        private void AddNamespace(NamespaceSymbol @namespace)
        {
            if (IsIncluded(@namespace))
            {
                index.Add(@namespace.Link, @namespace);

                AddEnums(@namespace.Enums);
                AddInterfaces(@namespace.Interfaces);
                AddClasses(@namespace.Classes);
            }
        }

        private void AddEnums(IEnumerable<EnumSymbol> enums)
            => enums.Apply(AddEnum);

        private void AddEnum(EnumSymbol @enum)
        {
            if (IsIncluded(@enum))
            {
                index.Add(@enum.Link, @enum);
                AddProperties(@enum.Properties);
            }
        }

        private void AddInterfaces(IEnumerable<InterfaceSymbol> interfaces)
            => interfaces.Apply(AddInterface);

        private void AddInterface(InterfaceSymbol @interface)
        {
            if (IsIncluded(@interface))
            {
                index.Add(@interface.Link, @interface);
                AddProperties(@interface.Properties);
            }
        }

        private void AddClasses(IEnumerable<ClassSymbol> classes)
            => classes.Apply(AddClass);

        private void AddClass(ClassSymbol @class)
        {
            if (IsIncluded(@class))
            {
                index.Add(@class.Link, @class);
                AddProperties(@class.Properties);
            }
        }

        private void AddProperties(IEnumerable<PropertySymbol> properties)
            => properties.Apply(AddProperty);

        private void AddProperty(PropertySymbol property)
        {
            if (IsIncluded(property))
            {
                index.Add(property.Link, property);
            }
        }

        private bool IsIncluded(Symbol symbol)
            => IncludeNonAccessible || symbol.IsAccessible;
    }
}
