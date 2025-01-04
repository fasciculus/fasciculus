using Fasciculus.ApiDoc;
using Fasciculus.ApiDoc.Models;
using Fasciculus.Collections;
using Fasciculus.IO;
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
            { "System.Collections.Generic", "Extensions to types in the 'System.Collections.Generic' namespace." },
            { "System.IO", "Extensions to types in the 'System.IO' namespace." },
            { "System.Net.Http", "Extensions to types in the 'System.Net.Http' namespace." },
            { "System.Reflection", "Extensions to types in the 'System.Reflection' namespace." },
        };

        public ApiPackages Packages { get; }

        public ApiProvider()
        {
            ApiDocBuilder builder = SetupApiDocBuilder();
            ApiDocuments documents = builder.Build();

            Packages = documents.Packages;

            SetPackageDescriptions();
            SetNamespaceDescriptions();
        }

        private void SetPackageDescriptions()
        {
            foreach (ApiPackage package in Packages)
            {
                if (PackageDescriptions.TryGetValue(package.Name, out string? description))
                {
                    package.Description = description;
                }
            }
        }

        private void SetNamespaceDescriptions()
        {
            foreach (ApiPackage package in Packages)
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
