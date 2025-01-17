using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Net.Navigating;
using Fasciculus.Threading.Synchronization;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Indexing
{
    public class SymbolIndicesFactory
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly SymbolIndicesOptions options;

        private Dictionary<UriPath, Symbol> symbols = [];

        public SymbolIndicesFactory(SymbolIndicesOptions options)
        {
            this.options = options;
        }

        public SymbolIndices Create(IEnumerable<PackageSymbol> input)
        {
            using Locker locker = Locker.Lock(mutex);

            symbols = [];

            AddPackages(input);

            return new()
            {
                Symbols = symbols,
            };
        }

        private void AddPackage(PackageSymbol package)
        {
            if (IsIncluded(package))
            {
                symbols.Add(package.Link, package);

                AddNamespaces(package.Namespaces);
            }
        }

        private void AddPackages(IEnumerable<PackageSymbol> packages)
            => packages.Apply(AddPackage);

        private void AddNamespace(NamespaceSymbol @namespace)
        {
            if (IsIncluded(@namespace))
            {
                symbols.Add(@namespace.Link, @namespace);

                AddClasses(@namespace.Classes);
                AddEnums(@namespace.Enums);
            }
        }

        private void AddNamespaces(IEnumerable<NamespaceSymbol> namespaces)
            => namespaces.Apply(AddNamespace);

        private void AddClasses(IEnumerable<ClassSymbol> classes)
            => classes.Apply(AddClass);

        private void AddClass(ClassSymbol @class)
        {
            if (IsIncluded(@class))
            {
                symbols.Add(@class.Link, @class);
            }
        }

        private void AddEnums(IEnumerable<EnumSymbol> enums)
            => enums.Apply(AddEnum);

        private void AddEnum(EnumSymbol @enum)
        {
            if (IsIncluded(@enum))
            {
                symbols.Add(@enum.Link, @enum);
            }
        }

        private bool IsIncluded(Symbol symbol)
            => options.IncludeNonAccessible || symbol.IsAccessible;
    }
}
