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
