using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Net;
using Fasciculus.Threading.Synchronization;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Indexing
{
    public class SymbolIndicesFactory
    {
        private readonly TaskSafeMutex mutex = new();

        private Dictionary<UriPath, Symbol> symbols = [];
        private Dictionary<UriPath, PackageSymbol> packages = [];
        private Dictionary<UriPath, NamespaceSymbol> namespaces = [];
        private Dictionary<UriPath, ClassSymbol> classes = [];

        private void Clear()
        {
            symbols = [];
            packages = [];
            namespaces = [];
            classes = [];
        }

        public SymbolIndices CreateIndices(IEnumerable<PackageSymbol> input)
        {
            using Locker locker = Locker.Lock(mutex);

            Clear();

            AddPackages(input);

            return new()
            {
                Symbols = symbols,
                Packages = packages,
                Namespaces = namespaces,
                Classes = classes
            };
        }

        private void AddPackage(PackageSymbol package)
        {
            symbols.Add(package.Link, package);
            packages.Add(package.Link, package);

            AddNamespaces(package.Namespaces);
        }

        private void AddPackages(IEnumerable<PackageSymbol> packages)
            => packages.Apply(AddPackage);

        private void AddNamespace(NamespaceSymbol @namespace)
        {
            symbols.Add(@namespace.Link, @namespace);
            namespaces.Add(@namespace.Link, @namespace);

            AddClasses(@namespace.Classes);
        }

        private void AddNamespaces(IEnumerable<NamespaceSymbol> namespaces)
            => namespaces.Apply(AddNamespace);

        private void AddClass(ClassSymbol @class)
        {
            symbols.Add(@class.Link, @class);
            classes.Add(@class.Link, @class);
        }

        private void AddClasses(IEnumerable<ClassSymbol> classes)
            => classes.Apply(AddClass);
    }
}
