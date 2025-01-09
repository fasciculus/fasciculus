using Fasciculus.ApiDoc;
using Fasciculus.ApiDoc.Models;
using Fasciculus.CodeAnalysis;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Fasciculus.Net.Navigating;
using Fasciculus.Site.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Services
{
    public class ApiProvider
    {
        private static readonly string[] PackageNames =
        [
            "Fasciculus.Core",
            "Fasciculus.Extensions"
        ];

        private static readonly Dictionary<string, string> PackageDescriptions = new()
        {
            { "Fasciculus.Core", "Common utilities." },
            { "Fasciculus.Extensions", "Extensions with MVVM and dependency injection." }
        };

        private static readonly Dictionary<string, string> NamespaceDescriptions = new()
        {
            { "Fasciculus.Algorithms", "Fast binary search, bit operations, fast array equality." },
        };

        private readonly CodeAnalyzerResult content;

        public PackageList Packages => content.Packages;

        public ApiPackages OldPackages { get; }
        public ApiPackage OldCombined { get; }

        public ApiProvider()
        {
            content = GetContent();

            ApiDocBuilder builder = SetupApiDocBuilder();
            ApiDocuments documents = builder.Build();

            OldPackages = documents.Packages;
            OldCombined = documents.Combined;

            SetPackageDescriptions();
            SetNamespaceDescriptions();
        }

        public Symbol? GetSymbol(UriPath link)
            => content.Indices.Symbols.TryGetValue(link, out Symbol? symbol) ? symbol : null;

        public ApiPackage GetPackage(UriPath link)
        {
            string name = link[0];

            if ("Combined" == name)
            {
                return OldCombined;
            }

            return OldPackages.First(p => p.Name == name);
        }

        public ApiNamespace GetNamespace(UriPath link)
        {
            string name = link[1];

            return GetPackage(link).Namespaces.First(n => n.Name == name);
        }

        public ApiClass GetClass(UriPath link)
        {
            return GetNamespace(link).Classes.First(c => c.Link[2] == link[2]);
        }

        public NavigationForest GetNavigation(UriPath link)
        {
            NavigationForest navigation = new(new("api"));
            ApiPackage package = GetPackage(link);

            foreach (ApiNamespace @namespace in package.Namespaces)
            {
                bool namespaceOpen = link.IsSelfOrDescendantOf(@namespace.Link);
                NavigationNode namespaceNode = new(@namespace.Name, @namespace.Link, namespaceOpen);

                foreach (var @class in @namespace.Classes)
                {
                    NavigationNode classNode = new(@class.Name, @class.Link, link.IsSelfOrDescendantOf(@class.Link));

                    namespaceNode.Add(classNode);
                }

                navigation.Add(namespaceNode);
            }

            return navigation;
        }

        private void SetPackageDescriptions()
        {
            foreach (ApiPackage package in OldPackages)
            {
                if (PackageDescriptions.TryGetValue(package.Name, out string? description))
                {
                    package.Description = description;
                }
            }
        }

        private void SetNamespaceDescriptions()
        {
            foreach (ApiPackage package in OldPackages.Append(OldCombined))
            {
                foreach (ApiNamespace @namespace in package.Namespaces)
                {
                    if (NamespaceDescriptions.TryGetValue(@namespace.Name, out string? description))
                    {
                        @namespace.Description = description;
                    }
                }
            }
        }

        private static ApiDocBuilder SetupApiDocBuilder()
        {
            ApiDocBuilder builder = new();

            GetProjectFiles().Apply(f => { builder.AddProjectFile(f); });

            return builder;
        }

        private static CodeAnalyzerResult GetContent()
        {
            return CodeAnalyzer.Create()
                .WithProjectFiles(GetProjectFiles())
                .Build().Analyze();
        }

        private static IEnumerable<FileInfo> GetProjectFiles()
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;

            foreach (string packageName in PackageNames)
            {
                DirectoryInfo? directory = DirectorySearch.Search(packageName, searchPath).FirstOrDefault();

                if (directory is not null)
                {
                    FileInfo file = directory.File($"{packageName}.csproj");

                    if (file.Exists)
                    {
                        yield return file;
                    }
                }
            }
        }
    }
}
