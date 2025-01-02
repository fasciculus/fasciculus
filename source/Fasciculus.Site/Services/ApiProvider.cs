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

        public IEnumerable<ApiPackage> Packages => packages;

        public ApiProvider()
        {
            ApiDocumentsBuilder builder = new();

            AddPackages(builder);

            ApiDocuments documents = builder.Build();
            ApiPackages packages = documents.Packages;

            PackageNames.Apply(name => { packages[name].Description = PackageDescriptions[name]; });
            PackageNames.Apply(name => { this.packages.Add(packages[name]); });
        }

        private static void AddPackages(ApiDocumentsBuilder builder)
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;

            PackageNames.Apply(name => { AddPackage(name, searchPath, builder); });
        }

        private static void AddPackage(string name, SearchPath searchPath, ApiDocumentsBuilder builder)
        {
            DirectoryInfo directory = DirectorySearch.Search(name, searchPath).First();
            FileInfo projectFile = directory.File($"{name}.csproj");

            builder.AddProjectFile(projectFile);
        }
    }
}
