using Fasciculus.CodeAnalysis;
using Fasciculus.CodeAnalysis.Configuration;
using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Api.Services
{
    public class ApiContent
    {
        private readonly CodeAnalyzerResult result;

        public SymbolIndex Index => result.Index;
        public PackageList Packages => result.Packages;
        public PackageSymbol Combined => result.Combined;

        public ApiContent()
        {
            result = CodeAnalyzer.Create()
                .WithProjects(GetProjects())
                .WithCombinedPackageLink(new("tree", "main", "source"))
                .Build().Analyze();
        }

        public Symbol? GetSymbol(UriPath link)
            => result.Index.TryGetSymbol(link, out Symbol? symbol) ? symbol : null;

        private static readonly string[] ProjectNames =
        [
            "Fasciculus.Core",
            "Fasciculus.Extensions"
        ];

        private static IEnumerable<CodeAnalyzerProject> GetProjects()
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;

            foreach (string projectName in ProjectNames)
            {
                DirectoryInfo directory = DirectorySearch.Search(projectName, searchPath).First();

                CodeAnalyzerProject project = new()
                {
                    ProjectFile = directory.File(projectName + ".csproj"),
                    RepositoryDirectory = new("tree", "main", "source", projectName)
                };

                yield return project;
            }
        }
    }
}
