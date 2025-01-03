using Fasciculus.ApiDoc;
using Fasciculus.ApiDoc.Models;
using Fasciculus.IO.Searching;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.GitHub.Services
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
            { "System", "Extensions to types in the 'System' namespace." },
            { "System.Collections.Generic", "Extensions to types in the 'System.Collections.Generic' namespace." },
            { "System.IO", "Extensions to types in the 'System.IO' namespace." },
            { "System.Net.Http", "Extensions to types in the 'System.Net.Http' namespace." },
            { "System.Reflection", "Extensions to types in the 'System.Reflection' namespace." },
        };

        private readonly List<ApiPackage> packages = [];
        private readonly Dictionary<string, ApiPackage> packagesByName = [];

        public IEnumerable<ApiPackage> Packages => packages;

        public ApiProvider()
        {
            ApiDocumentsBuilder builder = new();

            AddBuilderPackages(builder);

            ApiDocuments documents = builder.Build();

            AddPackages(documents.Packages);

            SetPackageDescriptions();
            SetNamespaceDescriptions();
        }

        private void AddPackages(ApiPackages allPackages)
        {
            foreach (var name in PackageNames)
            {
                if (allPackages.TryGetPackage(name, out ApiPackage? package))
                {
                    packages.Add(package);
                    packagesByName[name] = package;
                }
            }
        }

        private void SetPackageDescriptions()
        {
            foreach (var dsc in PackageDescriptions)
            {
                if (packagesByName.TryGetValue(dsc.Key, out ApiPackage? package))
                {
                    package.Description = dsc.Value;
                }
            }
        }

        private void SetNamespaceDescriptions()
        {
            foreach (var pkg in packages)
            {
                foreach (var ns in pkg.Namespaces)
                {
                    if (NamespaceDescriptions.TryGetValue(ns.Name, out string? dsc))
                    {
                        ns.Description = dsc;
                    }
                }
            }
        }

        private static void AddBuilderPackages(ApiDocumentsBuilder builder)
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;

            PackageNames.Apply(name => { AddBuilderPackage(name, searchPath, builder); });
        }

        private static void AddBuilderPackage(string name, SearchPath searchPath, ApiDocumentsBuilder builder)
        {
            DirectoryInfo directory = DirectorySearch.Search(name, searchPath).First();
            FileInfo projectFile = directory.File($"{name}.csproj");

            builder.AddProjectFile(projectFile);
        }
    }
}
