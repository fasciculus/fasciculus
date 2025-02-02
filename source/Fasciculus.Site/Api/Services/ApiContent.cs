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
        private readonly ICodeAnalyzerResult result;

        public ISymbolIndex Index => result.Index;
        public IPackageSymbol[] Packages => result.Packages;
        public IPackageSymbol Combined => result.Combined;

        public ApiContent()
        {
            result = CodeAnalyzer.Create()
                .WithProjects(GetProjects())
                .WithCombinedPackageName(SiteConstants.CombinedPackageName)
                .WithCombinedPackageLink(new("tree", "main", "source"))
                .Build().Analyze();
        }

        public ISymbol? GetSymbol(UriPath link)
            => Index.TryGetSymbol(link, out ISymbol? symbol) ? symbol : null;

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
                    File = directory.File(projectName + ".csproj"),
                    Repository = new($"https://github.com/fasciculus/fasciculus/tree/main/source/{projectName}/"),
                };

                yield return project;
            }
        }
    }
}
